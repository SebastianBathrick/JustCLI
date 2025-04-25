using Serilog;

namespace JustCLI.BuiltInCommands
{
    internal class CLIVersionCommand : ICommand
    {
        private const string PLACEHOLDER_VERSION = "0.1-alpha";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => "cli_version";

        public string Description => "Displays the CLI version.";

        public void SetVersion(string version) => _version = version;

        public void Execute(FlagEntries? flagEntries)
        {
            Log.Logger.Information("JustCLI Version: {Version}", _version);
            Log.Logger.Information("JustCLI Author: {Author}", "Sebastian Bathrick");
            Log.Logger.Information("Github: {URL}", "https://github.com/SebastianBathrick/JustCLI");
        }
    }
}

