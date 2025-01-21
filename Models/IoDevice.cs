namespace ProcessScheduler.Models;

public class IoDevice
{
    public int Id { get; set; }
    public bool IsInUse { get; set; }

    public IoDevice()
    {
        IsInUse = false;
    }
}