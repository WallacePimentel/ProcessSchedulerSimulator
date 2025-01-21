using ProcessScheduler.Models;

public class ProcessGenerator
{
    private Queue<Process> _newQueue;
    private int _processId; 
    private Random _random;

    public ProcessGenerator(Queue<Process> newQueue)
    {
        _newQueue = newQueue;
        _processId = 1;
        _random = new Random();
    }

    // Thread de geração de processos
    public void StartProcessGeneration()
    {
        while (true)
        {
            Thread.Sleep(1000);
            CreateProcess();
        }
    }

    // Criação de um novo processo
    private void CreateProcess()
    {
        int cpuPhase1Duration = _random.Next(1, 11) * 2;
        int ioPhaseDuration = _random.Next(1, 6) * 2;
        int cpuPhase2Duration = _random.Next(1, 11) * 2; 
        int ramRequired = _random.Next(10, 100);
        int totalExecutionTime = _random.Next(1, 10); 

        Process process = new Process
        {
            Id = _processId++,
            CpuPhase1Duration = cpuPhase1Duration,
            IoPhaseDuration = ioPhaseDuration,
            CpuPhase2Duration = cpuPhase2Duration,
            RamRequired = ramRequired,
            State = ProcessState.New,
            TotalExecutionTime = totalExecutionTime
        };

        Console.WriteLine($"Novo processo {process.Id} criado e aguardando alocação de recursos.");

        _newQueue.Enqueue(process);
    }
}