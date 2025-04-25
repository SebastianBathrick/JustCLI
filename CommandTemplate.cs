namespace JustCLI
{
    public class CommandTemplate : ICommand
    {
        public string Name { get; }
        public string Description { get; }
        public Flag[] Flags { get; }
        private readonly Action<FlagEntries?> executeAction;

        public CommandTemplate(
            string name,
            string description,
            Flag[]? flags,
            Action<FlagEntries?> executeAction)
        {
            Name = name;
            Description = description;
            Flags = flags ?? [];
            this.executeAction = executeAction;
        }

        public void Execute(FlagEntries? flagEntries)
        {
            executeAction(flagEntries);
        }
    }
}
