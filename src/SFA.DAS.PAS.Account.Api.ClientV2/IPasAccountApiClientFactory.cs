namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    public interface IPasAccountApiClientFactory
    {
        IPasAccountApiClient CreateClient();
    }
}