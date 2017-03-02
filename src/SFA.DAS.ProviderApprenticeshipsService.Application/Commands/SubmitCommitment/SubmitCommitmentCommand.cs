﻿using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    using SFA.DAS.Commitments.Api.Types;

    public class SubmitCommitmentCommand : IAsyncRequest
    {
        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public long CommitmentId { get; set; }

        public string Message { get; set; }

        public bool CreateTask { get; set; }

        public LastAction LastAction { get; set; }

        public string UserDisplayName { get; set; }

        public string UserEmailAddress { get; set; }

        public string UserId { get; set; }
    }
}