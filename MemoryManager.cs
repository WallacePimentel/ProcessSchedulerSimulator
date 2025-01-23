using ProcessScheduler.Models;

public class MemoryManager
{
    //Referência do RAM total
    private Ram _ram;

    //Construtor Memory Manager
    public MemoryManager(int totalMemory)
    {
        _ram = new Ram { TotalMemory = totalMemory, AvailableMemory = totalMemory };
    }

    //Função para alocar memória para um processo
    public void AllocateMemory(Process process)
    {
        _ram.AvailableMemory -= process.RamRequired;
    }

    //Função para liberar a memória de um processo
    public void ReleaseMemory(Process process)
    {
        _ram.AvailableMemory += process.RamRequired;
    }

    //Função pra saber quanto de memória está disponível
    public int GetAvailableMemory()
    {
        return _ram.AvailableMemory;
    }
}