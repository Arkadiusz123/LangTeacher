namespace LangTeacher.Server.Shared
{
    public class ValueResult<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }

        protected ValueResult(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        protected ValueResult(string error)
        {
            IsSuccess = false;
            Value = default;
            Error = error;
        }

        public static ValueResult<T> Success(T value) => new(value);
        public static ValueResult<T> Failure(string error) => new(error);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        protected Result()
        {
            IsSuccess = true;
            Error = null;
        }

        protected Result(string error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static Result Success() => new();
        public static Result Failure(string error) => new(error);
    }
}
