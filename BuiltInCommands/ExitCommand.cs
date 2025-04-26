namespace JustCLI.BuiltInCommands
{
    internal class ExitCommand : ICommand
    {
        public string Name => "exit";

        public string Description => "Exits the application.";

        public void Execute(FlagInputContainer flagEntries) => CLI.SetExiting(true);
    }
}
