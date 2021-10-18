namespace SwPoc.Main.Cli.Interfaces
{
    public interface ICommandResolver
    {
        public ICommand Resolve(string name);
    }
}