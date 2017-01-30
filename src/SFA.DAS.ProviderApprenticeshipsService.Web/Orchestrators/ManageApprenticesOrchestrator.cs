using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

using StructureMap.Diagnostics;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class ManageApprenticesOrchestrator
    {
        private readonly IMediator _mediator;

        public ManageApprenticesOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ManageApprenticeshipsViewModel> GetApprenticeships(long providerId)
        {
            var data = await _mediator.SendAsync(new GetAllApprenticesRequest { ProviderId = providerId });
            var apprenticeships = data.Apprenticeships.Select(MapFrom).ToList();

            return new ManageApprenticeshipsViewModel
                        {
                            ProviderId = providerId,
                            Apprenticeships = await Task.WhenAll(apprenticeships)
                        };
        }

        public async Task<ApprenticeshipDetailsViewModel> GetApprenticeship(long providerId, long appenticeshipId)
        {
            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { ProviderId = providerId, AppenticeshipId = appenticeshipId });
            return await MapFrom(data.Apprenticeship);
        }

        private async Task<ApprenticeshipDetailsViewModel> MapFrom(Apprenticeship apprenticeship)
        {
            //_mediator.SendAsync(new GetAllApprenticesRequest())
            return new ApprenticeshipDetailsViewModel
            {
                Id = apprenticeship.Id,
                FirstName = apprenticeship.FirstName,
                LastName  = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                Uln = apprenticeship.ULN,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = MapPaymentStatus(apprenticeship.PaymentStatus),
                EmployerName = "ToDo, add emplyer name"
            };
        }

        private string MapPaymentStatus(PaymentStatus paymentStatus)
        {
            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return "On programme";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Cancelled:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Completed";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }
    }
}