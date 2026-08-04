using System.ComponentModel;

namespace AuthService.Common.Enums
{
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

        [Description("Client closed the request before the server could respond.")]
        ClientClosedRequest = 499,

        [Description("Unauthorized access.")]
        Unauthorized = 401,

        [Description("Not Found")]
        BadRequest = 404,

        [Description("An unknown error occurred.")]
        UnKnown = 500
    }
}
