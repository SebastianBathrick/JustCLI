using JustCLI.Utilities;
using Serilog;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays help information for commands.
    /// </summary>
    internal class HelpCommand : ICommand
    {
        #region Constants
        private const string SCREEN_TITLE = "[CLI HELP]";
        private const string FLAGS_TITLE = "\tAssociated Flags:";
        private const string COMMAND_TITLE = "\t[COMMAND]:";
        private const string DETAILED_FLAG = "v";
        #endregion

        private List<ICommand> _validCommands = new List<ICommand>();

        public string Name => "help";

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


        /// <summary> 
        /// Displays a list of valid commands, and optionally their flags, using Serialog instance.
        /// </summary>
        public void Execute(FlagInputContainer flagEntries)
        {
            Log.Information("{Help}: The following is a list of valid commands:", SCREEN_TITLE);
            LogHelper.LogExtraLine();

            bool isDetailed = flagEntries.IsFlag(DETAILED_FLAG);

            foreach (var command in _validCommands)
            {
                if (isDetailed)
                {
                    Log.Information("{CommandTitle}", COMMAND_TITLE);
                }

                Log.Information("\t{Name}: " + command.Description, ICommand.PREFIX + command.Name);

                if(isDetailed)
                {
                    if (command.Flags.Count() > 0)
                    {
                        LogHelper.LogExtraLine();
                        Log.Information(FLAGS_TITLE);
                        foreach (var flag in command.Flags)
                            Log.Information("\t" + flag.ToString(), flag.name);
                    }

                    LogHelper.LogExtraLine();
                }         
            }

            if (!isDetailed)
            {
                LogHelper.LogExtraLine();
                Log.Information("{Note}: View a more detailed list by typing space and {Flag} after the {Help} command.",
                    "Note",
                    Flags[0].name,
                    ICommand.PREFIX + Name
                    );
            }
        }
    }
}

