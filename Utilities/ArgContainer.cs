namespace SebastianBathrick.JustCLI.Utilities
{
    public class ArgContainer
    {
        private string[] _args;
        private int _currIndex;

        public bool IsEmpty => _currIndex >= _args.Length;

        public ArgContainer(string[] args)
        {
            _args = args;
            _currIndex = 0;
        }

        public string Peek() => _args[_currIndex];

        public string Get() => _args[_currIndex++];
    }
}
