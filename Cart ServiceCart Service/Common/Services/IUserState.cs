using System.Security.Claims;
namespace Cart_ServiceCart_Service.Common.Services;
/// <summary>
/// Read-only view of the authenticated user for the current request. This is what handlers,
/// services and the DbContext depend on — it exposes no way to alter the identity.
/// </summary>
public interface IUserState
{
    bool IsAuthenticated { get; }

    /// <summary>Snowflake user id from the NameIdentifier claim; 0 when anonymous.</summary>
    long UserId { get; }

    string FullName { get; }

    string Email { get; }

    IReadOnlyList<string> Roles { get; }

    bool IsInRole(string role);
}

/// <summary>
/// Write side of <see cref="IUserState"/>. Only <see cref="Middelwares.UserStateMiddleware"/>
/// takes a dependency on this, and <see cref="UserState"/> implements it explicitly — so
/// Populate is not reachable from a UserState reference, only through this interface.
/// </summary>
public interface IUserStateWriter
{
    void Populate(ClaimsPrincipal? principal);
}
