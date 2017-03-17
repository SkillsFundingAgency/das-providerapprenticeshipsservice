namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public sealed class Navigation
    {
        public Navigation(string previousPageUri, string nextPageUri)
        {
            PreviousPageUrl = previousPageUri;
            NextPageUrl = nextPageUri;
        }

        public string PreviousPageUrl { get; }
        public string NextPageUrl { get; }
        public bool IsStartPage => !string.IsNullOrWhiteSpace(NextPageUrl) && string.IsNullOrWhiteSpace(PreviousPageUrl);
    }
}
