# JustCLI

JustCLI is a lightweight .NET command-line interface library designed to simplify the creation of command-line applications with minimal setup.

## Installation

### .NET CLI
```bash
dotnet add package JustCLInterface --version 0.0.1-alpha
```

### Package Manager
```powershell
NuGet\Install-Package JustCLInterface -Version 0.0.1-alpha
```

## Quick Start

Here's a simple example to get you started:

```csharp
using JustCLI;

// Add commands
CommandLineApp.AddCommands(
    [
        new CommandTemplate(
            "hello",
            "Prints hello world.",
            null,
            (flagEntries) =>
            {
                Console.WriteLine("Hello, world!");
            }
        ),
        new CommandTemplate(
            "goodbye",
            "Prints goodbye world.",
            null,
            (flagEntries) =>
            {
                Console.WriteLine("Goodbye, world!");
            }
        )
    ]
);

// Start the app
CommandLineApp.Start(isPromptBeforeExit: true);
```

## Features

- Simple command and flag system
- Built-in help command
- Easy command template creation
- Support for required and optional flags
- Flag values support
- Built-in validation for commands and flags

## Basic Concepts

### Commands

Commands start with `--` and are the primary entry point for your CLI functions:

```
--command
```

### Flags

Flags start with `-` and can be added to commands to modify their behavior:

```
--command -flag
```

### Flag Values

When a flag requires a value, the value must be:
- The argument directly after the flag
- Only one value per flag is allowed
- The value cannot start with `-` or `--` (cannot be another flag or command)

Example:
```
--command -flag value
```

### Creating Commands

You can create commands in two ways:

1. Using the `CommandTemplate` class (simplest approach):
```csharp
new CommandTemplate(
    "command-name",
    "Description of the command",
    [new Flag("flag-name", "Description of the flag")],
    (flagEntries) => {
        // Command execution logic here
    }
);
```

2. By implementing the `ICommand` interface for more complex cases.

### Working with Flags

Create flags to add parameters to your commands:

```csharp
new Flag(
    "name",             // Flag name (will be automatically prefixed with -)
    "Description",      // Description for help command
    isRequired: true,   // Whether the flag is required
    isValueRequired: true  // Whether the flag requires a value
)
```

When `isValueRequired` is set to `true`, the library will automatically look for a value after the flag and throw an error if one is not provided or if the value starts with `-` (which would indicate another flag).

### Reading Flag Values

When your command executes, it receives a `FlagEntries` object:

```csharp
(flagEntries) => {
    // Get a flag value
    if (flagEntries.TryGetFlagValue("flag-name", out string value))
    {
        Console.WriteLine($"Flag value: {value}");
    }
    
    // Check if a flag exists
    if (flagEntries.IsFlag("flag-name"))
    {
        Console.WriteLine("Flag exists");
    }
}
```

## Command Line Format

The general format for commands and flags is:

```
--command -flag1 -flag2 value2 -flag3
```

Where:
- `--command` is the command name
- `-flag1` is a flag with no value
- `-flag2 value2` is a flag with a value
- `-flag3` is another flag with no value

## Built-in Commands

JustCLI comes with several built-in commands:

- `--help`: Displays help information
  - `-detailed`: Shows detailed flag information
- `--version`: Shows your application version (set via `CommandLineApp.SetVersion()`)
- `--cli_version`: Shows JustCLI version information
- `--clear`: Clears the terminal

## Configuration

```csharp
// Set application version
CommandLineApp.SetVersion("1.0.0");

// Set default command (executes when no command is provided)
CommandLineApp.AddDefaultCommand(new CommandTemplate(
    "default",
    "Default command",
    null,
    (flagEntries) => {
        Console.WriteLine("Default command executed");
    }
));

// Start with options
CommandLineApp.Start(
    requireCommand: true,  // Error if no command is provided
    isPromptBeforeExit: true  // Ask if the user wants to enter more commands
);
```

## Helper Utilities

JustCLI provides several helper utilities for getting user input:

```csharp
using JustCLI.Utilities;

// Get user input
string userInput = CLIHelpers.GetStringFromUser("input name");
int number = CLIHelpers.GetIntFromUser("number");
bool answer = CLIHelpers.YesNoPrompt("Continue?");
string filePath = CLIHelpers.GetFilePathFromUser("file");
```

## License

Copyright © Sebastian Bathrick 2025. All Rights Reserved.

This software is proprietary and confidential.
Unauthorized use, modification, or distribution is strictly prohibited.
For licensing inquiries, please contact sebastianbathrick@gmail.com.
