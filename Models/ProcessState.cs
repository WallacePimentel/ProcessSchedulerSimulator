namespace ProcessScheduler.Models;

public enum ProcessState
{
    New,
    Ready,
    Running,
    Blocked,
    Finished
}