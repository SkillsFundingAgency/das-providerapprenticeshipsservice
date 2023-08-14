namespace SFA.DAS.PAS.Account.Application.Queries.GetUser;

public class GetUserQueryResponse
{
    public string? UserRef { get; set; }
    public string? Name { get; set; }
    public string? EmailAddress { get; set; }
    public bool IsSuperUser { get; set; }
}