namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService
{
    public interface ILinkGeneratorService
    {
        string ProviderCommitmentsLink(string path);
        string ProviderApprenticeshipServiceLink(string path);
        string ReservationsLink(string path);
        string RecruitLink(string path);
        string TraineeshipLink(string path);
        string RegistrationLink(string path);
        string EmployerDemandLink(string path);
        string CourseManagementLink(string path);
        string ProviderFundingLink(string path);
        string APIManagementLink(string path);
    }
}

