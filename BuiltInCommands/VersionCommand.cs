using Serilog;

namespace SebastianBathrick.JustCLI.BuiltInCommands
{
    public class VersionCommand : ICommand
    {
        private const string PLACEHOLDER_VERSION = "Unknown version";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => "version";

        public string Description => "Displays the version of the app in use.";

        public void SetVersion(string version) => _version = version;

        public bool Execute(List<(Flag flag, string? value)> usedFlagsAndValues)
        {
            try
            {
                Log.Logger.Information("Version: {Version}", _version);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred while executing the version command.");
                return false;
            }
        }
    }
}

