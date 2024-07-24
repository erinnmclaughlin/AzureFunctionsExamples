using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionsApp.Functions;

public sealed class TimedFunction(ILogger<TimedFunction> logger)
{
    private const string Cron = "0 * * * * *"; // every minute
    
    [Function(nameof(TimedFunction))]
    public void Run([TimerTrigger(Cron, RunOnStartup = true)] TimerInfo timerInfo)
    {
        logger.LogInformation("Hello!");
    }
}
