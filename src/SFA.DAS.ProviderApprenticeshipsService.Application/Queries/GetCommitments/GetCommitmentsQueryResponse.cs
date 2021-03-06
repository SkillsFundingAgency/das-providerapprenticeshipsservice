﻿using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments
{
    public class GetCommitmentsQueryResponse
    {
        public List<CommitmentListItem> Commitments { get; set; }
    }
}