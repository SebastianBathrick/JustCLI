using Serilog;

namespace JustCLI.Utilities
{
    public static class CLIHelpers
    {
        /// <summary>
        /// Tries to read the contents of a file at the specified path.
        /// </summary>
        /// <param name="filePath">Valid or invalid file path</param>
        /// <param name="fileLines">Contains each line of the file if found, and empty if not found.</param>
        /// <param name="allowEmpty">If true, an empty file will not print a message and return false.</param>
        /// <returns>Returns true if a file is found and meets the requirements specified.</returns>
        public static bool TryGetFileContents(
            string filePath, 
            out string[] fileLines, 
            bool allowEmpty = false
            )
        {
            if (!Path.Exists(filePath))
            {
                fileLines = Array.Empty<string>();
                Log.Error("The file does not exist: {FilePath}", filePath);
                return false;
            }

            try
            {
                fileLines = File.ReadAllLines(filePath);

                if (fileLines.Length == 0 && !allowEmpty)
                {
                    Log.Error("The file is empty: {FilePath}", filePath);
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                fileLines = Array.Empty<string>();
                Log.Error(e, "An error occurred while reading the file: {FilePath}", filePath);
                return false;
            }
        }

        #region User Input Methods
        #region Get Primitives
        /// <summary> Gets a int literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static int GetIntFromUser(string valueName)
        {
            Log.Information("Please enter a value for {ValueName} (int):", valueName);

            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                    return result;
                Log.Error("Invalid input. Please enter a valid integer:");
            }
        }

        /// <summary> Gets a float literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static float GetFloatFromUser(string valueName)
        {
            Log.Information("Please enter a value for {ValueName} (float):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (float.TryParse(input, out float result))
                    return result;
                Log.Error("Invalid input. Please enter a valid float:");
            }
        }

        /// <summary> Gets a string literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetStringFromUser(string valueName)
        {
            Log.Information("Please enter a value for {ValueName} (string):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                    return input;
                Log.Error("Invalid input. Please enter a valid string:");
            }
        }

        /// <summary> Gets a boolean literal from the user. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static bool GetBoolFromUser(string valueName)
        {
            Log.Information("Please enter a value for {ValueName} (true or false):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (bool.TryParse(input, out bool result))
                    return result;
                Log.Error("Invalid input. Please enter a valid boolean:");
            }
        }
        #endregion

        #region Get Paths
        /// <summary> Gets a directory from the user and verifies the directory exists. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetDirectoryFromUser(string valueName)
        {
            Log.Information("Please enter a value for {ValueName} (directory):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (Directory.Exists(input))
                    return input;
                Log.Error("Invalid input. Please enter a valid directory:");
            }
        }

        /// <summary> Gets a filepath from the user and verifies the file exists. </summary>
        /// <param name="allowEmptyFile"> If true, a path for an empty file can be returned.</param>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetFilePathFromUser(string valueName, bool allowEmptyFile=false)
        {
            Log.Information("Please enter a value for {ValueName} (file path):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();


                if (!string.IsNullOrEmpty(input) && File.Exists(input))
                {
                    if (allowEmptyFile)
                        return input;

                    try
                    {
                        if (File.ReadAllLines(input).Count() != 0)
                            return input;

                        Log.Error("The file is empty: {FilePath}", input);

                    }
                    catch(Exception e)
                    {
                        Log.Error(e, "An error occurred while reading the file: {FilePath}", input);
                    }                
                }
                    
                Log.Error("Invalid input. Please enter a valid file path:");
            }
        }
        #endregion

        #region Prompts
        /// <summary> Prompts the user for a yes or no response. </summary>
        public static bool YesNoPrompt(
            string promptMessage, 
            bool isDefault = false, 
            bool defaultTo = false
            )
        {
            Log.Information("{Message} (y/n):", promptMessage);
            while (true)
            {
                string? input = Console.ReadLine();

                if(string.IsNullOrEmpty(input))
                {
                    if (isDefault)
                        return defaultTo;
                }
                if (input.ToLower() == "y")
                    return true;
                else if (input.ToLower() == "n")
                    return false;
                Log.Error("Invalid input. Please enter 'y' or 'n':");
            }
        }
        #endregion
        #endregion
    }
}