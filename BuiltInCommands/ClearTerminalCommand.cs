namespace JustCLI.BuiltInCommands
{
    internal class ClearTerminalCommand : ICommand
    {
        public string Name => "clear";
        public string Description => "Clears the terminal of text.";
        public Flag[] Flags => Array.Empty<Flag>();

        /// <summary>
        /// Clears the terminal screen.
        /// </summary>
        public void Execute(FlagEntries? flagEntries)
        {
            Console.Clear();
        }
    }
}
