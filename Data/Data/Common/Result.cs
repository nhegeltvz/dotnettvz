using System.Diagnostics.CodeAnalysis;

namespace Data.Data.Common
{
    public record Result
    {
        public bool IsSuccess { get; }
        public IReadOnlyCollection<Error>? Errors { get; }

        protected Result(bool isSuccess, IReadOnlyCollection<Error>? errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new(true, errors: null);
        public static Result Failure(IReadOnlyCollection<Error> errors) => new(false, errors);
        public static Result Failure(Error error) => new(false, [error]);


    }

    public record Result<T> : Result
    {
        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        public T? Value { get; }

        private Result(T value) : base(true, errors: null) { Value = value; }
        private Result(IReadOnlyCollection<Error> errors) : base(false, errors: errors) { }
        private Result(bool success, IReadOnlyCollection<Error>? errors, T? value)
        : base(success, errors)
        => Value = value;


        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(IReadOnlyCollection<Error> errors) => new(errors);
        public static Result<T> Failure(Error error) => new([error]);
        public static Result<T> FromResult(Result result)
     => new Result<T>(result.IsSuccess, result.Errors, default);

    }
}
