namespace Cart_ServiceCart_Service.Common.ResultPattern;

public record RequestResult<T>(T Data, bool IsSuccess, string Message, ErrorCode ErrorCode)
{
    public static RequestResult<T> Success(T data, string message = "") =>
        new(data, true, message, ErrorCode.None);

    public static RequestResult<T> Failure(ErrorCode errorCode, string message = "") =>
        new(default!, false, message, errorCode);
}
