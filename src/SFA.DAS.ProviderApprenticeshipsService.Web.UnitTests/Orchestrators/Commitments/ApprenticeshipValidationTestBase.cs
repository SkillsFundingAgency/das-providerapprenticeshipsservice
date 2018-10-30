using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggle;
using FluentValidation.Results;

using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected ICurrentDateTime _currentDateTime;

        protected ApprenticeshipViewModel ValidModel;
        protected CommitmentOrchestrator _orchestrator;
        protected Mock<IMediator> _mockMediator = new Mock<IMediator>();
        protected Mock<IHashingService> _mockHashingService = new Mock<IHashingService>();
        protected Mock<IApprenticeshipCoreValidator> _mockApprenticeshipCoreValidator = new Mock<IApprenticeshipCoreValidator>();
        protected Mock<IApprenticeshipMapper> _mockMapper = new Mock<IApprenticeshipMapper>();
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected Mock<IFeatureToggle> MockFeatureToggleOn;

        private Mock<ApprenticeshipViewModelUniqueUlnValidator> _ulnValidator;

        [SetUp]
        public virtual void SetUp()
        {
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
            _currentDateTime = _currentDateTime ?? new CurrentDateTime();

            _ulnValidator = new Mock<ApprenticeshipViewModelUniqueUlnValidator>();
            _ulnValidator
                .Setup(m => m.ValidateAsyncOverride(It.IsAny<ApprenticeshipViewModel>()))
                .ReturnsAsync(new ValidationResult());

            MockFeatureToggleOn = new Mock<IFeatureToggle>();
            MockFeatureToggleOn.Setup(x => x.FeatureEnabled).Returns(true);
            MockFeatureToggleService = new Mock<IFeatureToggleService>();

            SetUpOrchestrator();
        }

        protected void SetUpOrchestrator()
        {
            _orchestrator = new CommitmentOrchestrator(
                       _mockMediator.Object,
                       _mockHashingService.Object,
                       Mock.Of<IProviderCommitmentsLogger>(),
                       _ulnValidator.Object,
                       Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                       _mockApprenticeshipCoreValidator.Object,
                       _mockMapper.Object,
                       MockFeatureToggleService.Object);
        }

        protected CommitmentListItem GetTestCommitmentOfStatus(long id, RequestStatus requestStatus)
        {
            switch (requestStatus)
            {
                case RequestStatus.NewRequest:
                    return new CommitmentListItem
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        ApprenticeshipCount = 0,
                        CanBeApproved = true,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.None,
                        ProviderLastUpdateInfo = new LastUpdateInfo()
                    };
                case RequestStatus.ReadyForApproval:
                    return new CommitmentListItem
                    {
                        AgreementStatus = AgreementStatus.EmployerAgreed,
                        ApprenticeshipCount = 5,
                        CanBeApproved = true,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.Approve,
                        ProviderLastUpdateInfo = new LastUpdateInfo {EmailAddress = "a@b", Name = "Test"}
                    };
                case RequestStatus.WithEmployerForApproval:
                    return new CommitmentListItem
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        ApprenticeshipCount = 6,
                        CanBeApproved = true,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.EmployerOnly,
                        LastAction = LastAction.Amend,
                        ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "a@b", Name = "Test" }
                    };
                case RequestStatus.ReadyForReview:
                    return new CommitmentListItem
                    {
                        AgreementStatus = AgreementStatus.EmployerAgreed,
                        ApprenticeshipCount = 5,
                        CanBeApproved = false,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.Amend,
                        ProviderLastUpdateInfo = new LastUpdateInfo {EmailAddress = "a@b", Name = "Test"}
                    };
                case RequestStatus.Approved:
                    return new CommitmentListItem
                    {
                        Id = id,
                        Reference = id.ToString(),
                        EditStatus = EditStatus.Both
                    };
                default:
                    Assert.Fail("Add the RequestStatus you require above, or else fix your test!");
                    throw new NotImplementedException();
            }
        }

        protected IEnumerable<CommitmentListItem> GetTestCommitmentsOfStatus(long startId, params RequestStatus[] requestStatuses)
        {
            return requestStatuses.Select(s => GetTestCommitmentOfStatus(startId++, s));
        }
    }
}