namespace ProcessScheduler.Models;

public class Process
{
    public int Id { get; set; }
    public int CpuPhase1Duration { get; set; } 
    public int IoPhaseDuration { get; set; } 
    public int CpuPhase2Duration { get; set; }
    public int RamRequired { get; set; }
    
    public int TotalExecutionTime  {get; set; }
    public ProcessState State { get; set; }
    public ProcessPhase Phase { get; set; }
}
