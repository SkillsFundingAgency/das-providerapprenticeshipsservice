using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Mappers.SelectEmployer
{
    [TestFixture]
    public class WhenIMapToChooseEmployerViewModel
    {
        private SelectEmployerMapper _selectEmployerMapper;
        private List<AccountProviderLegalEntityDto> _listOfLegalEntities;
        private AccountProviderLegalEntityDto _legalEntity1;
        private AccountProviderLegalEntityDto _legalEntity2;

        [SetUp]
        public void Arrange()
        {
            _legalEntity1 = new AccountProviderLegalEntityDto
            {
                AccountPublicHashedId = "PH1",
                AccountName = "AN",
                AccountLegalEntityPublicHashedId = "ALEPH1",
                AccountLegalEntityName = "LE1"
            };

            _legalEntity2 = new AccountProviderLegalEntityDto
            {
                AccountPublicHashedId = "PH2",
                AccountName = "AN",
                AccountLegalEntityPublicHashedId = "ALEPH2",
                AccountLegalEntityName = "LE2"
            };


            _listOfLegalEntities = new List<AccountProviderLegalEntityDto>
            {
                _legalEntity1, _legalEntity2
            };

            _selectEmployerMapper = new SelectEmployerMapper();
        }

        [TestCase(EmployerSelectionAction.CreateCohort)]
        [TestCase(EmployerSelectionAction.CreateReservation)]
        public void ThenLegalEntitiesAreMappedToViewModel(EmployerSelectionAction action)
        {
            var result = _selectEmployerMapper.Map(_listOfLegalEntities, action);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.LegalEntities.Count());
            Assert.AreEqual(_legalEntity1.AccountPublicHashedId, result.LegalEntities.First().EmployerAccountPublicHashedId);
            Assert.AreEqual(_legalEntity1.AccountName, result.LegalEntities.First().EmployerAccountName);
            Assert.AreEqual(_legalEntity1.AccountLegalEntityPublicHashedId, result.LegalEntities.First().EmployerAccountLegalEntityPublicHashedId);
            Assert.AreEqual(_legalEntity1.AccountLegalEntityName, result.LegalEntities.First().EmployerAccountLegalEntityName);
            Assert.AreEqual(action, result.EmployerSelectionAction);
        }
    }
}
