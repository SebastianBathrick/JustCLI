using JustCLI.BuiltInCommands;
using JustCLI.Utilities;
using Serilog;

namespace JustCLI
{
    /// <summary>
    /// Singleton class that handles command line arguments and executes commands.
    /// </summary>
    public class CommandLineApp
    {
        #region Variables
        private static CommandLineApp _instance = new CommandLineApp();

        // Built-In Commands
        private HelpCommand _helpCommand;
        private VersionCommand _versionCommand;
        private CLIVersionCommand _cliVersionCommand;

        private ICommand? _defaultCommand;
        private ArgContainer _argContainer;
        private Dictionary<string, ICommand> _commandDict;
        #endregion

        #region Setup Methods
        private CommandLineApp()
        {
            _instance = this;

            // Get the user's arguments from the command line (excluding the program name)
            _argContainer = new ArgContainer(Environment.GetCommandLineArgs());
            _argContainer.Get(); // Skip the first argument (the program path)
            _commandDict = new Dictionary<string, ICommand>();

            // Add built-in commands
            AddCommands([
                _helpCommand = new HelpCommand(),
                _versionCommand = new VersionCommand(),
                _cliVersionCommand = new CLIVersionCommand(),
                new ClearTerminalCommand()
                ]);

            // Instantiate Serilog logger
            CreateLogger();
        }
        #endregion

        #region Public Static Interface
        /// <summary> 
        /// Called by client to start reading commands (no commands should be added after calling.)
        /// </summary>
        /// <param name="requireCommand">Whether an error will be printed if there is no args. </param>
        /// <param name="isPromptBeforeExit">Asks the user if they want to enter more arguments 
        /// before exiting.</param>
        /// <param name="argOverride">If not null overrides initial args provided to the program.</param>
        public static void Start(
            bool requireCommand = false,
            bool isPromptBeforeExit = false,
            string[]? argOverride = null
            )
        {
            // Start just serves as a wrapper and StartInstance does as described by the docs
            _instance.StartInstance(
                requireCommand: requireCommand,
                isPromptBeforeExit: isPromptBeforeExit,
                argOverride: argOverride
                );
        }

        /// <summary> Assigns command to execute if no args are provided during the Start() method. </summary>
        public static void AddDefaultCommand(ICommand command)
        {
            if (!command.HasNoRequiredFlags())
            {
                Log.Error("Default command {Command} cannot have required flags.", command.Name);
                return;
            }

            _instance._defaultCommand = command;
            AddCommands([command]);
        }

        /// <summary> Adds command to singleton instance. Should be called only before Start! </summary>
        public static void AddCommands(ICommand[] commands)
        {
            foreach (var command in commands)
                if (!IsCommandAlreadyAdded(command))
                    _instance._commandDict.Add(ICommand.PREFIX + command.Name, command);
        }

        /// <summary> 
        /// Sets the version of the CLI application that will be shown when using
        /// the built-in version command
        /// </summary>
        public static void SetVersion(string version) => _instance._versionCommand.SetVersion(version);
        #endregion

        #region Command Parsing & Execution
        private void StartInstance(
            bool requireCommand = false,
            bool isPromptBeforeExit = false,
            string[]? argOverride = null
            )
        {
            // Add the client's and the built-in commands to the help command list.
            PopulateHelpList();

            // If the client provided args to override the user's args then cache the client's
            // override args and discard the user's args (if any)
            if (argOverride != null)
                _argContainer = new ArgContainer(argOverride);

            bool isFirstPass = true;

            do
            {
                // If the user chose to enter more arguments after the initial iteration
                if (!isFirstPass)
                {
                    // Get the arguments through a prompt asking for the user to enter a string
                    var newArgs = CLIHelpers.GetStringFromUser("additional arguments").
                        Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    _argContainer = new ArgContainer(newArgs);
                }

                if (_argContainer.IsEmpty)
                {
                    if (_defaultCommand != null)
                        _defaultCommand.Execute(null);
                    else
                    {
                        Log.Error("No arguments were provided.");
                        LogHelpDirections();

                        // Forcefully exit to avoid unintended behavior in client's program.
                        if (requireCommand)
                            return;
                    }
                }
                else
                    // Otherwise, parse and execute the provided commands
                    ParseCommands();

                // Enables the next pass to request args as input from user
                isFirstPass = false;
            }
            while (
            isPromptBeforeExit &&
            CLIHelpers.YesNoPrompt("Do you want to enter more arguments?",
            isDefault: true, defaultTo: false) // Default to no if field left blank.
            );
        }

