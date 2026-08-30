using System.ComponentModel;
namespace Cart_ServiceCart_Service.Common.Enums;

public enum ErrorCode
{
    [Description("No error.")]
    None = 200,

    [Description("Invalid input data.")]
    InvalidInput = 400,

    [Description("Product not found.")]
    ProductNotFound = 1000,

    [Description("Category not found.")]
    CategoryNotFound = 2000,

    [Description("Category is no longer available.")]
    CategoryNoLongerAvailable = 2001,

    [Description("Client closed the request before the server could respond.")]
    ClientClosedRequest = 499,

    [Description("Unauthorized access.")]
    Unauthorized = 401,

    [Description("Not Found")]
    BadRequest = 404,

    [Description("Internal Server Error")]
    InternalServerError = 500,

    [Description("Failed to generate JWT token.")]
    FailWhileJwtGenerateToken = 1,

    [Description("Failed to generate refresh token.")]
    FailWhileGenerateRefreshToken = 2,

    [Description("Failed to generate token.")]
    FailWhileGenerateToken = 3,

    [Description("Invalid credentials.")]
    InvalidCredentials = 501,

    [Description("Invalid request.")]
    InvalidRequest = 502,

    [Description("Invalid token.")]
    InvalidToken = 503,

    [Description("Email not confirmed.")]
    EmailNotConfirmed = 504,

    [Description("Current password is incorrect.")]
    InvalidCurrentPassword = 505,

    [Description("New password and confirmation do not match.")]
    PasswordMismatch = 506,

    [Description("New password must be different from the current password.")]
    PasswordReuse = 507,

    [Description("User not found.")]
    UserNotFound = 1008,

    [Description("Resource not found.")]
    NotFound = 404,

    [Description("Conflict.")]
    Conflict = 409,

    [Description("Service unavailable.")]
    ServiceUnavailable = 503,

    [Description("Internal error.")]
    InternalError = 500,

    [Description("An unknown error occurred.")]
    UnKnown = 500
}
