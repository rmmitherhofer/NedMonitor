using System.Security.Claims;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Extension methods to simplify working with ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Claim key for tenant identifier.</summary>
    public const string TENANT_ID = "tenantId";
    /// <summary>Claim key for user account.</summary>
    public const string USER_ACCOUNT= "userAccount";
    /// <summary>Claim key for user account code.</summary>
    public const string USER_ACCOUNT_CODE = "userAccountCode";
    /// <summary>Claim key for user document.</summary>
    public const string USER_DOCUMENT = "userDocument";
    /// <summary>Claim key for subject identifier.</summary>
    public const string SUB = "sub";

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>True if authenticated; otherwise false.</returns>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.Identity?.IsAuthenticated ?? false;
    }
    /// <summary>
    /// Gets the value of a claim by key.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    /// <returns>Claim value or null if not found.</returns>
    public static string? GetClaim(this ClaimsPrincipal user, string key)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.FindFirst(key)?.Value;
    }
    /// <summary>
    /// Gets the user's identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>User Id claim value or subject claim if user Id is absent.</returns>
    public static string? GetId(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.NameIdentifier) ?? user.GetClaim(SUB);
    /// <summary>
    /// Gets the user's name claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Name claim value or the Identity's Name if claim is missing.</returns>
    public static string? GetName(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.Name) ?? user.Identity?.Name;
    /// <summary>
    /// Gets the user's email claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Email claim value or null.</returns>
    public static string? GetEmail(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.Email);
    /// <summary>
    /// Gets the list of role claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>IEnumerable of role claim values.</returns>
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
    /// <summary>
    /// Gets the authentication type.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Authentication type or null.</returns>
    public static string? GetAuthenticationType(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user?.Identity?.AuthenticationType;
    }
    /// <summary>
    /// Gets the tenant identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Tenant Id claim value or null.</returns>
    public static string? GetTenantId(this ClaimsPrincipal user) => user.GetClaim(TENANT_ID);
    /// <summary>
    /// Gets the user document claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetDocument(this ClaimsPrincipal user) => user.GetClaim(USER_DOCUMENT);
    /// <summary>
    /// Gets the user account claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetAccount(this ClaimsPrincipal user) => user.GetClaim(USER_ACCOUNT);
    /// <summary>
    /// Gets the user account code claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetAccountCode(this ClaimsPrincipal user) => user.GetClaim(USER_ACCOUNT_CODE);
   


}
