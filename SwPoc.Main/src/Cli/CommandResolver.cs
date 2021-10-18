using System.Collections.Generic;
using System.Linq;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main.Cli
{
    public class CommandResolver : ICommandResolver
    {
        private readonly Dictionary<string, ICommand> _commandsMapping;

        public CommandResolver(IEnumerable<ICommand> commands)
        {
            _commandsMapping = commands.ToDictionary(command => command.GetName());
        }

        public ICommand Resolve(string name)
        {
            return _commandsMapping[name];
        }
    }
}