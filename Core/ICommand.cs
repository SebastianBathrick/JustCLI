using JustCLI.Utilities;

namespace JustCLI
{
    public interface ICommand
    {
        public const string PREFIX = "--";
        public const int DEFAULT_MIN_FLAG_COUNT = 0;

        /// <summary> The name the user will type to use command. </summary>
        public string Name { get; }

        /// <summary> The description of the command that will be shown by the help command </summary>
        public string Description { get; }

        /// <summary> 
        /// Minimum number of flags that need to follow to use this command.
        /// </summary>
        /// <remarks>
        /// Important when there are multiple optional flags but at least one needs to be entered.
        /// </remarks>
        public int MinFlagCount => DEFAULT_MIN_FLAG_COUNT; 

        /// <summary> Required and optional flags that can follow the command (or lack thereof) </summary>
        public Flag[] Flags { get { return []; } }

        /// <summary> Defines what happens when the user enters this command into the CLI. </summary>
        /// <param name="flagEntries">Holds flags the user entered and any values associated with them.</param>   
        public void Execute(FlagInputContainer flagEntries);

        /// <summary> Returns true and the flag of the given name if it exists. </summary>
        public bool TryGetFlag(string flagName, out Flag? validFlag)
        {
            foreach (var flag in Flags)
            {
                if (flag.name == flagName)
                {
                    validFlag = flag;
                    return true;
                }
            }

            validFlag = null;
            return false;
        }

        /// <summary> Returns true if the command has no required flags. </summary>
        public bool HasNoRequiredFlags() =>
            Flags.Count() == 0 || GetRequiredFlags().Count() == 0;

        /// <summary> Returns a list containing all required flags for this command. </summary>
        public List<Flag> GetRequiredFlags()
        {
            List<Flag> requiredFlags = new List<Flag>();
            foreach (var flag in Flags)
            {
                if (flag.isRequired)
                    requiredFlags.Add(flag);
            }
            return requiredFlags;
        }
    }
}