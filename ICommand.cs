namespace JustCLI
{
    public interface ICommand
    {
        public const string PREFIX = "--";

        public string Name { get; }

        public string? ShortHandName => null;

        public string Description { get; }

        public Flag[] Flags { get { return []; } }

        /// <summary> Defines what happens when the user enters this command into the CLI. </summary>
        /// <param name="flagEntries">The flags and values entered by the user.</param>   
        public void Execute(FlagEntries? flagEntries);

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