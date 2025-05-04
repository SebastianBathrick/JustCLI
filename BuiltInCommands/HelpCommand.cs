using JustCLI.ClientHelpers;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays help information for commands.
    /// </summary>
    internal class HelpCommand : ICommand
    {
        #region Constants
        public const string NAME = "help";
        private const string VERBOSE_FLAG = "v";
        #endregion

        #region Properties
        private List<ICommand> _validCommands = new List<ICommand>();

        public string Name => NAME;

        public string Description => "Displays help information for commands.";

        public Flag[] Flags => [
            new Flag(VERBOSE_FLAG, "Provides verbose command list and flag descriptions.", false)
            ];
        #endregion

        /// <summary> 
        /// Adds a valid command to list when executed. Needs to be called in CLI's constructor. 
        /// </summary>
        public void AddCommand(ICommand command)
        {
            if (!_validCommands.Contains(command))
                _validCommands.Add(command);
        }

        public void RemoveCommand(ICommand command)
        {
            var removeCommands = new List<ICommand>();

            foreach (var validCommand in _validCommands)
                if (validCommand == command)
                    removeCommands.Add(validCommand);

            foreach(var removeCommand in removeCommands)
                _validCommands.Remove(removeCommand);
        }

        /// <summary> 
        /// Displays a list of valid commands, and optionally their flags, using Serialog instance.
        /// </summary>
        public void Execute(FlagContainer flagEntries)
        {
            bool isDetailed = flagEntries.Contains(VERBOSE_FLAG);
            string header;

            if (!isDetailed)
                header = "[Help]: The following is a list of valid commands:";
            else
                header = "[Verbose Help]: The following is a list of valid commands and their flags:";

            LogHelper.LogWithMargin("{Help}", header);

            foreach (var command in _validCommands)
            {
                Log.Info("{Name}: " + command.Description, ICommand.PREFIX + command.Name);

                if(isDetailed && command.Flags.Count() > 0)
                { 
                    LogHelper.LogExtraLine();

                    foreach (var flag in command.Flags)
                        Log.Info($"\t" + flag.ToString(), flag.name);

                    LogHelper.LogExtraLine();
                }
                
            }

            if (!isDetailed)
                LogHelper.LogWithMargin(
                    "{Note}: View a more detailed list by typing space and {Flag} after the {Help} command.",
                    "Note",
                    Flags[0].name,
                    ICommand.PREFIX + Name
                    );
        }      
    }
}

