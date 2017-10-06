using System;

using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenMappingApprenticeshipUpdate
    {
        private IApprenticeshipMapper _mapper;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>()))
                .Returns("hashed");

            _mapper = new ApprenticeshipMapper(
               _hashingService.Object,
                Mock.Of<IMediator>(),
                new CurrentDateTime(),
                 Mock.Of<ILog>(),
                 Mock.Of<IAcademicYearValidator>());
        }

        [Test]
        public void ThenAllChangedFieldsAreMapped()
        {
            //Arrange
            var original = new Apprenticeship();
            var update = new ApprenticeshipUpdate
            {
                ApprenticeshipId = 1,
                FirstName = "FirstName",
                LastName = "LastName",
                DateOfBirth = new DateTime(2000,1,1),
                ULN = "2222222222",
                TrainingType = TrainingType.Framework,
                TrainingName = "TrainingName",
                TrainingCode = "TrainingCode",
                Cost = 1000,
                StartDate = new DateTime(2020,1,1),
                EndDate = new DateTime(2021,1,1),
                ProviderRef = "ProviderRef",
                EmployerRef = "EmployerRef"
            };           

            var result = _mapper.MapApprenticeshipUpdateViewModel<ApprenticeshipUpdateViewModel>(original, update);

            Assert.AreEqual("hashed", result.HashedApprenticeshipId);
            Assert.AreEqual("FirstName", result.FirstName);
            Assert.AreEqual("LastName", result.LastName);
            Assert.AreEqual(new DateTime(2000,1,1), result.DateOfBirth.DateTime);
            Assert.AreEqual("2222222222", result.ULN);
            Assert.AreEqual(Domain.TrainingType.Framework, result.TrainingType);
            Assert.AreEqual("TrainingName", result.TrainingName);
            Assert.AreEqual("TrainingCode", result.TrainingCode);
            Assert.AreEqual("1000", result.Cost);
            Assert.AreEqual(new DateTime(2020, 1, 1), result.StartDate.DateTime);
            Assert.AreEqual(new DateTime(2021, 1, 1), result.EndDate.DateTime);
            Assert.AreEqual("ProviderRef", result.ProviderRef);
            Assert.AreEqual("EmployerRef", result.EmployerRef);
            Assert.AreEqual(original, result.OriginalApprenticeship);
        }
    }
}
