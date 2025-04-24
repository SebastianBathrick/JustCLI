namespace SebastianBathrick.JustCLI
{
    public struct Flag
    {
        public const string PREFIX = "-";

        public string name;
        public string desc;
        public bool isRequired;
        public bool isValueRequired;

        public Flag(
            string name,
            string desc = "No description provided.",
            bool isRequired = true,
            bool isValueRequired = false)
        {
            this.name = PREFIX + name;
            this.desc = desc;
            this.isRequired = isRequired;
            this.isValueRequired = isValueRequired;
        }

        public override string ToString()
        {
            var strRep = String.Empty;

            if (!isRequired)
                strRep += "{Name}: ";
            else
                strRep += "{Name} (Required): ";

            strRep += desc;

            if (isValueRequired)
                strRep += " Must be directly followed by an argument.";

            return strRep;
        }
    }
}
