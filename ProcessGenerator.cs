using ProcessScheduler.Models;

public class ProcessGenerator
{

    //Refêrencia da fila de prontos
    private Queue<Process> _newQueue;

    //Id do processo
    private int _processId; 

    //Atributo random para obtenção de valores aleatórios
    private Random _random;

    //Construtor do processo
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
        //Escolhendo valores aleatórios para cada atributo do processo, simulando a ignorância do usuário sob o sistema operacional
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

        //Adicionando o processo novo na fila de novos
        _newQueue.Enqueue(process);
    }
}