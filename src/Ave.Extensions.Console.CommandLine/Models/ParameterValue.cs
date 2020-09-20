namespace Ave.Extensions.Console.CommandLine.Models
{
    public class ParameterValue
    {
        public ParameterValue(Parameter parameter, object value)
        {
            Parameter = parameter;
            Value = value;
        }

        public Parameter Parameter { get; }

        public object Value { get; }
    }
}
