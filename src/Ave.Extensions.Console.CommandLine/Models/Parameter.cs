namespace Ave.Extensions.Console.CommandLine.Models
{
    public class Parameter
    {
        private IParameterValueParser _valueParser;

        public Parameter(string name, string abbreviation, string defaultValue, bool mandatory, IParameterValueParser valueParser)
        {
            Name = name;
            Abbreviation = abbreviation;
            DefaultValue = defaultValue;
            Mandatory = mandatory;
            _valueParser = valueParser;
        }

        public string Name { get; }

        public string Abbreviation { get; }

        public string DefaultValue { get; }

        public string[] AllowedValues { get; }

        public bool Mandatory { get; }

        public Result<object> Convert(string token)
        {
            return _valueParser.Parse(Name, token);
        }
    }
}
