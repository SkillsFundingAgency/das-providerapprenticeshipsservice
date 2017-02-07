using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class UploadRowErrorViewModel
    {
        public int RowNumber { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public string Uln { get; set; }

        public string Name { get; set; }

        public string DateOfBirth { get; set; }
    }
}