namespace Cart_ServiceCart_Service.Common.ResultPattern;

public record EndpointResponse<T>(T Data, bool IsSuccess, string Message, ErrorCode ErrorCode)
{
    public static EndpointResponse<T> Success(T data, string message = "") =>
        new(data, true, message, ErrorCode.None);

    public static EndpointResponse<T> Failure(ErrorCode errorCode, string message = "") =>
        new(default!, false, message, errorCode);
}
