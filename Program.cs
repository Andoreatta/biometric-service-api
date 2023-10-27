using BiometricService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using CliWrap;

const string ServiceName = "Fingertech Biometric API";

if (args is { Length: 1 })
{
    try
    {
        string executablePath =
            Path.Combine(AppContext.BaseDirectory, "BiometricService.exe");

        if (args[0] is "/Install")
        {
            await Cli.Wrap("sc")
                .WithArguments(new[] { "create", ServiceName, $"binPath={executablePath}", "start=auto" })
                .ExecuteAsync();
        }
        else if (args[0] is "/Uninstall")
        {
            await Cli.Wrap("sc")
                .WithArguments(new[] { "stop", ServiceName })
                .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(new[] { "delete", ServiceName })
                .ExecuteAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWindowsService();
LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
builder.Services.AddScoped<Biometric>();
builder.Services.AddSingleton<APIService>();
builder.Services.AddHostedService<APIService>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.
        AllowAnyOrigin().
        AllowAnyHeader().
        AllowAnyMethod();
    });
});

var serviceApp = builder.Build();

serviceApp.UseRouting();
serviceApp.UseCors();
serviceApp.MapControllers();

serviceApp.Run();