        /// <summary> Parses each argument and executes commands in the order the user provided. </summary>
        private void ParseCommands()
        {
            do
            {
                var arg = _argContainer.Get();

                if (!arg.StartsWith(ICommand.PREFIX))
                {
                    Log.Error("Expected command starting with {Prefix} but got {Arg}.", ICommand.PREFIX, arg);
                    LogHelpDirections();
                    return;
                }
                else if (!_commandDict.ContainsKey(arg))
                {

                    Log.Error("Command {Command} is not a valid command.", arg);
                    LogHelpDirections();
                    return;
                }

                var command = _commandDict[arg];

                if (!TryGetCommandFlags(command, out var flagsAndVals))
                    return; // Message displayed by TryGetCommandFlags in each false condition.

                try
                {
                    command.Execute(flagsAndVals);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occurred while executing the command {Command}.", command.Name);
                }
            }
            while (!_argContainer.IsEmpty);
        }
        #endregion

        #region Flag Methods
        /// <summary> Gets any userFlagsAndValues trailing the current command and before the next command. </summary>
        /// <returns> Returns true if all flags and values (or lack-thereof) have been collected successfully. </returns>
        private bool TryGetCommandFlags(ICommand command, out FlagEntries flagEntries)
        {
            var possibleFlags = GetPossibleFlags();
            var validFlags = command.Flags;

            // Flags found + any values associated (if there are any of either)
            flagEntries = new FlagEntries();

            for (int i = 0; i < possibleFlags.Count; i++)
            {
                if (!TryParseFlag(possibleFlags[i], command, out var flag) || flag == null)
                    return false; // Messages in prior method

                // Switch type to non-nullable flag
                var verifiedFlag = (Flag)flag;
                string? flagValue = null;

                // Check if the flag needs a value to follow it, and if so, cache it
                if (verifiedFlag.isValueRequired)
                    // Check if the next argument is a value
                    if (i + 1 < possibleFlags.Count && !possibleFlags[i + 1].StartsWith(Flag.PREFIX))
                    {
                        flagValue = possibleFlags[i + 1];
                        i++; // Skip the value in the next iteration
                    }
                    else
                    {
                        Log.Error("Flag {Flag} requires a value but none was provided.", verifiedFlag.name);
                        LogHelpDirections();
                        return false;
                    }

                flagEntries.AddFlag(verifiedFlag, flagValue);
            }

            // Check if all required flags were provided
            foreach (var flag in command.GetRequiredFlags())
                if (!flagEntries.IsFlag(flag.name))
                {
                    Log.Error("Flag {Flag} is required but was not provided.", flag.name);
                    LogHelpDirections();
                    return false;
                }

            // All flags and values were collected successfully
            return true;
        }

        /// <summary> 
        /// Retreives each arg following the current command until there are no more 
        /// or another command is found.
        /// </summary>
        private List<string> GetPossibleFlags()
        {
            var possibleFlags = new List<string>();

            while (!_argContainer.IsEmpty && !_argContainer.Peek().StartsWith(ICommand.PREFIX))
                possibleFlags.Add(_argContainer.Get());

            return possibleFlags;
        }

        /// <summary> Ensures the argument is a flag and a valid one for the given command. </summary>
        private bool TryParseFlag(string postCommandArg, ICommand command, out Flag? flag)
        {
            flag = null;

            if (!postCommandArg.StartsWith(Flag.PREFIX))
            {
                Log.Error("Expected flag starting with {Prefix} but got {Flag}.", Flag.PREFIX, postCommandArg);
                LogHelpDirections();
                return false;
            }

            if (!command.TryGetFlag(postCommandArg, out var validFlag))
            {
                Log.Error("Flag {Flag} is not a valid for {Command}.", postCommandArg, command.Name);
                LogHelpDirections();
                return false;
            }

            flag = validFlag;
            return true;
        }
        #endregion

        #region Helper Methods
        private void LogHelpDirections()
        {
            string helpFullName = ICommand.PREFIX + _helpCommand.Name;

            Log.Information("Use {Help} to display all valid commands. Optionally follow with the {Detailed} flag to see valid flags.",
                helpFullName, _helpCommand.Flags[0].name);
        }

        /// <summary> Populates the help command with all client and built-in commands </summary>
        private void PopulateHelpList()
        {
            // Add all built-in and client commands to the help command list.
            foreach (var command in _commandDict.Values)
                if (command is not CLIVersionCommand)
                    _helpCommand.AddCommand(command);

            // Add the CLI version after the client's commands for ease of use
            _helpCommand.AddCommand(_cliVersionCommand);
        }

        private static bool IsCommandAlreadyAdded(ICommand command) =>
            _instance._commandDict.ContainsKey(ICommand.PREFIX + command.Name);

        public void CreateLogger(ILogger? logger = null)
        {
            if (logger == null)
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            else
                Log.Logger = logger;
        }
        #endregion
    }
}

