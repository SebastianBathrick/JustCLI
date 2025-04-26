using JustCLI.Utilities;
using Serilog;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays the version of the app in use (to be set by client.)
    /// </summary>
    internal class OpenVSCommand : ICommand
    {
        private const string FILE_PATH_FLAG = "f";
        private const string VS_ENVIRO_VAR = "code";  

        public string Name => "open_vs";

        public Flag[] Flags => [
            new Flag(FILE_PATH_FLAG, "Opens the file in the following argument.", false)
            ];

        public string Description => "Opens VS Code using its environment variable.";

        public void Execute(FlagInputContainer flagEntries)
        {
            if (flagEntries.IsEmpty)
                FileIOHelper.OpenApplication(VS_ENVIRO_VAR);
        }
    }
}

