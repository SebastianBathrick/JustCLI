using SebastianBathrick.JustCLI.BuiltInCommands;
using SebastianBathrick.JustCLI.Utilities;
using Serilog;

namespace SebastianBathrick.JustCLI
{
    public class CommandLineEnviro
    {
        private static CommandLineEnviro _instance = new CommandLineEnviro();

        private ArgContainer _argContainer;
        private HelpCommand _helpCommand;
        private VersionCommand _versionCommand;
        private Dictionary<string, ICommand> _commandDict;

        #region Setup Methods
        private CommandLineEnviro()
        {
            _instance = this;
            var args = Environment.GetCommandLineArgs();
            _argContainer = new ArgContainer(args);
            _argContainer.Get(); // Skip the first argument (the program name)

            _commandDict = new Dictionary<string, ICommand>();
            AddCommands([
                _helpCommand = new HelpCommand(), 
                _versionCommand = new VersionCommand(),
                ]);
            StartLogger();
        }

        /// <summary> Adds command to singleton instance. Should be called only before Start! </summary>
        public static void AddCommands(ICommand[] commands)
        {
            foreach (var command in commands)
                _instance._commandDict.Add(ICommand.PREFIX + command.Name, command);         
        }

        public static void SetVersion(string version) =>
            _instance._versionCommand.SetVersion(version);
        #endregion

        /// <summary> 
        /// Called by client to start reading commands (no commands should be added after calling.)
        /// </summary>
        /// <returns>True if the command line successfully read all commands or lack thereof.</returns>
        public static bool Start(bool requireCommand = false)
        {
            if (_instance._argContainer.IsEmpty)
            {
                if (!requireCommand)
                {
                    Log.Information("No arguments provided. Continuing execution...");
                    return true; // No commands is considered a valid execution config.
                }
                else
                {
                    Log.Error("No commands provided. Please type {Help} for a list of valid commands", 
                        ICommand.PREFIX + _instance._helpCommand);
                    return false; // No commands is not valid if required.
                }
            }

            foreach (var command in _instance._commandDict.Values)
                _instance._helpCommand.AddCommand(command);

            return _instance.ParseCommands();
        }
            
        private bool ParseCommands()
        {
            do
            {
                var arg = _argContainer.Get();

                if (!arg.StartsWith(ICommand.PREFIX))
                {
                    Log.Error("Expected command starting with {Prefix} but got {Arg}.", ICommand.PREFIX, arg);
                    return false;
                }

                if (!_commandDict.ContainsKey(arg))
                {
                    Log.Error("Command {Command} is not a valid command. Please type {Help} for a list of valid commands", 
                        arg, _helpCommand.Name);
                    return false;
                }

                var command = _commandDict[arg];

                if (!TryGetCommandFlags(command, out var flagsAndVals))
                    return false; // Message displayed by TryGetCommandFlags in each false condition.

                if(flagsAndVals == null)
                    return false; // To please Visual Studio, but should never happen.

                command.Execute(flagsAndVals);
            }
            while (!_argContainer.IsEmpty);

            return true;
        }

        #region Flag Methods
        /// <summary> Gets any userFlagsAndValues trailing the current command and before the next command. </summary>
        /// <returns> Returns true if all flags and values (or lack-thereof) have been collected successfully. </returns>
        private bool TryGetCommandFlags(ICommand command, out List<(Flag flag, string? value)>? userFlagsAndValues)
        {
            userFlagsAndValues = null;

            var possibleFlags = GetPossibleFlags();
            var validFlags = command.Flags;

            // Flags found + any values associated (if there are any of either)
            var usedFlags = new List<(Flag flag, string? value)>();

            for (int i = 0; i < possibleFlags.Count; i++)
            {
                if (!TryParseFlag(possibleFlags[i], command, out var nullableFlag))
                    return false;

                if (nullableFlag == null) // Declaration marks is nullable
                    return false;

                var validFlag = (Flag)nullableFlag;
                string? flagValue = null;

                // Check if the flag needs a value to follow it, and if so, cache it
                if(validFlag.isValueRequired)
                    // Check if the next argument is a value
                    if (i + 1 < possibleFlags.Count && !possibleFlags[i + 1].StartsWith(Flag.PREFIX))
                    {
                        flagValue = possibleFlags[i + 1];
                        i++; // Skip the value in the next iteration
                    }                        
                    else
                    {
                        Log.Error("Flag {Flag} requires a value but none was provided.", validFlag.name);
                        return false;
                    }

                usedFlags.Add((validFlag, flagValue));
            }

            // Check if all required flags were provided
            foreach (var flag in validFlags)
            {
                if (!flag.isRequired)
                    continue;

                bool isFlagUsed = usedFlags.Any(f => f.flag.name == flag.name);

                if (!isFlagUsed)
                {
                    Log.Error("Flag {Flag} is required but was not provided.", flag.name);
                    return false;
                }
            }

            userFlagsAndValues = usedFlags;
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

        private bool TryParseFlag(string postCommandArg, ICommand command, out Flag? flag)
        {
            flag = null;

            if (!postCommandArg.StartsWith(Flag.PREFIX))
            {
                Log.Error("Expected postCommandArg starting with {Prefix} but got {Flag}.", Flag.PREFIX, postCommandArg);
                return false;
            }

            if (!command.TryGetFlag(postCommandArg, out var validFlag))
            {
                Log.Error("Flag {Flag} is not a valid postCommandArg. Please type {Help} for a list of " +
                    "valid userFlagsAndValues", postCommandArg, _helpCommand);
                return false;
            }

            flag = validFlag;
            return true;
        }
        #endregion

        #region Helper Methods
        private void StartLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
        #endregion
    }
}



