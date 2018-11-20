namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort
{
    public class LegalEntityViewModel
    {
        public string EmployerAccountPublicHashedId { get; set; }
        public string EmployerAccountName { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityName { get; set; }

        public bool IsComplete => !string.IsNullOrWhiteSpace(EmployerAccountLegalEntityName)
                                  && !string.IsNullOrWhiteSpace(EmployerAccountPublicHashedId)
                                  && !string.IsNullOrWhiteSpace(EmployerAccountLegalEntityPublicHashedId)
                                  && !string.IsNullOrWhiteSpace(EmployerAccountLegalEntityName);
    }
}