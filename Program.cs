using ProcessScheduler.Models;
namespace ProcessScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // Recebe a memória disponível como parâmetro de linha de comando
            if (args.Length == 0 || !int.TryParse(args[0], out int availableMemory))
            {
                Console.WriteLine("Por favor, forneça a memória disponível como um parâmetro (em MB). Usando valor padrão de 1024MB.");
                availableMemory = 1024;
            }

            // Inicializando o gerenciador de memória com a memória disponível passada como argumento
            MemoryManager memoryManager = new MemoryManager(availableMemory);

            // Inicializando as filas de processos
            Queue<Process> newQueue = new Queue<Process>(); 
            Queue<Process> readyQueue = new Queue<Process>(); 

            // Inicializando o gerador de processos
            ProcessGenerator processGenerator = new ProcessGenerator(newQueue);
            
            //Inicializando a fila de bloqueados
            Queue<Process> blockedQueue = new Queue<Process>();

            // Inicializando a lista de Cpus e dispositivo de entrada e saída
            var cpus = new List<Cpu>()
            {
                new Cpu(1, readyQueue, blockedQueue, memoryManager),
                new Cpu(2, readyQueue, blockedQueue, memoryManager),
                new Cpu(3, readyQueue, blockedQueue, memoryManager),
                new Cpu(4, readyQueue, blockedQueue, memoryManager)
            };
            
            IoDevice ioDevice = new IoDevice();
            
            //Inicializando a thread de cada CPU
            Thread cpu1Thread = new Thread(cpus[0].StartExecution);
            Thread cpu2Thread = new Thread(cpus[1].StartExecution);
            Thread cpu3Thread = new Thread(cpus[2].StartExecution);
            Thread cpu4Thread = new Thread(cpus[3].StartExecution);
            cpu1Thread.Start();
            cpu2Thread.Start();
            cpu3Thread.Start();
            cpu4Thread.Start();

            // Inicializando o dispatcher (escalonador de processos)
            Dispatcher dispatcher = new Dispatcher(readyQueue, blockedQueue, newQueue, memoryManager, cpus, ioDevice);

            // Iniciando o thread de geração de processos
            Thread processGenerationThread = new Thread(processGenerator.StartProcessGeneration);
            processGenerationThread.Start();
            Console.WriteLine("Iniciando a geração de processos...");

            // Iniciando o thread do dispatcher para escalonar os processos
            Thread dispatcherThread = new Thread(dispatcher.StartScheduling);
            dispatcherThread.Start();
            Console.WriteLine("Iniciando o escalonamento de processos...");

            // Aguardar os threads terminarem (no caso, eles vão rodar indefinidamente)
            cpu1Thread.Join();
            cpu2Thread.Join();
            cpu3Thread.Join();
            cpu4Thread.Join();
            processGenerationThread.Join();
            dispatcherThread.Join();
        }
    }
}
