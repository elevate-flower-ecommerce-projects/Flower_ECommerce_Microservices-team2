namespace Cart_ServiceCart_Service.Common.ResultPattern;

public enum ErrorCode
{
    None = 0,
    InvalidInput = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    ServiceUnavailable = 5,
    InternalError = 6
}
