using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetReservationValidation;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.Reservations.Api.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenValidatingApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mockMediator;
        private Mock<IApprovedApprenticeshipValidator> _mockValidator;
        private Mock<IApprenticeshipMapper> _mockMapper;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();

            _mockValidator = new Mock<IApprovedApprenticeshipValidator>();
            _mockValidator.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());
            _mockMapper = new Mock<IApprenticeshipMapper>();

            _mockValidator.Setup(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            _mockValidator.Setup(m => m.ValidateAcademicYear(It.IsAny<CreateApprenticeshipUpdateViewModel>()))
                .Returns(new Dictionary<string, string>());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mockMediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mockMapper.Object,
                _mockValidator.Object,
                Mock.Of<IDataLockMapper>());
        }

        [Test]
        public async Task ShouldValidate()
        {
            var viewModel = new ApprenticeshipViewModel();
            var updateModel = new CreateApprenticeshipUpdateViewModel();

            _mockMapper
                .Setup(m => m.MapApprenticeship(It.IsAny<ApprenticeshipViewModel>()))
                .ReturnsAsync(new Apprenticeship{ReservationId = Guid.NewGuid(), StartDate = DateTime.Today});

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOverlappingApprenticeshipsQueryResponse { Overlaps = new List<ApprenticeshipOverlapValidationResult>() });

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetReservationValidationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetReservationValidationResponse{Data = new ReservationValidationResult(new ReservationValidationError[0])});

            await _orchestrator.ValidateEditApprenticeship(viewModel, updateModel);

            _mockMediator.Verify(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "Should call");
            _mockMediator.Verify(m => m.Send(It.IsAny<GetReservationValidationRequest>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "Should call");
            _mockValidator.Verify(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()), Times.Once, failMessage: "Should verify overlapping apprenticeship");
            _mockValidator.Verify(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate apprenticeship");
            _mockValidator.Verify(m => m.ValidateAcademicYear(It.IsAny<CreateApprenticeshipUpdateViewModel>()), Times.Once, failMessage: "Should validate academic year");
        }

        [Test]
        public async Task ShouldAppendErrorsTogether()
        {
            var viewModel = new ApprenticeshipViewModel();
            var updateModel = new CreateApprenticeshipUpdateViewModel();

            _mockMapper
                .Setup(m => m.MapApprenticeship(It.IsAny<ApprenticeshipViewModel>()))
                .ReturnsAsync(new Apprenticeship{ReservationId = Guid.NewGuid(), StartDate = DateTime.Today});

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOverlappingApprenticeshipsQueryResponse { Overlaps = new List<ApprenticeshipOverlapValidationResult>() });

            _mockValidator
                .Setup(v => v.ValidateToDictionary(viewModel))
                .Returns(BuildDictionary("VTD1=error1", "VTD2=error2"));

            _mockValidator
                .Setup(v => v.ValidateAcademicYear(updateModel))
                .Returns(BuildDictionary("VAY1=error3", "VAY2=error4"));

            _mockValidator
                .Setup(v => v.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(BuildDictionary("OLE1=error7", "OLE2=error8"));

            _mockMediator
                .Setup(m => m.Send(It.IsAny<GetReservationValidationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetReservationValidationResponse
                {
                    Data = new ReservationValidationResult(new[]
                    {
                        new ReservationValidationError("RVE1","error9"),
                        new ReservationValidationError("RVE2","error10")
                    })
                });

            var errors = await _orchestrator.ValidateEditApprenticeship(viewModel, updateModel);

            TickoffError(errors, "VTD1", "error1");
            TickoffError(errors, "VTD2", "error2");
            TickoffError(errors, "VAY1", "error3");
            TickoffError(errors, "VAY2", "error4");
            TickoffError(errors, "OLE1", "error7");
            TickoffError(errors, "OLE2", "error8");
            TickoffError(errors, "RVE1", "error9");
            TickoffError(errors, "RVE2", "error10");

            Assert.AreEqual(0, errors.Count, "Additional unexpected errors were returned by the validator");
        }


        private Dictionary<string, string> BuildDictionary(params string[] keyvalues)
        {
            var result = new Dictionary<string, string>();

            foreach (var keyvalue in keyvalues)
            {
                var s = keyvalue.Split('=');
                if (s.Length != 2)
                {
                    throw new InvalidOperationException(
                        $"The string '{keyvalue}' should be formatted as <name>=<value>");
                }
                result.Add(s[0].Trim(), s[1].Trim());
            }

            return result;
        }

        private void TickoffError(Dictionary<string, string> errors, string key, string value)
        {
            Assert.IsTrue(errors.ContainsKey(key), $"Error with key {key} was not in the returned list");

            Assert.AreEqual(value, errors[key], $"The message associated with error {key} is incorrect");

            errors.Remove(key);
        }
    }
}
