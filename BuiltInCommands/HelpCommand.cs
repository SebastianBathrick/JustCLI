using JustCLI.Utilities;
using Serilog;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// Command that displays help information for commands.
    /// </summary>
    internal class HelpCommand : ICommand
    {
        #region Constants
        private const string SCREEN_TITLE = "[CLI HELP]";
        private const string FLAGS_TITLE = "[ASSOCIATED FLAGS]";
        private const string COMMAND_TITLE = "[COMMAND]";
        private const string DETAILED_FLAG = "detailed";
        #endregion

        private Flag[] _flags = [
            new Flag(DETAILED_FLAG, "Provides detailed command and flag descriptions.", false)
        ];

        private List<ICommand> _validCommands = new List<ICommand>();
        
        public string Name => "help";

        public string Description => "Displays help information for commands.";

        public Flag[] Flags => _flags;

        /// <summary> 
        /// Adds a valid command to list when executed. Needs to be called in CommandLineApp's constructor. 
        /// </summary>
        public void AddCommand(ICommand command)
        {
            if(!_validCommands.Contains(command))
                _validCommands.Add(command);
        }
            

        /// <summary> 
        /// Displays a list of valid commands, and optionally their flags, using Serialog instance.
        /// </summary>
        public void Execute(FlagEntries? flagEntries)
        {
            Log.Information("{Help}: The following is a list of valid commands:", SCREEN_TITLE);
            bool isDetailed = flagEntries.IsFlag(DETAILED_FLAG);

            foreach (var command in _validCommands)
            {
                if (isDetailed)
                {
                    Log.Information("");
                    Log.Information("{CommandTitle}", COMMAND_TITLE);
                }

                Log.Information("\t{Name}: " + command.Description, ICommand.PREFIX + command.Name);

                if (isDetailed && command.Flags.Count() > 0)
                {
                    Log.Information("");
                    Log.Information("{FlagsTitle}", FLAGS_TITLE);
                    foreach (var flag in command.Flags)
                        Log.Information("\t" + flag.ToString(), flag.name);
                }
            }

            Log.Information("");
        }
    }
}

