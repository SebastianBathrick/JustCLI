using JustCLI.Utilities;
using Serilog;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays the version of the app in use (to be set by client.)
    /// </summary>
    internal class VersionCommand : ICommand
    {
        private const string PLACEHOLDER_VERSION = "Unknown version";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => "version";

        public string Description => "Displays the version of the app in use.";

        public void SetVersion(string version) => _version = version;

        public void Execute(FlagInputContainer flagEntries)
        {
            Log.Logger.Information("Version: {Version}", _version);
        }
    }
}

