namespace SwPoc.Main.Cli.Interfaces
{
    public interface ICommand
    {
        public string GetName();

        public bool Handle();
    }
}