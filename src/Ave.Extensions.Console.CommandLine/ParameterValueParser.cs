using System.ComponentModel;
using System.Linq;

namespace Ave.Extensions.Console.CommandLine
{
    public class ParameterValueParser<T> : IParameterValueParser
    {
        private T[] _allowedValues;

        public ParameterValueParser(T[] allowedValues)
        {
            _allowedValues = allowedValues;
        }

        public Result<object> Parse(string parameterName, string token)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var convertedValue = converter.ConvertFromString(token);

            if (_allowedValues != null)
            {
                if (!_allowedValues.Any(allowedValue => allowedValue.Equals(convertedValue)))
                {
                    return Result.Fail<object>($"'{token}' is not a valid value for parameter '{parameterName}'.");
                }
            }
            return Result.Ok<object>(token);
        }
    }
}
