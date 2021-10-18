using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main
{
    public class AppRunner : IAppRunner
    {
        private readonly ILogger<AppRunner> _log;
        private readonly IConfiguration _config;
        private readonly ICommandResolver _commandResolver;

        public AppRunner(ILogger<AppRunner> log, IConfiguration config, ICommandResolver commandResolver)
        {
            _log = log;
            _config = config;
            _commandResolver = commandResolver;
        }
        
        public void Run()
        {
            var testMessage = _config.GetValue<string>("App:TestMessage");
            
            _log.LogInformation("Test message is: {TestMessage}" ,testMessage);
            
            while (true)
            {
                try
                {
                    if (_commandResolver.Resolve(Console.ReadLine()).Handle())
                    {
                        continue;
                    }

                    _log.LogInformation("Shutting down");

                    break;
                }
                catch (Exception e)
                {
                    _log.LogError("Error while processing command \n {E}", e);
                }
            }
        }
    }
}