using SebastianBathrick.JustCLI.Utilities;
using Serilog;

namespace SebastianBathrick.JustCLI.BuiltInCommands
{
    public class HelpCommand : ICommand
    {
        #region Constants & Variables
        public const string SCREEN_TITLE = "[CLI HELP]";
        public const string FLAGS_TITLE = "[ASSOCIATED FLAGS]";
        public const string COMMAND_TITLE = "[COMMAND]";

        private Flag[] _flags = [
            new Flag("detailed", "Provides detailed command and flag descriptions.", false)
        ];

        private List<ICommand> _validCommands = new List<ICommand>();
        #endregion

        #region Properties
        public string Name => "help";

        public string Description => "Displays help information for commands.";

        public Flag[] Flags => _flags;
        #endregion

        /// <summary> 
        /// Adds a valid command to list when executed. Needs to be called in CommandLineEnviro's constructor. 
        /// </summary>
        public void AddCommand(ICommand command) =>
            _validCommands.Add(command);

        /// <summary> 
        /// Displays a list of valid commands, and optionally their flags, using Serialog instance.
        /// </summary>
        public bool Execute(List<(Flag flag, string? value)> usedFlagsAndValues)
        {
            try
            {
                Log.Information("{Help}: The following is a list of valid commands:", SCREEN_TITLE);
                bool isDetailed = CommandUtilities.IsFlagUsed(_flags[0].name, usedFlagsAndValues);

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
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred while executing the help command.");
                return false;
            }
        }
    }
}

