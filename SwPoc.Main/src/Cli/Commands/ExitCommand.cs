using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main.Cli.Commands
{
    public class ExitCommand : ICommand
    {
        public string GetName()
        {
            return "exit";
        }

        public bool Handle()
        {
            return false;
        }
    }
}