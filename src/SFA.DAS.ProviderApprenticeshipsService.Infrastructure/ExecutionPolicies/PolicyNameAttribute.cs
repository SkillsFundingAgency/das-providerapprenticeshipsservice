using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies;

[AttributeUsage(AttributeTargets.Class)]
public class PolicyNameAttribute : Attribute
{
    public PolicyNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}