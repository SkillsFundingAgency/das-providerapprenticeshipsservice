namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService;

public interface ILinkGeneratorService
{
    string TraineeshipLink(string path);
    string ProviderFundingLink(string path);
}