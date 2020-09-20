using System;
using System.Collections.Generic;
using System.Text;

namespace Ave.Extensions.Console.CommandLine
{
    public interface IParameterValueParser
    {
        Result<object> Parse(string parameterName, string token);
    }
}
