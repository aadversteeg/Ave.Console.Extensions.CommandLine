using System;
using Ave.Extensions.Console.CommandLine;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class ResultTests
    {
        [Fact(DisplayName = "R-001: Calling Result.Ok is a success and not a failure.")]
        public void R001()
        { 
            // arrange, act
            var result = Result.Ok();

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Should().BeOfType<Result>();
        }

        [Fact(DisplayName = "R-002: Calling Result.Fail is a failure and not a success.")]
        public void R002()
        {
            // arrange, act
            var result = Result.Fail("Failure reason");

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Failure reason");
            result.Should().BeOfType<Result>();
       }

        [Fact(DisplayName = "R-003: Getting error of a success is an invalid operation.")]
        public void R003()
        {
            // arrange, act
            var result = Result.Ok();

            string error;
            Action action = () => error = result.Error;

            // assert
            action.Should().Throw<InvalidOperationException>();
        }
    }
}
