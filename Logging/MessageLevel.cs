namespace JustCLI.Logging
{
    /// <summary>Represents messages from most important to least.</summary>
    public enum MessageLevel
    {
        /// <summary>A critical failure indicating the application cannot continue.</summary>
        Fatal,

        /// <summary>An error that occurred during execution.</summary>
        Error,

        /// <summary>A warning about a potential issue or unexpected behavior.</summary>
        Warning,

        /// <summary>Informational message about normal application behavior.</summary>
        Info,

        /// <summary>Diagnostic information useful during development.</summary>
        Debug,
    }
}