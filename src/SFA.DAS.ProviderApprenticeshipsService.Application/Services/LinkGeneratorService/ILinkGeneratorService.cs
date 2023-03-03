namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService
{
    public interface ILinkGeneratorService
    {
        string TraineeshipLink(string path);
        string CourseManagementLink(string path);
        string ProviderFundingLink(string path);
        string APIManagementLink(string path);
    }
}

