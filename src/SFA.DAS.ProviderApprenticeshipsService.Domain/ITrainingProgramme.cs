namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public interface ITrainingProgramme
    {
        int Id { get; set; }

        string Title { get; set; }

        int Level { get; set; }
    }
}
