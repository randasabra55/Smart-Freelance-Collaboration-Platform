using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Infrastructure.Common.Responses
{
    #region Generic wiht data

    public class Result<T> : Result
    {
        public T? Data { get; }

        public Result(bool isSuccess, Error error, T? data) : base(isSuccess, error)
        {
            Data = data;
        }
        public static implicit operator Result<T>(T data) => Success(data);
        public static implicit operator Result<T>(ErrorCode errorCode) =>
            Failure<T>(new Error(errorCode));

        public static implicit operator Result<T>(Error error) =>
            Failure<T>(error);
    }

    #endregion

    #region Base

    public class Result
    {
        public Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<T> Success<T>(T data) => new(true, Error.None, data);

        public static Result<T> Failure<T>(Error error) => new(false, error, default);

        public static implicit operator Result(ErrorCode errorCode) =>
            Failure(new Error(errorCode));

        public static implicit operator Result(Error error) =>
            Failure(error);
    }

    #endregion
}
