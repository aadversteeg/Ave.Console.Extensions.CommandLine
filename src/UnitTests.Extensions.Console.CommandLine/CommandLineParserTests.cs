using System.Collections.Generic;
using System.ComponentModel;
using Ave.Extensions.Console.CommandLine;
using Ave.Extensions.Console.CommandLine.Models;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class CommandLineParserTests
    {
        public static readonly CommandGroup TestCommandGroup = new CommandGroup(
            "root",
            new CommandBase[]
            {
                new Command(
                    "login",
                    new[]
                    {
                        new Parameter("username", "u", null, false, new ParameterValueParser<string>(null)),
                        new Parameter("password", "p", null, false, new ParameterValueParser<string>(null))
                    }),
                new Command("logout"),
                new CommandGroup(
                    "account",
                    new CommandBase[] {
                       new Command("show")
                    }),
                new CommandGroup(
                    "group",
                    new CommandBase[]
                    {
                        new CommandGroup(
                            "deployment",
                            new CommandBase[]
                            {
                                new Command("create", new [] {
                                    new Parameter("name", null, null, true, new ParameterValueParser<string>(null)) })
                            })
                    })
            });

        public static readonly IReadOnlyCollection<Parameter> GlobalParameters = new[]
        {
            new Parameter("output", null, null, false, new ParameterValueParser<string>(new[] { "json, text"})),
            new Parameter("query", null, null, false,  new ParameterValueParser<string>(null)),
            new Parameter("debug", null, "false", false,  new ParameterValueParser<string>(null))
        };

        [Fact(DisplayName = "CLP-001: Parser should return specified command from root command group with parameter values.")]
        public void CLP001()
        {
            // arrange
            var parser = new CommandLineParser(TestCommandGroup, GlobalParameters);

            // act
            var result = parser.Parse("login --username user --password secret");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("login");
            result.ParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("username");
                    one.Value.Should().Be("user");
                },
                two =>
                {
                    two.Parameter.Name.Should().Be("password");
                    two.Value.Should().Be("secret");
                });
        }

        [Fact(DisplayName = "CLP-002: Parser should return specified command from root command group with parameter values when using abbreviations.")]
        public void CLP002()
        {
            // arrange
            var parser = new CommandLineParser(TestCommandGroup, GlobalParameters);

            // act
            var result = parser.Parse("login -u user -p secret");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("login");
            result.ParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("username");
                    one.Value.Should().Be("user");
                },
                two =>
                {
                    two.Parameter.Name.Should().Be("password");
                    two.Value.Should().Be("secret");
                });
        }

        [Fact(DisplayName = "CLP-003: Parser should return specified command from first level command group.")]
        public void CLP003()
        {

            // arrange
            var parser = new CommandLineParser(TestCommandGroup, GlobalParameters);

            // act
            var result = parser.Parse("account show");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("show");
        }


        [Fact(DisplayName = "CLP-004: Parser should return specified command from second level command group.")]
        public void CLP004()
        {
            // arrange
            var commandGroup = new CommandGroup(
                "root",
                new CommandBase[]
                {
                new CommandGroup(
                    "group",
                    new CommandBase[]
                    {
                        new CommandGroup(
                            "deployment",
                            new CommandBase[]
                            {
                                new Command("create")
                            })
                    })
                });

            var globalParameters = new Parameter[0]
            {
            };

            var parser = new CommandLineParser(commandGroup, globalParameters);

            // act
            var result = parser.Parse("group deployment create");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("create");
            result.ParameterValues.Count.Should().Be(0);
            result.GlobalParameterValues.Count.Should().Be(0);
        }

        [Fact(DisplayName = "CLP-005: Parser should return specified command from root command group with global parameter value when using global parameter.")]
        public void CLP005()
        {
            // arrange
            var commandGroup = new CommandGroup(
            "root",
            new CommandBase[]
            {
                new Command("logout")
            });

            var globalParameters = new[]
            {
                new Parameter("output", null, null, false, new ParameterValueParser<string>(null))
            };

            var parser = new CommandLineParser(commandGroup, globalParameters);

            // act
            var result = parser.Parse("logout --output json");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("logout");
            result.ParameterValues.Count.Should().Be(0);
            result.GlobalParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("output");
                    one.Value.Should().Be("json");
                });
        }

        [Fact(DisplayName = "CLP-006: Parser should return default value for not specified parameter in global parameters value list.")]
        public void CLP006()
        {
            // arrange
            var commandGroup = new CommandGroup(
            "root",
            new CommandBase[]
            {
                new Command("logout")
            });

            var globalParameters = new[]
            {
                new Parameter("debug", null, "false", false,  new ParameterValueParser<string>(null))
            };

            var parser = new CommandLineParser(commandGroup, globalParameters);

            // act
            var result = parser.Parse("logout");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("logout");
            result.ParameterValues.Count.Should().Be(0);
            result.GlobalParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("debug");
                    one.Value.Should().Be("false");
                });
        }

        [Fact(DisplayName = "CLP-007: Parser should return default value for not specified parameter command value list.")]
        public void CLP007()
        {
            // arrange
            var commandGroup = new CommandGroup(
            "root",
            new CommandBase[]
            {
                new Command("logout", new[] {
                    new Parameter("force", null, "false", false,  new ParameterValueParser<string>(null))
                })
            });

            var globalParameters = new Parameter[0]
            {
            };

            var parser = new CommandLineParser(commandGroup, globalParameters);

            // act
            var result = parser.Parse("logout");

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("logout");
            result.ParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("force");
                    one.Value.Should().Be("false");
                });
            result.GlobalParameterValues.Count.Should().Be(0);
        }

        [Theory(DisplayName = "CLP-008: Parser should return quoted parameter value correctly.")]
        [InlineData( "tasks add --title mytitle", "mytitle")]
        [InlineData( "tasks add --title \"my title\"", "my title")]
        [InlineData("tasks add --title \"  my title  \"", "  my title  ")]
        public void CLP008(string commandLine, string expectedParameterValue)
        {
            // arrange
            var commandGroup = new CommandGroup(
            "root",
            new CommandBase[]
            {
                new CommandGroup
                (
                    "tasks",
                    new CommandBase[]
                    {
                        new Command(
                            "add", 
                            new[] {
                                new Parameter("title", "t", null, false,  new ParameterValueParser<string>(null))
                        })
                    }
                )
            });

            var globalParameters = new Parameter[0]
            {
            };

            var parser = new CommandLineParser(commandGroup, globalParameters);

            // act
            var result = parser.Parse(commandLine);

            // assert
            result.Should().BeSuccessful();
            result.Command.Name.Should().Be("add");
            result.ParameterValues.Should().SatisfyRespectively(
                one =>
                {
                    one.Parameter.Name.Should().Be("title");
                    one.Value.Should().Be(expectedParameterValue);
                });
            result.GlobalParameterValues.Count.Should().Be(0);
        }

        [Theory(DisplayName = "CLP-999: Parser should return error if specified command line is not valid.")]
        [InlineData("hide", "'hide' is not in the 'root' command group.")]
        [InlineData("account hide", "'hide' is not in the 'root account' command group.")]
        [InlineData("group deployment destroy", "'destroy' is not in the 'root group deployment' command group.")]
        [InlineData("logout --output xml", "'xml' is not a valid value for parameter 'output'.")]
        [InlineData("group deployment create", "missing required parameter 'name'.")]
        public void CLP999(string commandLine, string error)
        {
            // arrange
            var parser = new CommandLineParser(TestCommandGroup, GlobalParameters);

            // act
            var result = parser.Parse(commandLine);

            // assert
            result.Should().BeFailure();
            result.Error.Should().Be(error);
        }
    }
}
