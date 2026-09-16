using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PAS.Jobs.Configuration;

[ExcludeFromCodeCoverage]
public class RoatpApiConfiguration
{
    public string ApiBaseUrl { get; set; }
    public string IdentifierUri { get; set; }
}
