namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public interface ITrainingProgramme
    {
        string Id { get; set; }

        string Title { get; set; }

        int Level { get; set; }
        int MaxFunding { get; set; }
    }
}
