using Microsoft.AspNetCore.Authorization;

namespace EIskele.Infrastructure.Security;

public class HasPermissionAttribute : AuthorizeAttribute
{
    const string POLICY_PREFIX = "Permission:";

    public HasPermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public string Permission
    {
        get
        {
            if (Policy != null && Policy.StartsWith(POLICY_PREFIX))
            {
                return Policy.Substring(POLICY_PREFIX.Length);
            }
            return default!;
        }
        set
        {
            Policy = $"{POLICY_PREFIX}{value}";
        }
    }
}
