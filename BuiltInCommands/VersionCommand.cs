using JustCLI.Utilities;
using JustCLI.Logging;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays the version of the app in use (to be set by client.)
    /// </summary>
    internal class VersionCommand : ICommand
    {
        public const string NAME = "version";
        private const string PLACEHOLDER_VERSION = "Unknown version";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => NAME;

        public string Description => "Displays the version of the app in use.";

        public void SetVersion(string version) => _version = version;

        public void Execute(FlagInputContainer flagEntries)
        {
            Log.Info("Version: {Version}", _version);
        }
    }
}

