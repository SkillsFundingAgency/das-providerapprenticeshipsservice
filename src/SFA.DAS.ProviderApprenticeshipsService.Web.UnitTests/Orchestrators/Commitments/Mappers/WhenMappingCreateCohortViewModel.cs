using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingCreateCohortViewModel
    {
        private CreateCohortMapper _mapper;
        private List<RelationshipDto> _source;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns("EmployerAccountPublicHashedId");

            _source = new List<RelationshipDto>
            {
                new RelationshipDto
                {
                    EmployerAccountId = 1,
                    EmployerAccountPublicHashedId = "EmployerAccountPublicHashedId",
                    EmployerAccountLegalEntityName = "EmployerAccountLegalEntityName",
                    EmployerAccountLegalEntityPublicHashedId = "EmployerAccountLegalEntityName",
                    EmployerAccountName = "EmployerName",
                    Ukprn = 2
                }
            };

            _mapper = new CreateCohortMapper(_hashingService.Object);
        }

        [Test]
        public void ThenEmployerAccountIdIsMapped()
        {
            var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();
            Assert.AreEqual("EmployerAccountPublicHashedId", result.EmployerAccountPublicHashedId);
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
            Assert.AreEqual(source.EmployerAccountName, result.EmployerAccountName);
        }

        [Test]
        public void ThenAllProviderRelationshipsAreMapped()
        {
            _source = new List<RelationshipDto>
            {
                new RelationshipDto(),
                new RelationshipDto(),
                new RelationshipDto()               
            };

            var result = _mapper.Map(TestHelper.Clone(_source));

            Assert.AreEqual(_source.Count(), result.LegalEntities.Count());
        }
    }
}
