using System.Collections.Generic;

namespace SFA.DAS.PAS.Jobs.ApiModels;

public class GetAllProvidersResponse
{
    public List<Provider> Organisations { get; set; } = [];
}
