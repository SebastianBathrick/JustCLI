# JustCLI

JustCLI is a lightweight .NET 8 command-line interface framework designed to simplify the creation of command-line applications with minimal setup while maintaining flexibility.

## Installation

### .NET CLI
```bash
dotnet add package JustCLInterface --version 0.0.16-alpha
```

### Package Manager
```powershell
NuGet\Install-Package JustCLInterface -Version 0.0.16-alpha
```

## Quick Start

Here's a simple example to get you started:

```csharp
using JustCLI;
using Serilog;

// Setup logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

// Add commands using the ActionCommand class
CLI.AddCommands([
    new ActionCommand(
        "hello",
        "Prints hello world.",
        (flagEntries) => {
            Log.Information("Hello, world!");
        }
    ),
    new ActionCommand(
        "greet",
        "Greets a user by name.",
        (flagEntries) => {
            if (flagEntries.TryGetValue("name", out string? name))
                Log.Information("Hello, {Name}!", name);
            else
                Log.Information("Hello, anonymous user!");
        },
        [new Flag("name", "The name to greet", false, true)]
    )
]);

// Set the application version
CLI.SetVersion("1.0.0");

// Start the CLI
CLI.Start(allowMoreCommands: true);
```

## Core Concepts

### Commands and Flags

In JustCLI, the command-line interface follows these conventions:

- **Commands** start with `--` (e.g., `--help`, `--version`)
- **Flags** start with `-` (e.g., `-name`, `-v`)
- **Flag Values** follow directly after flags that require them (e.g., `-name John`)

Example of a command with flags:
```
--command -flag1 -flag2 value2 -flag3
```

### Creating Commands

JustCLI offers two ways to create commands:

#### 1. Using the ActionCommand class (recommended for simple commands)

```csharp
new ActionCommand(
    "command-name",
    "Description of the command",
    (flagEntries) => {
        // Command execution logic here
    },
    [new Flag("flag-name", "Description of the flag")], // Optional flags array
    minFlagCount: 0 // Optional minimum flag count
)
```

#### 2. Implementing the ICommand interface (for more complex commands)

```csharp
public class CustomCommand : ICommand
{
    public string Name => "custom";
    public string Description => "A custom command implementation";
    public int MinFlagCount => 0; // Override if needed
    
    public Flag[] Flags => [
        new Flag("flag1", "Description of flag1"),
        new Flag("flag2", "Description of flag2", false, true)
    ];
    
    public void Execute(FlagInputContainer flagEntries)
    {
        // Command execution logic
    }
}
```

### Working with Flags

Create flags to add parameters to your commands:

```csharp
new Flag(
    "name",             // Flag name (will be automatically prefixed with -)
    "Description",      // Description for help command
    isRequired: true,   // Whether the flag is required (default: true)
    isValueRequired: false  // Whether the flag requires a value (default: false)
)
```

### Reading Flag Values

When your command executes, it receives a `FlagInputContainer` object:

```csharp
(flagEntries) => {
    // Check if a flag exists
    if (flagEntries.IsFlag("flag-name"))
    {
        Log.Information("Flag exists");
    }
    
    // Get a flag value
    if (flagEntries.TryGetValue("flag-name", out string? value))
    {
        Log.Information("Flag value: {Value}", value);
    }
}
```

## Built-in Commands

JustCLI comes with several built-in commands:

- `--help`: Displays help information
  - `-v`: Shows detailed flag information (verbose mode)
- `--version`: Shows your application version (set via `CLI.SetVersion()`)
- `--clear`: Clears the terminal
- `--exit`: Exits the application

## Configuration Options

```csharp
// Set application version
CLI.SetVersion("1.0.0");

// Add a default command (executes when no command is provided)
CLI.AddDefaultCommand(new ActionCommand(
    "default",
    "Default command",
    (flagEntries) => {
        Log.Information("Default command executed");
    }
));

// Define a custom logger (optional)
var customLogger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
CLI.DefineLogger(customLogger);

// Start the CLI with options
CLI.Start(
    requireCommand: true,       // Error if no command is provided
    allowMoreCommands: true,    // Allow the user to enter more commands after execution
    argOverride: null,          // Override command-line arguments (optional)
    useBuiltInCommands: true    // Include built-in commands (help, version, clear, exit)
);
```

## Utility Helpers

JustCLI provides several utility helper classes to simplify common operations:

### PrimitiveIOHelper
```csharp
// Get primitive values from user
string input = PrimitiveIOHelper.GetStringFromUser("input name");
int number = PrimitiveIOHelper.GetIntFromUser("number");
float decimal = PrimitiveIOHelper.GetFloatFromUser("decimal");
bool flag = PrimitiveIOHelper.GetBoolFromUser("boolean");
```

### PromptHelper
```csharp
// Present options to user
string[] options = ["Option 1", "Option 2", "Option 3"];
int selection = PromptHelper.PickOption(options, "Select an option:");

// Yes/No prompt
bool answer = PromptHelper.YesNoPrompt("Continue?", 
    isDefault: true, 
    defaultTo: false,
    onYes: () => Log.Information("User said yes"),
    onNo: () => Log.Information("User said no")
);
```

### FileIOHelper
```csharp
// Get file paths from user
string filePath = FileIOHelper.GetFilePathFromUser("config file");
string directory = FileIOHelper.GetDirectoryFromUser("output directory");

// Try to read file contents
if (FileIOHelper.TryGetFileContents(filePath, out string[] lines))
{
    foreach (var line in lines)
        Log.Information(line);
}

// Create a new file
string newFilePath = FileIOHelper.CreateFile(".txt", "File contents here");

// Open an application
FileIOHelper.OpenApplication("notepad.exe", filePath);
```

### LogHelper
```csharp
// Format log output
LogHelper.PrintHeader("PROCESS STARTED");
LogHelper.LogLineVisual();
LogHelper.LogExtraLine(2);
```

## Advanced Usage

### Removing Commands
```csharp
// Remove a specific command
CLI.RemoveCommand("command-name");

// Clear all commands
CLI.ClearCommands(excludeBuiltIns: true);
```

### Exiting Programmatically
```csharp
// Set the exit flag to terminate the CLI loop
CLI.SetExiting(true);
```

## License

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
