using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService, IAccountLegalEntityPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}
