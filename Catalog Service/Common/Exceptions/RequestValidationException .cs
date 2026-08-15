namespace Catalog_Service.Common.Exceptions
{
    public class RequestValidationException : Exception
    {
        public string Message { get; set; }
        public RequestValidationException(string message) : base(message)
        {
            Message = message;
        }
    }
}
