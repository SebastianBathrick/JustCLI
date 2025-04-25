using JustCLI.Utilities;
using Serilog;

namespace JustCLI
{
    internal class OpenFileTest : ICommand
    {
        private const string PATH_FLAG = "path";

        public string Name => "open";
        public string Description => "Opens a file in the default application.";
        public Flag[] Flags => new Flag[]
        {
            new Flag(PATH_FLAG, "The path to the file to open.", false, true)
        };
        public void Execute(FlagEntries? flagEntries)
        {
            if (flagEntries == null || !flagEntries.TryGetFlagValue(PATH_FLAG, out var filePath))
                filePath = CLIHelpers.GetFilePathFromUser("file to open", false);

            if (!CLIHelpers.TryGetFileContents(filePath, out var lines))
                return;

            foreach(var line in lines)
                Log.Logger.Information(line);
        }
    }
}

