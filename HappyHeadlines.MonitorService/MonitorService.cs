using Serilog;
using Serilog.Core;

namespace HappyHeadlines.MonitorService;

public static class MonitorService
{
    public static Logger Log { get; private set; }

    public static void Initialize()
    {
        // Configure the logger
        Log = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

        Log.Information("MonitorService Initialized.");
    }
}