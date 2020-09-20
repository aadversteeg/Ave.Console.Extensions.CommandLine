using System;

namespace Ave.Extensions.Console.CommandLine
{
    public class Result
    {
        private string _error;

        protected Result(string error)
        {
            _error = error;
        }

        public static Result Ok()
        {
            return new Result(null);
        }

        public static Result Fail(string error)
        {
            return new Result(error);
        }

        public static Result<T> Ok<T>(T value)
        {
            return Result<T>.Ok(value);
        }

        public static Result<T> Fail<T>(string error)
        {
            return Result<T>.Fail(error);
        }

        public bool IsSuccess => _error == null;
        public bool IsFailure => _error != null;

        public string Error
        {
            get
            {
                if (!IsFailure)
                {
                    throw new InvalidOperationException();
                }
                return _error;
            }
        }
    }


    public class Result<T> : Result
    {
        private T _value;

        private Result(T value, string error)
            :base(error)
        {
            _value = value;
        }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(value, null);
        }
        
        public new static Result<T> Fail(string error)
        {
            return new Result<T>(default(T), error);
        }

        public T Value {
            get {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("");
                }
                return _value;
            }
        }
    }
}
