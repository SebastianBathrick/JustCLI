using Serilog;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// Command that displays the version of the CLI, its author, and the GitHub URL.
    /// </summary>
    internal class CLIVersionCommand : ICommand
    {
        private const string PLACEHOLDER_VERSION = "0.1-alpha";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => "cli_version";

        public string Description => "Displays the CLI version.";

        public void SetVersion(string version) => _version = version;

        public void Execute(FlagEntries? flagEntries)
        {
            Log.Information("[{VersionHeader}]", "VERSION INFO");
            Log.Information("[JustCLI Version]: {Version}", _version);
            Log.Information("[JustCLI Github]: {URL}", "https://github.com/SebastianBathrick/JustCLI");
            Log.Information("");
            var serilogAssembly = typeof(Serilog.Log).Assembly;
            var version = serilogAssembly.GetName().Version;
            Log.Information("[Serilog Version]: {Version}", version);
            Log.Information("[Serilog Github]: {URL}", "https://github.com/serilog/serilog");
            Log.Information("");
            Log.Information("[{ContactHeader}]", "SUPPORT");
            Log.Information("[Author]: {Author}", "Sebastian Bathrick");
            Log.Information("[Email]: sebastianbathrick@gmail.com");

        }
    }
}

