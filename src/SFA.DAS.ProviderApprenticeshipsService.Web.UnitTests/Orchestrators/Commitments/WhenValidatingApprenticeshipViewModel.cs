using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModel : ApprenticeshipValidationTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            _currentDateTime = new CurrentDateTime(new DateTime(DateTime.Now.Year, 11, 01));
            _mockApprenticeshipCoreValidator.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());

            base.SetUp();
        }

        [Test]
        public async Task ShouldAllowUpdateOfApprenticeshipWithStartDateInLastAcademicYear()
        {
            ValidModel.StartDate = new DateTimeViewModel(1, 08, DateTime.Now.Year);
            ValidModel.EndDate = new DateTimeViewModel(1, 12, DateTime.Now.Year);
            ValidModel.ULN = "1238894231";
            var result = await _orchestrator.ValidateApprenticeship(ValidModel);

            result.Keys.Should().NotContain($"{nameof(ValidModel.StartDate)}");
        }
    }
}
