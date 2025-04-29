using JustCLI.BuiltInCommands;
using JustCLI.Utilities;
using Serilog;
using Serilog.Core;

namespace JustCLI
{
    /// <summary>
    /// Singleton class that handles command line arguments and executes commands.
    /// </summary>
    public class CLI
    {
        #region Variables
        private static CLI _instance = new CLI();
        private static bool _isDiffLogger = false;

        // Built-In Commands
        private HelpCommand? _helpCommand;
        private VersionCommand? _versionCommand;
        private ArgContainer _argContainer;
        private Dictionary<string, ICommand> _commandDict;

        private ICommand? _defaultCommand;
        private ICommand[] _builtInCommands;

        private bool _isExiting = false;
        
        #endregion

        #region Setup Methods
        private CLI()
        {
            _instance = this;

            // Get the user's arguments from the command line (excluding the program name)
            _argContainer = new ArgContainer(Environment.GetCommandLineArgs());
            _argContainer.Get(); // Skip the first argument (the program path)
            _commandDict = new Dictionary<string, ICommand>();
            _builtInCommands = [
                _helpCommand = new HelpCommand(),
                _versionCommand = new VersionCommand(),
                new ClearTerminalCommand(),
                new ExitCommand(),
                ];

            AddCommands(_builtInCommands);

            if(!_isDiffLogger)
                // Instantiate Serilog logger
                CreateLogger();
        }
        #endregion

        #region Command Parsing & Execution
        private void StartInstance(
            bool requireCommand,
            bool allowMoreCommands,
            string[]? argOverride,
            bool useBuiltInCommands
            )
        {
            if (!useBuiltInCommands)
                foreach (var command in _commandDict.Values)
                    RemoveCommand(command.Name);

            if (_commandDict.Count == 0)
            {
                Log.Error("No commands are defined for the command line interface.");
                return;
            }

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
                    // GetValue the arguments through a prompt asking for the user to enter a string
                    var newArgs = PrimitiveIOHelper.GetStringFromUser("additional arguments").
                        Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    _argContainer = new ArgContainer(newArgs);
                }

                if (_argContainer.IsEmpty)
                {
                    if (_defaultCommand != null)
                        _defaultCommand.Execute(FlagInputContainer.Empty);
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

                if (!allowMoreCommands)
                    _isExiting = true;
            }
            while (!_isExiting);

            // Reset in case call is made again.
            _isExiting = false; 
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

        #region Public Static Interface
        /// <summary> 
        /// Called by client to start reading commands (no commands should be added 
        /// after calling.)
        /// </summary>
        /// <param name="requireCommand">Whether an error will be printed and the CLI
        /// will exit if there are no args.</param>
        /// <param name="allowMoreCommands">Whether the user is able to enter more 
        /// commands after environment args.</param>
        /// <param name="argOverride">If not null overrides initial args provided 
        /// to the program.</param>
        /// <param name="useBuiltInCommands">Whether or not built-in commands will 
        /// be included in the interface</param>
        public static void Start(
            bool requireCommand = false,
            bool allowMoreCommands = false,
            string[]? argOverride = null,
            bool useBuiltInCommands = true
            )
        {
            // Start just serves as a wrapper and StartInstance does as described by the docs
            _instance.StartInstance(requireCommand, allowMoreCommands, argOverride, useBuiltInCommands);
        }

        /// <summary> Removes previously added or built-in command. </summary>
        /// <param name="commandName">Name of command to remove with no prefix.</param>
        public static void RemoveCommand(string commandName)
        {
            string prefixedName = ICommand.PREFIX + commandName;
            var cli = _instance;

            if (!cli._commandDict.TryGetValue(prefixedName, out var command))
            {
                Log.Warning("Attempted to remove command {Name} but it does not exist.", commandName);
                return;
            }

            cli._commandDict.Remove(prefixedName);

            if (command == cli._versionCommand)
                cli._versionCommand = null;
            else if (command == cli._helpCommand)
                cli._helpCommand = null;

            if (cli._helpCommand != null)
                cli._helpCommand.RemoveCommand(command);
        }
            

