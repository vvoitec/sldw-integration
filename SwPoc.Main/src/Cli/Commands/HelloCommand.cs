using System;
using Microsoft.Extensions.Logging;
using Serilog;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main.Cli.Commands
{
    public class HelloCommand : ICommand
    {
        private readonly ILogger<HelloCommand> _log;
        private const string Name = "hello";

        public HelloCommand(ILogger<HelloCommand> log)
        {
            _log = log;
        }
        
        public string GetName()
        {
            return Name;
        }

        public bool Handle()
        {
            _log.LogInformation("Hello from command!");

            return true;
        }
    }
}