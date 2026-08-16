using AuthService.Common.Enums;
using AuthService.Common.Helpers;

namespace AuthService.Common.ResultPattern
{
    public record EndpointResponse<T>(T Data, bool IsSuccess, string Message, ErrorCode ErrorCode)
    {
        public static EndpointResponse<T> Success(T data, string message = "")
        {
            return new EndpointResponse<T>(data, true, message, ErrorCode.None);
        }
        public static EndpointResponse<T> Failure(ErrorCode errorCode, string message = null)
        {
            if (message is null)
            {
                message = errorCode.GetDescription();
            }
            return new EndpointResponse<T>(default, false, message, errorCode);
        }
    }
}
