using System;

namespace Ave.Extensions.Console.CommandLine
{
    public class StringTokenizer
    {
        public string PositionAfterLast = "Position is after last character.";
        public string PositionBeforeFirst = "Position is before first character.";

        public const int DefaultInitialTokenCapacity = 64;

        private readonly char[] _source;
        private readonly int _sourceLength;

        private char[] _token;
        private int _tokenLength;

        private int _tokenPosition;
        private int _sourcePosition;

        public StringTokenizer(string source, int initialTokenCapacity)
        {
            _source = source.ToCharArray();
            _sourceLength = _source.Length;

            _token = new char[initialTokenCapacity];
            _tokenLength = _token.Length;
            _sourcePosition = -1;
        }

        public StringTokenizer(string source)
            : this(source, DefaultInitialTokenCapacity)
        {
        }

        public bool MoveNext()
        {
            if (_sourcePosition == _sourceLength)
            {
                throw new InvalidOperationException(PositionAfterLast);
            }
            _sourcePosition++;
            return _sourcePosition < _sourceLength;
        }

        public void ClearToken()
        {
            _tokenPosition = 0;
        }

        public void AddCurrentChar()
        {
            if (_sourcePosition == _sourceLength)
            {
                throw new InvalidOperationException(PositionAfterLast);
            }
            if (_sourcePosition == -1)
            {
                throw new InvalidOperationException(PositionBeforeFirst);
            }
            AddChar(_source[_sourcePosition]);
        }

        public void AddChar(char c)
        {
            if (_tokenPosition == _tokenLength)
            {
                var newTokenLength = _tokenLength * 2;
                Array.Resize(ref _token, newTokenLength);
                _tokenLength = _token.Length;
            }
            _token[_tokenPosition] = c;
            _tokenPosition++;
        }

        public char CurrentChar
        {
            get
            {
                if (_sourcePosition == _sourceLength)
                {
                    throw new InvalidOperationException(PositionAfterLast);
                }
                if (_sourcePosition == -1)
                {
                    throw new InvalidOperationException(PositionBeforeFirst);
                }
                return _source[_sourcePosition];
            }
        }

        public string CurrentToken
        {
            get 
            {
                return _tokenPosition == 0 
                    ? string.Empty  
                    : new string(_token, 0, _tokenPosition);
            }
        }

        public bool PositionIsValid
        {
            get
            {
                return !(_sourcePosition == -1 || _sourcePosition == _sourceLength);
            }
        }
    }
}
