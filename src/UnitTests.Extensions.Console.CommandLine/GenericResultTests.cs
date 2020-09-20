using System;
using Ave.Extensions.Console.CommandLine;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class GenericResultTests
    {
        [Fact(DisplayName = "GR-001: Calling Result.Ok is a success and not a failure.")]
        public void GR001()
        { 
            // arrange, act
            var result = Result.Ok(100);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Should().BeOfType<Result<int>>();
        }

        [Fact(DisplayName = "GR-002: Calling Result.Fail is a failure and not a success.")]
        public void GR002()
        {
            // arrange, act
            var result = Result.Fail<int>("Failure reason");

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Failure reason");
            result.Should().BeOfType<Result<int>>();
       }

        [Fact(DisplayName = "GR-003: Getting value of a failure is an invalid operation.")]
        public void GR003()
        {
            // arrange, act
            var result = Result.Fail<int>("Failure reason");

            int v;
            Action action = () => v = result.Value;

            // assert
            action.Should().Throw<InvalidOperationException>();
        }
    }
}
