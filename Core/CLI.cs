using JustCLI.BuiltInCommands;
using JustCLI.Utilities;

namespace JustCLI
{
    /// <summary>
    /// Singleton class that handles command line arguments and executes commands.
    /// </summary>
    public class CLI
    {
        #region Variables
        private static CLI _instance = new CLI();

        private HelpCommand? _helpCommand;
        
        private ICommand? _defaultCommand;

        private Dictionary<string, ICommand> _commandDict;
        private ArgContainer _argContainer;

        private bool _isExiting = false; 
        #endregion

        #region Setup Methods
        private CLI()
        {
            Log.SetLogger(new Logging.Default.TerminalLogger());

            // Get the user's arguments from the command line (excluding the program name)
            _argContainer = new ArgContainer(Environment.GetCommandLineArgs());
            _argContainer.Get(); // Skip the first argument (the program path)
            _commandDict = new Dictionary<string, ICommand>();
        }

        /// <summary> Adds client commands to the help list (provided there is a help command) </summary>
        private void AddCommandsToHelp()
        {
            if (_helpCommand == null)
                return;

            // Add all built-in and client commands to the help command list.
            foreach (var command in _commandDict.Values)
                _helpCommand.AddCommand(command);
        }
        #endregion

        #region Command Parsing & Execution
        private void StartInstance(
            bool requireCommand,
            bool allowMoreCommands,
            string[]? argOverride,
            bool isHelp
            )
        {
            if (isHelp)
            {
                _helpCommand = new HelpCommand();
                AddCommand(_helpCommand);
                AddCommandsToHelp();
            }
               
            if (_commandDict.Count == 0)
            {
                Log.Error("No commands are defined for the command line interface.");
                return;
            }

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
                    Log.Info("Please enter {Arg}:", "valid argument(s)");
                    string? rawUserInput = Console.ReadLine();
                    rawUserInput ??= string.Empty;

                    _argContainer = new ArgContainer(SplitSpacedText(rawUserInput));
                }

                if (_argContainer.IsEmpty)
                {
                    if (_defaultCommand != null)
                        _defaultCommand.Execute(FlagContainer.Empty);
                    else if(requireCommand)
                    {
                        Log.Error("No arguments were provided.");
                        LogHelpDirections();
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

        /// <summary> Gets any userFlagsAndValues trailing the current command and before the next command. </summary>
        /// <returns> Returns true if all flags and values (or lack-thereof) have been collected successfully. </returns>
        private bool TryGetCommandFlags(ICommand command, out FlagContainer flagEntries)
        {
            var possibleFlags = GetPossibleFlagsOrValues();
            var validFlags = command.Flags;

            // Flags found + any values associated (if there are any of either)
            flagEntries = new FlagContainer();

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
                if (!flagEntries.Contains(flag.name))
                {
                    Log.Error("Flag {Flag} is required but was not provided.", flag.name);
                    LogHelpDirections();
                    return false;
                }

            // In case there are multiple optional flags but a certain number need to be provided
            if (command.MinFlagCount != ICommand.DEFAULT_MIN_FLAG_COUNT && command.MinFlagCount > flagEntries.Count)
            {
                Log.Error("Command {ICommand} requires at least {MinFlagCount} flags but got {FlagCount}.",
                    command.Name, command.MinFlagCount, flagEntries.Count);
                LogHelpDirections();
                return false;
            }

            // All flags and values were collected successfully
            return true;
        }

        /// <summary> Returns an empty list or args up until the _argContainer is empty, or the next arg is a command </summary>
        private List<string> GetPossibleFlagsOrValues()
        {
            var possibleFlags = new List<string>();

            while (!_argContainer.IsEmpty && !_argContainer.Peek().StartsWith(ICommand.PREFIX))
                possibleFlags.Add(_argContainer.Get());

            return possibleFlags;
        }

        private bool TryParseFlag(string arg, ICommand command, out Flag? flag)
        {
            flag = null;

            if (!arg.StartsWith(Flag.PREFIX))
            {
                Log.Error("Expected flag starting with {Prefix} but got {Flag}.", Flag.PREFIX, arg);
                LogHelpDirections();
                return false;
            }

            if (!command.TryGetFlag(arg, out var validFlag))
            {
                Log.Error("Flag {Flag} is not a valid for {ICommand}.", arg, command.Name);
                LogHelpDirections();
                return false;
            }

            flag = validFlag;
            return true;
        }
        #endregion

        #region Public Static Interface
        /// <summary> 
        /// Called by client to start reading commands (no commands should be added  after calling.)
        /// </summary>
        /// <param name="requireCommand">Whether an error will be printed and the CLI will exit if 
        /// there are no args.</param>
        /// <param name="allowMoreCommands">Whether the user is able to enter more  commands after
        /// environment args.</param>
        /// <param name="argOverride">If not null overrides initial args provided to the program.</param>
        /// <param name="useHelp">Whether or not built-in commands will 
        /// be included in the interface</param>
        public static void Start(
            bool requireCommand = false,
            bool allowMoreCommands = false,
            string[]? argOverride = null,
            bool useHelp = true
            )
        {
            // Start just serves as a wrapper and StartInstance does as described by the docs
            _instance.StartInstance(requireCommand, allowMoreCommands, argOverride, useHelp);
        }

        /// <summary> Removes client command with the provided name with no command prefix. </summary>
        public static void RemoveCommand(string commandName)
        {
            var cli = _instance;
            string prefixedName = ICommand.PREFIX + commandName;

            if (!cli._commandDict.TryGetValue(prefixedName, out var command))
            {
                Log.Warning("Attempted to remove command {Name} but it does not exist.", commandName);
                return;
            }

            cli._commandDict.Remove(prefixedName);
        }
            
        /// <summary> Removes all client defined commands. </summary>
        /// <remarks> Does not include help command because it can be disabled on start. </remarks>
        public static void ClearCommands(bool excludeBuiltIns = true) => _instance._commandDict.Clear();

        /// <summary> Adds all built in commands to CLI instance. </summary>
        public static void ForceExit(object? startReturnObj)
        {
            _instance._isExiting = true;
            Log.Info("Exiting CLI...");             
        }

        /// <summary> Assigns command to execute if no args are provided during the Start() method. </summary>
        public static void SetDefaultCommand(ICommand command)
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

        /// <summary> Adds command to singleton instance. </summary>
        public static void AddCommand(ICommand command)
        {
            if (!_instance.IsCommandAlreadyAdded(command))
            {
                if (command.MinFlagCount > command.Flags.Length)
                    Log.Error($"Command {command.Name} has more required flags than available. " +
                        $"MinFlagCount: {command.MinFlagCount}, Flags: {command.Flags.Length}");
                else
                    _instance._commandDict.Add(ICommand.PREFIX + command.Name, command);
            }
        }

        /// <summary> Exits after the current command is completed. </summary>
        public static void Exit() =>
            _instance._isExiting = true;
        #endregion

        #region Helper Methods
        private void LogHelpDirections()
        {
            if (_helpCommand == null)
                return;

            string helpFullName = ICommand.PREFIX + HelpCommand.NAME;

            Log.Info("Use {Help} to display all valid commands. Optionally follow with the {Detailed} flag to see valid flags.",
                helpFullName, _helpCommand.Flags[0].name);
        }

        private string[] SplitSpacedText(string text) =>
            text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        
        private bool IsCommandAlreadyAdded(ICommand command) =>
            _instance._commandDict.ContainsKey(ICommand.PREFIX + command.Name);
        #endregion
    }
}

