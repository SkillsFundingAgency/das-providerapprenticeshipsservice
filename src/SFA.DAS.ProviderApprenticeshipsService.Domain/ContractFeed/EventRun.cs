namespace SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed
{
    public class EventRun
    {
        public int NewLastReadPageNumber { get; set; }

        public long ExecutionTimeMs { get; set; }

        public int ContractCount { get; set; }

        public int PagesRead { get; set; }

    }
}