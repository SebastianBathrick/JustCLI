using JustCLI.Logging;
using JustCLI.ClientHelpers;
using JustCLI.Core;

namespace JustCLI.Commands
{
    /// <summary>
    /// ICommand that displays help information for commands.
    /// </summary>
    internal class HelpCommand : ICommand
    {
        #region Constants
        public const string NAME = "help";
        private const string SCREEN_TITLE = "[CLI HELP]";
        private const string FLAGS_TITLE = "\tAssociated Flags:";
        private const string COMMAND_TITLE = "\t[COMMAND]:";
        private const string DETAILED_FLAG = "v";
        #endregion

        private List<ICommand> _validCommands = new List<ICommand>();

        public string Name => NAME;

        public string Description => "Displays help information for commands.";

        public Flag[] Flags => [
            new Flag(DETAILED_FLAG, "Provides verbose command list and flag descriptions.", false)
            ];

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
        public void Execute(FlagInputContainer flagEntries)
        {
            Log.Info("{Help}: The following is a list of valid commands:", SCREEN_TITLE);
            LogHelper.LogExtraLine();

            bool isDetailed = flagEntries.IsFlag(DETAILED_FLAG);

            foreach (var command in _validCommands)
            {
                if (isDetailed)
                {
                    Log.Info("{CommandTitle}", COMMAND_TITLE);
                }

                Log.Info("\t{Name}: " + command.Description, ICommand.PREFIX + command.Name);

                if(isDetailed)
                {
                    if (command.Flags.Count() > 0)
                    {
                        LogHelper.LogExtraLine();
                        Log.Info(FLAGS_TITLE);
                        foreach (var flag in command.Flags)
                            Log.Info("\t\t" + flag.ToString(), flag.name);
                    }
                }
            }

            if (!isDetailed)
            {
                LogHelper.LogExtraLine();
                Log.Info("{Note}: View a more detailed list by typing space and {Flag} after the {Help} command.",
                    "Note",
                    Flags[0].name,
                    ICommand.PREFIX + Name
                    );
            }
        }
    
        
    }
}

