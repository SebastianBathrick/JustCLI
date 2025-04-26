using JustCLI.Utilities;
using Serilog;
using System;
using System.Reflection;

namespace JustCLI.BuiltInCommands
{
    /// <summary>
    /// ICommand that displays the version of the CLI, its author, and the GitHub URL.
    /// </summary>
    internal class CLIVersionCommand : ICommand
    {
        private const string PLACEHOLDER_VERSION = "0.1-alpha";

        private string _version = PLACEHOLDER_VERSION;

        public string Name => "cli_version";

        public string Description => "Displays the CLI version.";

        public void Execute(FlagInputContainer flagEntries)
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            Log.Information("{VersionHeader}", "[VERSION INFO]");
            PrintVersionInfo(
                "JustCLI",
                assemblyVersion,
                "https://github.com/SebastianBathrick/JustCLI");

            Log.Information("{VersionHeader}", "[DEPENDENTS' VERSION INFO]");
            PrintVersionInfo(
                "Serilog",
                "4.2.0",
                "https://github.com/serilog/serilog");

            PrintVersionInfo(
                "Serilog.Sinks.Console",
                "6.0.0",
                "https://github.com/serilog/serilog-sinks-console");

            PrintVersionInfo(
                "TextCopy",
                "6.2.1",
                "https://github.com/CopyText/TextCopy");

            Log.Information("{ContactHeader}", "[SUPPORT]");
            Log.Information("[Author]: {Author}", "Sebastian Bathrick");
            Log.Information("[Email]: sebastianbathrick@gmail.com");
            Log.Information("");
            Log.Information("{LicenseHeader}", "[LICENSE]");
            Log.Information("{License}", "Sebastian Bathrick 2025. All Rights Reserved.");
            Log.Information("To see more information about licensing visit the Github repository provided above.");
            LogHelper.LogExtraLine();
        }

        private void PrintVersionInfo(string pkgName, string version, string url)
        {
            Log.Information($"[{pkgName} Version]:" + "{Version}", version);
            Log.Information($"[{pkgName} GitHub]:" + "{GitHub}", "https://github.com/serilog/serilog");
            LogHelper.LogExtraLine();
        }
    }
}

