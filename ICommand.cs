namespace SebastianBathrick.JustCLI
{
    public interface ICommand
    {
        public const string PREFIX = "--";

        public string Name { get; }

        public string Description { get; }

        public Flag[] Flags { get { return []; } }

        /// <summary> Defines what happens when the user enters this command into the CLI. </summary>
        /// <param name="providedFlags">The flags and values entered by the user.</param>   
        /// <returns>True if no exception was thrown in the scope of Execute.</returns>
        public bool Execute(List<(Flag flag, string? value)> providedFlags);

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
    }
}