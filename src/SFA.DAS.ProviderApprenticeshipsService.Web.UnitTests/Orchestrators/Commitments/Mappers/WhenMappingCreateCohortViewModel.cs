using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingCreateCohortViewModel
    {
        private CreateCohortMapper _mapper;
        private List<ProviderRelationshipResponse.ProviderRelationship> _source;

        [SetUp]
        public void Arrange()
        {
            _source = new List<ProviderRelationshipResponse.ProviderRelationship>
            {
                new ProviderRelationshipResponse.ProviderRelationship
                {
                    EmployerAccountId = 1,
                    EmployerAccountLegalEntityName = "EmployerAccountLegalEntityName",
                    EmployerAccountLegalEntityPublicHashedId = "EmployerAccountLegalEntityName",
                    EmployerName = "EmployerName",
                    Ukprn = 2
                }
            };

            _mapper = new CreateCohortMapper();
        }

        [Test]
        public void ThenEmployerAccountIdIsMapped()
        {
            var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

            var source = _source.First();
            Assert.AreEqual(source.EmployerAccountId, result.EmployerAccountId);
        }

        [Test]
        public void ThenEmployerAccountLegalEntityNameIsMapped()
        {
            var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

            var source = _source.First();
            Assert.AreEqual(source.EmployerAccountLegalEntityName, result.EmployerAccountLegalEntityName);
        }

        [Test]
        public void ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

            var source = _source.First();
            Assert.AreEqual(source.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public void ThenEmployerAccountIsMapped()
        {
            var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

            var source = _source.First();
            Assert.AreEqual(source.EmployerName, result.EmployerName);
        }

        [Test]
        public void ThenAllProviderRelationshipsAreMapped()
        {
            _source = new List<ProviderRelationshipResponse.ProviderRelationship>
            {
                new ProviderRelationshipResponse.ProviderRelationship(),
                new ProviderRelationshipResponse.ProviderRelationship(),
                new ProviderRelationshipResponse.ProviderRelationship()               
            };

            var result = _mapper.Map(TestHelper.Clone(_source));

            Assert.AreEqual(_source.Count(), result.LegalEntities.Count());
        }
    }
}
