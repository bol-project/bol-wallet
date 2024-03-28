using System.Diagnostics;
using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Templates;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingExtensions
{
    private const long MaxFileSizeLimitBytes = 1048576L;

    public static IServiceCollection ConfigureSerilog(this IServiceCollection services)
    {
        var logDirectory = Path.Combine(FileSystem.AppDataDirectory,
            "Logs",
            AppInfo.Current.Name,
            AppInfo.Current.VersionString);

        var filePath = Path.Combine(logDirectory, "bolwallet_.log"); // This will create bolwallet_yyyyMMdd.log

        var outputTemplate =
            new ExpressionTemplate(
                "[{@t:yyyy-MM-dd HH:mm:ss.fff zzz}] [{@l:u3}] {#if SourceContext is not null}[{SourceContext}] {#end}{@m:lj}\n{@x}");

        var configuration = new LoggerConfiguration();
        configuration = Debugger.IsAttached
            ? configuration.MinimumLevel.Verbose().WriteTo.Debug()
            : configuration.MinimumLevel.Information();
        
        Log.Logger = configuration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                outputTemplate,
                filePath,
                encoding: Encoding.UTF8,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                fileSizeLimitBytes: MaxFileSizeLimitBytes,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .WriteTo.Console(outputTemplate)
            .CreateLogger();

        var deviceInfo = new
        {
            DeviceInfo.Current.Name,
            DeviceInfo.Current.DeviceType,
            DeviceInfo.Current.Manufacturer,
            DeviceInfo.Current.Model,
            Idiom = DeviceInfo.Current.Idiom.ToString(),
            Platform = DeviceInfo.Current.Platform.ToString(),
            DeviceInfo.Current.VersionString
        };
        var appInfo = new
        {
            AppInfo.Current.Name,
            AppInfo.Current.VersionString,
            AppInfo.Current.BuildString,
            AppInfo.Current.PackageName,
            AppInfo.Current.PackagingModel,
            AppInfo.Current.RequestedTheme,
            AppInfo.Current.RequestedLayoutDirection
        };
        Log.Logger.Information("Starting BolWallet application...");
        Log.Logger.Information("Device Info: {@DeviceInfo}", deviceInfo);
        Log.Logger.Information("App Info: {@AppInfo}", appInfo);

        services.AddLogging(configure => configure.AddSerilog(dispose: true));

        return services;
    }
}
