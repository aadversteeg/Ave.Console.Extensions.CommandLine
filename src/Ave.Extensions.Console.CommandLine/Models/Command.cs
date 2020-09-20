using System.Collections.Generic;

namespace Ave.Extensions.Console.CommandLine.Models
{
    public class Command : CommandBase
    {
        public Command(string name, IReadOnlyCollection<Parameter> parameters) 
            : base( name)
        {
            Parameters = parameters;
        }

        public Command(string name)
            : this(name, new Parameter[0])
        {
        }

        public IReadOnlyCollection<Parameter> Parameters { get; }
   }
}
