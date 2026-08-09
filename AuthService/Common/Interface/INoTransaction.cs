namespace AuthService.Common.Interface
{
    /// <summary>
    /// Marker interface to indicate that a command should not be wrapped in a database transaction
    /// by the TransactionMiddleware.
    /// Implement this on any ICommand that must run without an ambient transaction.
    /// </summary>
    public interface INoTransaction
    {
    }
}
