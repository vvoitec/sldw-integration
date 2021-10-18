using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SwPoc.Main.Cli;
using SwPoc.Main.Cli.Commands;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            Log.Logger.Information("App Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IAppRunner, AppRunner>();
                    services.AddTransient<ICommand, HelloCommand>();
                    services.AddTransient<ICommand, ExitCommand>();
                    services.AddTransient<ICommand, RunCommand>();
                    services.AddTransient<ICommandResolver, CommandResolver>();
                })
                .UseSerilog()
                .Build();

            var runner = ActivatorUtilities.CreateInstance<AppRunner>(host.Services);
            runner.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    true, true)
                .AddEnvironmentVariables();
        }
    }
}