using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface IIdamsEmailServiceWrapper
{
    Task<List<string>> GetEmailsAsync(long ukprn, string identities);
}