using CqrsExample.Api.Configurations;
using Serilog;
using Serilog.Templates.Themes;
using SerilogTracing;
using SerilogTracing.Expressions;

namespace CqrsExample.Api;

public static class Program
{
    public static int Main(string[] args)
    {
        var config = LoadConfigs();
        SetupSerilog(config);
        var listener = SetupActivityListener();

        try
        {
            Log.Information("Starting web host");
            BuildApp(args, config).Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
            listener.Dispose();
        }
    }

    private static IConfiguration LoadConfigs() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

    private static void SetupSerilog(IConfiguration configuration) =>
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name ?? "CqrsExample.Api")
            .WriteTo.Console(Formatters.CreateConsoleTextFormatter(theme: TemplateTheme.Code))
            .CreateLogger();

    private static IDisposable SetupActivityListener() =>
        new ActivityListenerConfiguration()
            .Instrument.AspNetCoreRequests()
            .TraceToSharedLogger();

    private static WebApplication BuildApp(string[] args, IConfiguration config)
    {
        var webAppBuilder = WebApplication.CreateBuilder(args);

        webAppBuilder.Host.UseSerilog();
        webAppBuilder.Services.ConfigureServices(config);

        var webApp = webAppBuilder.Build();
        webApp.MapAllEndpoints();

        return webApp;
    }

    private static void ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureJsonOptions();
        services.AddSerilogLogger();
        services.AddOpenApiConfiguration();
        services.AddDependencies(config);
#if !PRODUCTION
        services.AddEndpointsApiExplorer();
#endif
    }
}