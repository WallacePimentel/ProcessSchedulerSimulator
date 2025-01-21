using System.Collections;

namespace ProcessScheduler.Models;

public class Cpu
{
    public int Id { get; set; }
    public bool IsInUse { get; set; }
    private Queue<Process> _readyQueue;
    private Queue<Process> _blockedQueue;
    private MemoryManager _memoryManager;
    public Process _cpuProcess { get; set; }
    private int _quantum = 4;
    
    public Cpu(int id, Queue<Process> readyQueue, Queue<Process> blockedQueue, MemoryManager memoryManager)
    {
        Id = id;
        IsInUse = false;
        _readyQueue = readyQueue;
        _blockedQueue = blockedQueue;
        _memoryManager = memoryManager;
    }

    public void StartExecution()
    {
        while (true)
        {
            if (IsInUse)
            {
                if (_cpuProcess.Phase == ProcessPhase.Phase1)
                {
                    //Thread.Sleep(2000);
                    Thread.Sleep(Math.Min(_cpuProcess.CpuPhase1Duration, _quantum) * 100);
                    
                    ExecutePhase1(_cpuProcess);
                }
                else if (_cpuProcess.Phase == ProcessPhase.Phase2)
                {
                    //Thread.Sleep(2000);
                    Thread.Sleep(Math.Min(_cpuProcess.CpuPhase2Duration, _quantum) * 100);
                    
                    ExecutePhase2(_cpuProcess);
                    _cpuProcess = null;
                }
                IsInUse = false;
            }
        }
    }
    
    private void ExecutePhase1(Process process)
    {
        int timeSlice = Math.Min(process.CpuPhase1Duration, _quantum);

        for (int i = 0; i < timeSlice; i++)
        {
            process.CpuPhase1Duration--;
            Thread.Sleep(100);
            Console.WriteLine($"Processo {process.Id} em execução (Fase 1). Tempo restante: {process.CpuPhase1Duration}");

            if (process.CpuPhase1Duration == 0)
            {
                Console.WriteLine($"Processo {process.Id} terminou a fase 1. Movendo para I/O.");
                process.State = ProcessState.Blocked;
                process.Phase = ProcessPhase.Io;
                _blockedQueue.Enqueue(process);
                return;
            }
        }

        Console.WriteLine($"Quantum expirado para o processo {process.Id} (Fase 1). Reenfileirando para a fila de prontos.");
        _readyQueue.Enqueue(process);
    }

    private void ExecutePhase2(Process process)
    {
        int timeSlice = Math.Min(process.CpuPhase2Duration, _quantum);

        for (int i = 0; i < timeSlice; i++)
        {
            process.CpuPhase2Duration--;
            Thread.Sleep(100); 
            Console.WriteLine($"Processo {process.Id} em execução (Fase 2). Tempo restante: {process.CpuPhase2Duration}");

            if (process.CpuPhase2Duration == 0)
            {
                Console.WriteLine($"Processo {process.Id} terminou a fase 2. Liberando memória.");
                _memoryManager.ReleaseMemory(process);
                Console.WriteLine($"Processo {process.Id} finalizado.");
                return;
            }
        }

        Console.WriteLine($"Quantum expirado para o processo {process.Id} (Fase 2). Reenfileirando para a fila de prontos.");
        _readyQueue.Enqueue(process);
    }
}