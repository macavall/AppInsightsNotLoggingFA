using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
        .ConfigureFunctionsWebApplication()
        .ConfigureServices(services =>
        {
            // Add Application Insights telemetry for worker service
            services.AddApplicationInsightsTelemetryWorkerService();

            // Disable adaptive sampling in Application Insights
            services.Configure<TelemetryConfiguration>((config) =>
            {
                // Find the AdaptiveSamplingTelemetryProcessor and disable sampling by setting percentage to 100%
                var adaptiveSamplingProcessor = config.DefaultTelemetrySink.TelemetryProcessors
                    .OfType<AdaptiveSamplingTelemetryProcessor>()
                    .FirstOrDefault();

                if (adaptiveSamplingProcessor != null)
                {
                    adaptiveSamplingProcessor.MinSamplingPercentage = 100;
                }
            });
        })
        .Build();

        host.Run();
    }
}