        /// <summary> Removes all client and optionally built-in commands (including the default.) </summary>
        public static void ClearCommands(bool excludeBuiltIns = true)
        {
            var keysToRemove = new List<string>();

            foreach (var pair in _instance._commandDict)
            {
                if (excludeBuiltIns && _instance._builtInCommands.Contains(pair.Value))
                    continue;
                keysToRemove.Add(pair.Key);
            }

            foreach (var key in keysToRemove)
                _instance._commandDict.Remove(key);
        }

        /// <summary> Adds all built in commands to CLI instance. </summary>

        public static void SetExiting(bool isExiting=true)
        {
            _instance._isExiting = isExiting;
            if (isExiting)
                Log.Information("Exiting CLI.");             
        }

        /// <summary> Assigns command to execute if no args are provided during the Start() method. </summary>
        public static void AddDefaultCommand(ICommand command)
        {
            if (!command.HasNoRequiredFlags())
            {
                Log.Error("Default command {ICommand} cannot have required flags.", command.Name);
                return;
            }

            _instance._defaultCommand = command;
            AddCommands([command]);
        }

        /// <summary> Adds command(s) to singleton instance. Should be called only before Start! </summary>
        public static void AddCommands(ICommand[] commands)
        {
            foreach (var command in commands)
                AddCommand(command);    
        }

        /// <summary> Adds command to singleton instance. Should be called only before Start! </summary>
        public static void AddCommand(ICommand command)
        {
            if (!IsCommandAlreadyAdded(command))
            {
                if (command.MinFlagCount > command.Flags.Length)
                    throw new Exception($"Command {command.Name} has more required flags than available. " +
                        $"MinFlagCount: {command.MinFlagCount}, Flags: {command.Flags.Length}");
                else
                    _instance._commandDict.Add(ICommand.PREFIX + command.Name, command);
            }
        }

        /// <summary> 
        /// Sets the version of the CLI application that will be shown when using
        /// the built-in version command
        /// </summary>
        public static void SetVersion(string version)
        {
            if (_instance._versionCommand == null)
            {
                Log.Warning("Attempted to define version despite the {Command} being removed.", VersionCommand.NAME);
                return;
            }

            _instance._versionCommand.SetVersion(version);
        }
        
        public static void DefineLogger(Logger logger) =>
            Log.Logger = logger;
        #endregion

        #region Flag Methods
        /// <summary> Gets any userFlagsAndValues trailing the current command and before the next command. </summary>
        /// <returns> Returns true if all flags and values (or lack-thereof) have been collected successfully. </returns>
        private bool TryGetCommandFlags(ICommand command, out FlagInputContainer flagEntries)
        {
            var possibleFlags = GetPossibleFlags();
            var validFlags = command.Flags;

            // Flags found + any values associated (if there are any of either)
            flagEntries = new FlagInputContainer();

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

            // In case there are multiple optional flags but a certain number need to be provided
            if(command.MinFlagCount != ICommand.DEFAULT_MIN_FLAG_COUNT && command.MinFlagCount > flagEntries.Count)
            {
                Log.Error("Command {ICommand} requires at least {MinFlagCount} flags but got {FlagCount}.",
                    command.Name, command.MinFlagCount, flagEntries.Count);
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
                Log.Error("Flag {Flag} is not a valid for {ICommand}.", postCommandArg, command.Name);
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
            if (_helpCommand == null)
                return;

            string helpFullName = ICommand.PREFIX + HelpCommand.NAME;

            Log.Information("Use {Help} to display all valid commands. Optionally follow with the {Detailed} flag to see valid flags.",
                helpFullName, _helpCommand.Flags[0].name);
        }

        /// <summary> Populates the help command with all client and built-in commands </summary>
        private void PopulateHelpList()
        {
            if (_helpCommand == null)
                return;

            // Add all built-in and client commands to the help command list.
            foreach (var command in _commandDict.Values)
                    _helpCommand.AddCommand(command);
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
            {
                _isDiffLogger = true;
                Log.Logger = logger;
            }             
        }
        #endregion
    }
}

