using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class ValidatingContractFeed
    {
        [TestCase("", "", "", "")]
        [TestCase("Abba", "Levy", "Approved", "Approved")]
        [TestCase("Contract", "Abba", "Approved", "Approved")]
        [TestCase("Contract", "Levy", "Abba", "Approved")]
        [TestCase("Contract", "Levy", "Approved", "Abba")]
        public void ShouldNotValidate(string hierarchyType, string fundingTypeCode, string parentStatus, string status)
        {
            var sut = new ContractFeedEventValidator();
            var model = new ContractFeedEvent
                            {
                                HierarchyType = hierarchyType,
                                FundingTypeCode = fundingTypeCode,
                                ParentStatus = parentStatus,
                                Status = status
                            };
            sut.Validate(model).Should().BeFalse();
        }

        [TestCase("Contract", "Levy", "Approved", "Approved")]
        [TestCase("contract", "levy", "approved", "approved")]
        public void ShouldValidate(string hierarchyType, string fundingTypeCode, string parentStatus, string status)
        {
            var sut = new ContractFeedEventValidator();
            var model = new ContractFeedEvent
            {
                HierarchyType = hierarchyType,
                FundingTypeCode = fundingTypeCode,
                ParentStatus = parentStatus,
                Status = status
            };
            sut.Validate(model).Should().BeTrue();
        }
    }
}
