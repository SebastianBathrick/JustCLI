using JustCLI.Utilities;

namespace JustCLI.BuiltInCommands
{
    internal class ExitCommand : ICommand
    {
        public const string NAME = "exit";

        public string Name => NAME;

        public string Description => "Exits the application.";

        public void Execute(FlagInputContainer flagEntries) => CLI.SetExiting(true);
    }
}
