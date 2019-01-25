using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    //[TestFixture]
    //public class WhenMappingCreateCohortViewModel
    //{
    //    private SelectEmployerMapper _mapper;
    //    private List<AccountProviderLegalEntityDto> _source;
    //    private Mock<IHashingService> _hashingService;

    //    [SetUp]
    //    public void Arrange()
    //    {
    //        _hashingService = new Mock<IHashingService>();
    //        _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns("EmployerAccountPublicHashedId");

    //        _source = new List<AccountProviderLegalEntityDto>
    //        {
    //            new AccountProviderLegalEntityDto
    //            {
    //                AccountId = 1,
    //                AccountPublicHashedId = "EmployerAccountPublicHashedId",
    //                AccountLegalEntityName = "EmployerAccountLegalEntityName",
    //                AccountLegalEntityPublicHashedId = "EmployerAccountLegalEntityName",
    //                AccountName = "EmployerName"
    //            }
    //        };

    //        _mapper = new SelectEmployerMapper(_hashingService.Object);
    //    }

    //    [Test]
    //    public void ThenEmployerAccountIdIsMapped()
    //    {
    //        var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();
    //        Assert.AreEqual("EmployerAccountPublicHashedId", result.EmployerAccountPublicHashedId);
    //    }

    //    [Test]
    //    public void ThenEmployerAccountLegalEntityNameIsMapped()
    //    {
    //        var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

    //        var source = _source.First();
    //        Assert.AreEqual(source.AccountLegalEntityName, result.EmployerAccountLegalEntityName);
    //    }

    //    [Test]
    //    public void ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
    //    {
    //        var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

    //        var source = _source.First();
    //        Assert.AreEqual(source.AccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
    //    }

    //    [Test]
    //    public void ThenEmployerAccountIsMapped()
    //    {
    //        var result = _mapper.Map(TestHelper.Clone(_source)).LegalEntities.First();

    //        var source = _source.First();
    //        Assert.AreEqual(source.AccountName, result.EmployerAccountName);
    //    }

    //    [Test]
    //    public void ThenAllProviderRelationshipsAreMapped()
    //    {
    //        _source = new List<AccountProviderLegalEntityDto>
    //        {
    //            new AccountProviderLegalEntityDto(),
    //            new AccountProviderLegalEntityDto(),
    //            new AccountProviderLegalEntityDto()               
    //        };

    //        var result = _mapper.Map(TestHelper.Clone(_source));

    //        Assert.AreEqual(_source.Count(), result.LegalEntities.Count());
    //    }
    //}
}
