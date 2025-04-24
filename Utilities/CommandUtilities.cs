using Serilog;

namespace SebastianBathrick.JustCLI.Utilities
{
    public static class CommandUtilities
    {
        public static bool IsFlagUsed(string flagName, List<(Flag flag, string? value)> userFlagsAndValues)
        {
            foreach (var (flag, value) in userFlagsAndValues)
                if (flag.name == flagName)
                    return true;
            return false;
        }

        public static string GetUsedFlagValue(string flagName, List<(Flag flag, string? value)> userFlagsAndValues)
        {
            foreach (var (flag, value) in userFlagsAndValues)
                if (flag.name == flagName && !string.IsNullOrEmpty(value))
                    return value;

            return string.Empty;
        }

        public static bool TryGetFileContents(string filePath, out string[] fileLines)
        {
            if (Path.Exists(filePath))
            {
                fileLines = Array.Empty<string>();
                Log.Error("The file does not exist: {FilePath}", filePath);
                return false;
            }

            try
            {
                fileLines = File.ReadAllLines(filePath);
                return true;
            }
            catch (Exception e)
            {
                fileLines = Array.Empty<string>();
                Log.Error(e, "An error occurred while reading the file: {FilePath}", filePath);
                return false;
            }
        }
    }
}