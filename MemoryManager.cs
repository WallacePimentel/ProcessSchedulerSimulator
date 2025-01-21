using ProcessScheduler.Models;

public class MemoryManager
{
    private Ram _ram;

    public MemoryManager(int totalMemory)
    {
        _ram = new Ram { TotalMemory = totalMemory, AvailableMemory = totalMemory };
    }

    public void AllocateMemory(Process process)
    {
        _ram.AvailableMemory -= process.RamRequired;
    }

    public void ReleaseMemory(Process process)
    {
        _ram.AvailableMemory += process.RamRequired;
    }

    public int GetAvailableMemory()
    {
        return _ram.AvailableMemory;
    }
}