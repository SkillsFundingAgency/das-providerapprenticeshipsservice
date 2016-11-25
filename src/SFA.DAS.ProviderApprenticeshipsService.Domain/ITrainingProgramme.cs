namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public interface ITrainingProgramme
    {
        string Id { get; set; }

        string Title { get; set; }

        int Level { get; set; }
    }
}
