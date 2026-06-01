using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EIskele.Infrastructure.Security;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options) : base(options) { }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        const string POLICY_PREFIX = "Permission:";

        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring(POLICY_PREFIX.Length);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
            return policy;
        }

        return await base.GetPolicyAsync(policyName);
    }
}
