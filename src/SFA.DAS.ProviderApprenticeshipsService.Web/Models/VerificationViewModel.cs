using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class VerificationViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public bool? ConfirmProvisionOfTrainingForOrganisation { get; set; }
    }
}