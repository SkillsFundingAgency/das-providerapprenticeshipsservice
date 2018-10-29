
namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    // copy pattern for injecting PublicHashingService as per EmployerCommitments
    // (if HashingService also implemented IPublicHashingService we wouldn't need this)
    public class PublicHashingService : HashingService.HashingService, IPublicHashingService
    {
        public PublicHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}
