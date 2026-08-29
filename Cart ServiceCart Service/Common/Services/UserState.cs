using System.Collections.ObjectModel;
using System.Security.Claims;
namespace Cart_ServiceCart_Service.Common.Services;
/// <summary>
/// Per-request snapshot of the authenticated user, projected once from the JWT claims by
/// <see cref="Middelwares.UserStateMiddleware"/>. Registered scoped, so every component
/// resolved within a request sees the same instance.
///
/// Three things keep this read-only for application code:
///   1. Consumers depend on <see cref="IUserState"/>, which has no writable surface.
///   2. Populate is an *explicit* <see cref="IUserStateWriter"/> implementation, so it is not
///      callable from a UserState reference — you must hold the writer interface.
///   3. It is write-once: a second call throws rather than silently swapping the identity.
/// </summary>
public sealed class UserState : IUserState, IUserStateWriter
{
    private bool _isPopulated;

    public bool IsAuthenticated { get; private set; }

    /// <summary>Snowflake user id from the NameIdentifier claim; 0 when anonymous.</summary>
    public long UserId { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    // ReadOnlyCollection rather than the bare array — an IReadOnlyList<string> backed by
    // string[] can be cast back to string[] and mutated; this wrapper cannot.
    public IReadOnlyList<string> Roles { get; private set; } = ReadOnlyCollection<string>.Empty;

    public bool IsInRole(string role) =>
        Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Projects the claims principal onto this instance. Anonymous requests leave the defaults
    /// in place rather than throwing — authorization is the pipeline's job, not this type's.
    /// Explicit implementation: reachable only via <see cref="IUserStateWriter"/>.
    /// </summary>
    void IUserStateWriter.Populate(ClaimsPrincipal? principal)
    {
        if (_isPopulated)
        {
            throw new InvalidOperationException(
                "UserState has already been populated for this request and cannot be reassigned.");
        }

        _isPopulated = true;

        if (principal?.Identity?.IsAuthenticated != true)
            return;

        IsAuthenticated = true;

        // AuthService issues these in GenerateJwtTokenHandler.
        UserId   = long.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        FullName = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        Email    = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        Roles    = new ReadOnlyCollection<string>(
                       principal.FindAll(ClaimTypes.Role)
                                .Select(claim => claim.Value)
                                .ToArray());
    }
}
