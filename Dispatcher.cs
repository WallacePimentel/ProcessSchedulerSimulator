using System.Collections;
using ProcessScheduler.Models;
using Spectre.Console;

public class Dispatcher
{
    //Refêrencia da fila de prontos
    private Queue<Process> _readyQueue;

    //Referência da fila de bloqueados
    private Queue<Process> _blockedQueue;

    //Referência do gerenciador de memória
    private MemoryManager _memoryManager;

    //Referência da fila de novos
    private Queue<Process> _newQueue;

    //Referência da lista de CPUs
    private List<Cpu> _cpuList;

    //Referência do dispositivo de entrada e saída
    private IoDevice _ioDevice;

    //Quantum do dispositivo de entrada e saída
    private int _ioQuantum = 3;

    //Construtor do escalonador
    public Dispatcher(Queue<Process> readyQueue,Queue<Process> blockedQueue, Queue<Process> newQueue, MemoryManager memoryManager, List<Cpu> cpuList, IoDevice ioDevice)
    {
        _readyQueue = readyQueue;
        _blockedQueue = blockedQueue;
        _newQueue = newQueue;
        _memoryManager = memoryManager;
        _ioDevice = ioDevice;
        _cpuList = cpuList;
    }

    //Função principal da Thread do escalonador
    public void StartScheduling()
    {
        while (true)
        {
            // Fase 1: Processos novos
            if (_newQueue.Count > 0)
            {
                //Vizualizando se há memória suficiente pro processo
                Process newProcess = _newQueue.Peek();

                //Se há memória disponível:
                if (_memoryManager.GetAvailableMemory() >= newProcess.RamRequired)
                {
                    newProcess.Phase = ProcessPhase.Phase1;

                    //Colocando o novo processo na fila de prontos
                    _readyQueue.Enqueue(newProcess);
                    _memoryManager.AllocateMemory(newProcess);
                    AnsiConsole.Markup($"[fuchsia]Processo {newProcess.Id} movido para a fila de prontos.[/]\n");
                    _newQueue.Dequeue();
                }
                else
                {
                    AnsiConsole.Markup($"[red]Memória insuficiente para o processo {newProcess.Id}. Aguardando.[/]\n");
                }
            }

            // Fase 2: Processos prontos
            if (_readyQueue.Count > 0)
            {
                //Verificando se há alguma CPU disponível
                var availableCpu = _cpuList.FirstOrDefault(cpu => !cpu.IsInUse);

                if (availableCpu != null) {
                    
                    //Pegando o próximo processo a executar da fila
                    Process currentProcess = _readyQueue.Dequeue();
                    currentProcess.State = ProcessState.Running;
                    
                    Console.WriteLine($"Iniciando execução do processo: {currentProcess.Id} na CPU {availableCpu.Id}");

                    //Passando o processo para a CPU
                    availableCpu._cpuProcess = currentProcess;
                    
                    //Definindo o boolean "Está em uso" da CPU atual com verdadeiro
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
        //Verificando se há processos na fila de bloqueados
        foreach (var blockedProcess in _blockedQueue.ToList())
        {
            //Se o IO estiver ocupado
            if (_ioDevice.IsInUse)
            {
                Console.WriteLine($"Dispositivo de I/O ocupado. Processo {blockedProcess.Id} aguardando.");
                continue;
            }

            //Se IO estiver livre
            _ioDevice.IsInUse = true;
            Console.WriteLine($"Dispositivo de I/O alocado para o processo {blockedProcess.Id}.");

            //Calculando quantum do IO
            int ioTimeSlice = Math.Min(blockedProcess.IoPhaseDuration, _ioQuantum);

            for (int i = 0; i < ioTimeSlice; i++)
            {
                //Executando a fase de IO do processo
                blockedProcess.IoPhaseDuration--;
                Thread.Sleep(100);
                AnsiConsole.Markup($"[cyan]Processo {blockedProcess.Id} executando I/O. Tempo restante: {blockedProcess.IoPhaseDuration}[/]\n");

                //Se a fase de IO acabou, seta fase do processo como 2 e coloca ele de volta na fila de prontos
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
            //Liberando o IO
            _ioDevice.IsInUse = false;
        }
    }
}
