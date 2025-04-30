using JustCLI.Commands;
using JustCLI.Core;

namespace JustCLI
{
    public class ActionCommand : ICommand
    {
        public string Name { get; }
        public string Description { get; }
        public int MinFlagCount { get; } = ICommand.DEFAULT_MIN_FLAG_COUNT;
        public Flag[] Flags { get; }

        private readonly Action<FlagInputContainer> _executeAction;

        public ActionCommand(string name, string description, Action<FlagInputContainer> executeAction, Flag[]? flags = null, int minFlagCount = ICommand.DEFAULT_MIN_FLAG_COUNT)
        {
            Name = name;
            Description = description;
            _executeAction = executeAction;
            Flags = flags ?? Array.Empty<Flag>();
            MinFlagCount = minFlagCount;
        }

        public void Execute(FlagInputContainer flagEntries)
        {
            _executeAction(flagEntries);
        }
    }
}
