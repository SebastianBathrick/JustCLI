namespace JustCLI
{
    /// <summary> Container for flags and values provided by the user after a command. </summary>
    public class FlagContainer
    {
        private List<Flag> _flags = new List<Flag>();
        private List<string?> _values = new List<string?>();

        public int Count => _flags.Count;

        public bool IsEmpty => _flags.Count == 0;

        public static FlagContainer Empty => new FlagContainer();

        /// <summary> Add a flag and value/null provided by user arguments. </summary>
        public void AddFlag(Flag flag, string? value = null)
        {
            _flags.Add(flag);
            _values.Add(value);
        }

        /// <summary> Gets the name of the first flag in the container. </summary>
        /// <exception cref="InvalidOperationException">Throws if the container is empty.</exception>
        public string PeekFlagName()
        {
            if (IsEmpty)
                throw new InvalidOperationException("No flags found in the container.");
            return _flags[0].name;
        }


        /// <summary> Attempts to retreive the value associated with a flag of the provided name. </summary>
        public bool TryGetValue(string flagName, out string? value)
        {
            value = null;
            flagName = VerifyFlagNamePrefix(flagName);

            if (!Contains(flagName))
                return false;

            for (int i = 0; i < _flags.Count; i++)
                if (_flags[i].name == flagName)
                {
                    value = GetValueAtIndex(i);
                    return value != string.Empty;
                }

            return false;
        }

        /// <summary>
        /// Returns true if the flag of the given name is found in the container and false otherwise.
        /// </summary>
        public bool Contains(string flagName)
        {
            flagName = VerifyFlagNamePrefix(flagName);

            foreach (var flag in _flags)
                if (flag.name == flagName)
                    return true;
            return false;
        }

        #region Helper Methods
        private string? GetValueAtIndex(int index)
        {
            string? value = _values[index];
            return value;
        }

        private string VerifyFlagNamePrefix(string flagName)
        {
            if (flagName.StartsWith(Flag.PREFIX))
                return flagName;
            return Flag.PREFIX + flagName;
        }
        #endregion
    }
}


