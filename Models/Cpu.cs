using System.Collections;
using Spectre.Console;

namespace ProcessScheduler.Models;

public class Cpu
{
    public int Id { get; set; }

    //Booleano de aviso da ocupação de CPU
    public bool IsInUse { get; set; }

    //Referência da fila de prontos
    private Queue<Process> _readyQueue;

    //Referência da fila de bloqueados
    private Queue<Process> _blockedQueue;

    //Referência do gerenciador de memória
    private MemoryManager _memoryManager;

    //Processo presente na CPU
    public Process _cpuProcess { get; set; }
    private int _quantum = 4;
    
    //Construtor de CPU
    public Cpu(int id, Queue<Process> readyQueue, Queue<Process> blockedQueue, MemoryManager memoryManager)
    {
        Id = id;
        IsInUse = false;
        _readyQueue = readyQueue;
        _blockedQueue = blockedQueue;
        _memoryManager = memoryManager;
    }

    //Método executado como thread de cada CPU
    public void StartExecution()
    {
        while (true)
        {
            //Verificando se a CPU está ativada
            if (IsInUse)
            {
                //Se o processo estiver na fase 1
                if (_cpuProcess.Phase == ProcessPhase.Phase1)
                {
                    Thread.Sleep(Math.Min(_cpuProcess.CpuPhase1Duration, _quantum) * 100);
                    
                    ExecutePhase1(_cpuProcess);
                }

                //Se o processo estiver na fase 2
                else if (_cpuProcess.Phase == ProcessPhase.Phase2)
                {
                    Thread.Sleep(Math.Min(_cpuProcess.CpuPhase2Duration, _quantum) * 100);
                    
                    ExecutePhase2(_cpuProcess);
                    _cpuProcess = null;
                }

                //Liberando a CPU para uso
                IsInUse = false;
            }
        }
    }
    
    private void ExecutePhase1(Process process)
    {
        //Calculando o próximo quantum de escalonamento
        int timeSlice = Math.Min(process.CpuPhase1Duration, _quantum);

        for (int i = 0; i < timeSlice; i++)
        {
            //Executando a fase 1
            process.CpuPhase1Duration--;
            Thread.Sleep(100);
            AnsiConsole.Markup($"[yellow]Processo {process.Id} em execução (Fase 1). Tempo restante: {process.CpuPhase1Duration}[/]\n");
            //Console.WriteLine($"Processo {process.Id} em execução (Fase 1). Tempo restante: {process.CpuPhase1Duration}");

            //Se a fase 1 acabou
            if (process.CpuPhase1Duration == 0)
            {
                Console.WriteLine($"-----Processo {process.Id} terminou a fase 1. Movendo para I/O.");

                //Colocando o estado do processo como bloqueado e colocando ele na fila de bloqueados
                process.State = ProcessState.Blocked;
                process.Phase = ProcessPhase.Io;
                _blockedQueue.Enqueue(process);
                return;
            }
        }

        //Caso a fase 1 não tenha acabado (preempção por quantum), colocar ele de volta na fila de prontos
        AnsiConsole.Markup($"[mediumpurple1]Quantum expirado para o processo {process.Id} (Fase 1). Reenfileirando para a fila de prontos.[/]\n");
        _readyQueue.Enqueue(process);
    }

    private void ExecutePhase2(Process process)
    {
        //Calculando o próximo quantum de escalonamento
        int timeSlice = Math.Min(process.CpuPhase2Duration, _quantum);

        for (int i = 0; i < timeSlice; i++)
        {
            //Executando a fase 2
            process.CpuPhase2Duration--;
            Thread.Sleep(100); 
            AnsiConsole.Markup($"[green]Processo {process.Id} em execução (Fase 2). Tempo restante: {process.CpuPhase2Duration}[/]\n");

            //Se a fase 2 acabou
            if (process.CpuPhase2Duration == 0)
            {
                Console.WriteLine($"-----Processo {process.Id} terminou a fase 2. Liberando memória.");

                //Liberar memória
                _memoryManager.ReleaseMemory(process);
                Console.WriteLine($"+++++++++++++++Processo {process.Id} finalizado.");
                return;
            }
        }

        //Se a fase 2 não acabou (preempção por quantum), colocar de volta na fila de prontos
        AnsiConsole.Markup($"[mediumpurple1]Quantum expirado para o processo {process.Id} (Fase 2). Reenfileirando para a fila de prontos.[/]\n");
        _readyQueue.Enqueue(process);
    }
}