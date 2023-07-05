using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

public class Provider
{
    public long Ukprn { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? UpdatedFromIDAMS { get; set; }
}