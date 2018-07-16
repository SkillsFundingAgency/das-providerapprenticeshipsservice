namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse
{
    public class Standard : ITrainingProgramme
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
    }
}