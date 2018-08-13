using System;
using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();
        protected Mock<IUlnValidator> MockUlnValidator = new Mock<IUlnValidator>();
        protected Mock<IMediator> MockMediator;
        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            MockMediator = new Mock<IMediator>();
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse
                {
                    TrainingProgrammes = new List<ITrainingProgramme>
                    {
                        new Standard
                        {
                            Id = "TESTCOURSE",
                            EffectiveFrom = new DateTime(2018, 5, 1),
                            EffectiveTo = new DateTime(2018, 7, 1)
                        }
                    }
                });

            CurrentDateTime.Setup(x => x.Now).Returns(DateTime.Now.AddMonths(6));
            Validator = new ApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(new AcademicYearDateProvider(CurrentDateTime.Object)),
                CurrentDateTime.Object,
                new AcademicYearDateProvider(CurrentDateTime.Object),
                MockUlnValidator.Object,
                MockMediator.Object);
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
