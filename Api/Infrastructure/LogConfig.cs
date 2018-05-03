using System.Configuration;
using System.IO;
using System.Text;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;
using SerilogWeb.Classic;
using SerilogWeb.Classic.Enrichers;

namespace Api.Infrastructure
{
    public class LogConfig
    {
        public static void ConfigureLogging()
        {
            const int retainedFileCountLimit = 31;

            var logDirectory = ConfigurationManager.AppSettings["Global.ApplicationLogsFolder"];
            var applicationName = ConfigurationManager.AppSettings["Global.ApplicationName"];
            var applicationVersion = ConfigurationManager.AppSettings["Global.ApplicationVersion"];
            var logFileNameFormat = $"{applicationName}-{{Date}}.log";
            var pathFormat = Path.Combine(logDirectory, logFileNameFormat);

            ApplicationLifecycleModule.IsEnabled = false; // Disable logging of all requests

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentUserName()
                .Enrich.With<HttpRequestUrlEnricher>()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("ApplicationVersion", applicationVersion)
                .Enrich.WithProperty("Environment", ConfigurationManager.AppSettings.Get("Global.Environment.Name"))
                .WriteTo.Sink(new RollingFileSink(pathFormat, new JsonFormatter(renderMessage: true), null, retainedFileCountLimit, Encoding.UTF8))
                .CreateLogger();
        }
    }
}