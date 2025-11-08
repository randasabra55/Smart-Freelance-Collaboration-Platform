using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Infrastructure.Common.Responses
{
    public class Error
    {
        public Error(ErrorCode code)
        {
            Code = code;
            Message = code.ToString();
        }

        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
        public Error(ErrorCode code, string message, object errorDetails)
        {
            Code = code;
            Message = message;
            ObjectErrorDetails = errorDetails;
        }
        public Error(ErrorCode code, IEnumerable<string> errosDetails)
        {
            Code = code;
            ErrorDetails = errosDetails;
        }



        public ErrorCode Code { get; }
        public string? Message { get; }
        public object? ObjectErrorDetails { get; }
        public IEnumerable<string>? ErrorDetails { get; }

        public static Error None => new(ErrorCode.None);

        public ErrorCode ErrorCode { get; }

        public static implicit operator Error(ErrorCode code) => new(code);
    }
}



