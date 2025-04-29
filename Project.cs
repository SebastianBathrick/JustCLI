/*
using JustCLI.Logging;

Log.Info("Hello, {Name}!", "Alice");
Log.Warning("Disk space low: {FreeSpace} GB left", 2.5);
Log.Error(new Exception("File {FileName} not found"), "config.json");
Log.Fatal("Unrecoverable error in module {Module}", "AuthService");
Log.Debug("Loaded {Count} items from cache", 42);

// No placeholders — nothing should be colored in message
Log.Info("System started without parameters.");

// More placeholders than args — unfilled placeholders should be ignored
Log.Warning("Expecting {One}, {Two}, {Three}", "First", "Second");

// More args than placeholders — extra args should be ignored
Log.Debug("Using {Lang}", "C#", "ExtraArg1", "ExtraArg2");

// Placeholder edge case: no braces at all
Log.Fatal("Critical shutdown requested immediately.");

// Placeholder edge case: malformed token (no closing brace)
Log.Info("Bad token: {Unfinished", "ShouldNotShow");
*/