using System.Collections;
using ProcessScheduler.Models;

public class Dispatcher
{
    private Queue<Process> _readyQueue;
    private Queue<Process> _blockedQueue;
    private MemoryManager _memoryManager;
    private Queue<Process> _newQueue;
    private List<Cpu> _cpuList;
    private IoDevice _ioDevice;
    private int _quantum = 4;
    private int _ioQuantum = 3;

    public Dispatcher(Queue<Process> readyQueue,Queue<Process> blockedQueue, Queue<Process> newQueue, MemoryManager memoryManager, List<Cpu> cpuList, IoDevice ioDevice)
    {
        _readyQueue = readyQueue;
        _blockedQueue = blockedQueue;
        _newQueue = newQueue;
        _memoryManager = memoryManager;
        _ioDevice = ioDevice;
        _cpuList = cpuList;
    }

    public void StartScheduling()
    {
        while (true)
        {
            // Fase 1: Processos novos
            if (_newQueue.Count > 0)
            {
                Process newProcess = _newQueue.Peek();

                if (_memoryManager.GetAvailableMemory() >= newProcess.RamRequired)
                {
                    newProcess.Phase = ProcessPhase.Phase1;
                    _readyQueue.Enqueue(newProcess);
                    _memoryManager.AllocateMemory(newProcess);
                    Console.WriteLine($"Processo {newProcess.Id} movido para a fila de prontos.");
                    _newQueue.Dequeue();
                }
                else
                {
                    Console.WriteLine($"Memória insuficiente para o processo {newProcess.Id}. Aguardando.");
                }
            }

            // Fase 2: Processos prontos
            if (_readyQueue.Count > 0)
            {

                /*
                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine(_cpuList[i].IsInUse);
                }
                */
                
                var availableCpu = _cpuList.FirstOrDefault(cpu => !cpu.IsInUse);

                if (availableCpu != null) {
                    
                    Process currentProcess = _readyQueue.Dequeue();
                    currentProcess.State = ProcessState.Running;
                    
                    Console.WriteLine($"Iniciando execução do processo: {currentProcess.Id} na CPU {availableCpu.Id}");

                    availableCpu._cpuProcess = currentProcess;
                    
                    availableCpu.IsInUse = true;
                }
            }

            // Fase 3: Processos bloqueados (I/O)
            ProcessBlockedQueue();

            Thread.Sleep(500);
        }
    }

    private void ProcessBlockedQueue()
    {
        foreach (var blockedProcess in _blockedQueue.ToList())
        {
            if (_ioDevice.IsInUse)
            {
                Console.WriteLine($"Dispositivo de I/O ocupado. Processo {blockedProcess.Id} aguardando.");
                continue;
            }

            _ioDevice.IsInUse = true;
            Console.WriteLine($"Dispositivo de I/O alocado para o processo {blockedProcess.Id}.");

            int ioTimeSlice = Math.Min(blockedProcess.IoPhaseDuration, _ioQuantum);

            for (int i = 0; i < ioTimeSlice; i++)
            {
                blockedProcess.IoPhaseDuration--;
                Thread.Sleep(100);
                Console.WriteLine($"Processo {blockedProcess.Id} executando I/O. Tempo restante: {blockedProcess.IoPhaseDuration}");

                if (blockedProcess.IoPhaseDuration == 0)
                {
                    Console.WriteLine($"Processo {blockedProcess.Id} terminou a fase de I/O.");
                    blockedProcess.State = ProcessState.Ready;
                    blockedProcess.Phase = ProcessPhase.Phase2;
                    _readyQueue.Enqueue(blockedProcess);
                    _blockedQueue.Dequeue();
                    break;
                }
            }
            _ioDevice.IsInUse = false;
        }
    }
}
