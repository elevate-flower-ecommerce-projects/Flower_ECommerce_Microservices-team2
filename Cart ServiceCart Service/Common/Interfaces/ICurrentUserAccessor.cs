namespace Cart_ServiceCart_Service.Common.Interfaces;

public interface ICurrentUserAccessor
{
    long? UserId { get; }
}
