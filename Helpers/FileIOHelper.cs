using System.Diagnostics;
using JustCLI.Logging;

namespace JustCLI.Helpers
{
    public static class FileIOHelper
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

        /// <summary> Gets a directory from the user and verifies the directory exists. </summary>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetDirectoryFromUser(string valueName)
        {
            Log.Info("Please enter a value for {ValueName} (directory):", valueName);
            while (true)
            {
                string? input = Console.ReadLine();
                if (Directory.Exists(input))
                    return input;
                Log.Error("Invalid enteredFilePath. Please enter a valid directory:");
            }
        }

        /// <summary> Gets a filepath from the user and verifies the file exists. </summary>
        /// <param name="allowEmptyFile"> If true, a path for an empty file can be returned.</param>
        /// <remarks> Loops continously until the user makes a valid entry. </remarks>
        public static string GetFilePathFromUser(
            string valueName, bool allowEmptyFile = false, bool doesRequireFile = true
            )
        {
            Log.Info("Please enter a value for {ValueName} (file path):", valueName);
            do
            {
                string? enteredFilePath = Console.ReadLine();

                try
                {
                    if (string.IsNullOrEmpty(enteredFilePath))
                    {
                        Log.Error("Invalid enteredFilePath. Please enter a valid file path:");
                        continue;
                    }

                    bool fileExists = File.Exists(enteredFilePath);

                    if (!fileExists && doesRequireFile)
                    {
                        Log.Error("Invalid enteredFilePath. Please enter a valid file path:");
                        continue;
                    }

                    if (fileExists && File.ReadLines(enteredFilePath).Count() == 0 && !allowEmptyFile)
                    {
                        Log.Error("The file {FilePath} is empty.", enteredFilePath);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e,
                        "An exception was thrown occurred while reading the file: {FilePath}. " +
                        "Likely a bug. Report to application author.", enteredFilePath);
                    continue;
                }

                return enteredFilePath;
            }
            while (true);
        }

        /// <summary> Prompts the user for a file path until the file is successfully created. </summary>
        /// <remarks> You can populate a file using an array of strings(lines), one string, or
        /// just leave it empty. </remarks>
        /// <returns>Returns the file path of the file created.</returns>
        public static string CreateFile(
            string extension, string? contents = null, string[]? contentLines = null)
        {
            do
            {
                // GetValue user input (already validated)
                var enteredFilePath = GetFilePathFromUser(
                    extension, doesRequireFile: false, allowEmptyFile: true);

                // Ensure the file has the correct extension
                if (!enteredFilePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    enteredFilePath += extension;

                if (File.Exists(enteredFilePath))
                {
                    Log.Error("The file {Name} already exists. Please enter a new file name:",
                        enteredFilePath);
                    continue;
                }

                // Create empty file
                File.Create(enteredFilePath).Close();

                // Write contents based on the provided arguments or leave empty
                if (contents != null)
                    File.WriteAllText(enteredFilePath, contents);
                else if (contentLines != null)
                    File.WriteAllLines(enteredFilePath, contentLines);

                Log.Info("The file {Name} was created successfully.", enteredFilePath);
                return enteredFilePath; // Add this line to fix the function
            }
            while (true);
        }

        /// <summary> Opens an application with any provided arguments. </summary>
        public static void OpenApplication(string app, string args = "")
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = app;
                    myProcess.StartInfo.Arguments = args;
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred while opening the application: {App}", app);
            }
        }


    }

}
