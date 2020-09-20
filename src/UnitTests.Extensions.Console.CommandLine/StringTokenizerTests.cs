using System;
using Ave.Extensions.Console.CommandLine;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.Console.CommandLine
{
    public class StringTokenizerTests
    {
        [Fact(DisplayName="ST-001: MoveNext should return false after last char." )]
        public void ST001()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);
            for (var i = 0; i < 4; i++)
            {
                stringTokenizer.MoveNext();
            }

            // act
            var result = stringTokenizer.MoveNext();

            // assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ST-002: MoveNext should throw error after moved after last char.")]
        public void ST002()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);
            for (var i = 0; i < 5; i++)
            {
                stringTokenizer.MoveNext();
            }

            // act
            Action action = () => stringTokenizer.MoveNext();

            // assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "ST-003: CurrentChar should throw exception if pointer moved after last char.")]
        public void ST003()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);
            for (var i = 0; i < 5; i++)
            {
                stringTokenizer.MoveNext();
            }

            // act
            char currentChar;
            Action action = () => currentChar = stringTokenizer.CurrentChar;

            // assert
            action.Should().ThrowExactly<InvalidOperationException>();
        }


        [Fact(DisplayName = "ST-004: CurrentChar should throw exception if pointer is not moved to first position.")]
        public void ST004()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            // act
            char currentChar;
            Action action = () => currentChar = stringTokenizer.CurrentChar;

            // assert
            action.Should().ThrowExactly<InvalidOperationException>();
        }


        [Fact(DisplayName = "ST-005: CurrentToken should be empty if no characters are added.")]
        public void ST005()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);
            for (var i = 0; i < 5; i++)
            {
                stringTokenizer.MoveNext();
            }

            // act
            var currentToken = stringTokenizer.CurrentToken;

            // assert
            currentToken.Should().BeEmpty();
        }

        [Fact(DisplayName = "ST-006: CurrentToken should be equal to source after adding all characters.")]
        public void ST006()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            while( stringTokenizer.MoveNext())
            {
                stringTokenizer.AddCurrentChar();
            }

            // act
            var currentToken = stringTokenizer.CurrentToken;

            // assert
            currentToken.Should().Be(source);
        }


        [Fact(DisplayName = "ST-007: CurrentToken should be equal to added characters.")]
        public void ST007()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            while (stringTokenizer.MoveNext())
            {
                stringTokenizer.AddChar('A');
            }

            // act
            var currentToken = stringTokenizer.CurrentToken;

            // assert
            currentToken.Should().Be("AAAA");
        }

        [Fact(DisplayName = "ST-008: Position is not valid if pointer is not moved to first position.")]
        public void ST008()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            // act
            var result = stringTokenizer.PositionIsValid;

            // assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ST-009: Position is not valid if pointer is moved after last position.")]
        public void ST009()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            while (stringTokenizer.MoveNext())
            {
                stringTokenizer.AddCurrentChar();
            }

            // act
            var result = stringTokenizer.PositionIsValid;

            // assert
            result.Should().BeFalse();
        }



        [Fact(DisplayName = "ST-006: Token should be empty after clearing token.")]
        public void ST010()
        {
            // arrange
            var source = "1234";
            var stringTokenizer = new StringTokenizer(source);

            while (stringTokenizer.MoveNext())
            {
                stringTokenizer.AddCurrentChar();
            }

            // act
            stringTokenizer.ClearToken();
            var currentToken = stringTokenizer.CurrentToken;

            // assert
            currentToken.Should().BeEmpty();
        }
    }
}
