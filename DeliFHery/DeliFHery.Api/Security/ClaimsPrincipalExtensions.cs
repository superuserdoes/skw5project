using System.Security.Claims;

namespace DeliFHery.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool HasScope(this ClaimsPrincipal principal, string scope)
    {
        if (string.IsNullOrWhiteSpace(scope))
        {
            return true;
        }

        var scopeClaims = principal.Claims
            .Where(claim => claim.Type is "scope" or "scp");

        foreach (var claim in scopeClaims)
        {
            var values = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (values.Contains(scope, StringComparer.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
