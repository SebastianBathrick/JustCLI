using Serilog;
using System.Reflection;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// Command that displays the serilogVersion of the CLI, its author, and the GitHub URL.
    /// </summary>
    internal class CLIVersionCommand : ICommand
    {
        public string Name => "cli_version";

        public string Description => "Displays version of the CLI and its dependencies.";

        public void Execute(FlagEntries? flagEntries)
        {
            var cliVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log.Information("[{VersionHeader}]", "VERSION INFO");
            Log.Information("[JustCLI Version]: {Version}", cliVersion);
            Log.Information("[JustCLI Github]: {URL}", "https://github.com/SebastianBathrick/JustCLI");
            Log.Information("");

            var serilogAssembly = typeof(Serilog.Log).Assembly;
            var serilogVersion = serilogAssembly.GetName().Version;
            Log.Information("[Serilog Version]: {Version}", serilogVersion);
            Log.Information("[Serilog Github]: {URL}", "https://github.com/serilog/serilog");
            Log.Information("");
            Log.Information("[{ContactHeader}]", "SUPPORT");
            Log.Information("[Author]: {Author}", "Sebastian Bathrick");
            Log.Information("[Email]: sebastianbathrick@gmail.com");

        }
    }
}

