namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

public class AgreementsViewModel
{
    public IEnumerable<CommitmentAgreement> CommitmentAgreements { get; set; }
    public string SearchText { get; set; }
    public IEnumerable<string> AllProviderOrganisationNames { get; set; }
}