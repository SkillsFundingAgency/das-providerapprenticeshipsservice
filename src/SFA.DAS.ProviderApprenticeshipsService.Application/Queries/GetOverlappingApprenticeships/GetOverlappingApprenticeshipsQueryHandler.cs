using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Commitments.Api.Types.Validation.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryHandler :
        IAsyncRequestHandler<GetOverlappingApprenticeshipsQueryRequest, GetOverlappingApprenticeshipsQueryResponse>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;

        public GetOverlappingApprenticeshipsQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            if(commitmentsApi == null)
                throw new ArgumentException(nameof(commitmentsApi));
            _providerCommitmentsApi = commitmentsApi;
        }

        public async Task<GetOverlappingApprenticeshipsQueryResponse> Handle(GetOverlappingApprenticeshipsQueryRequest request)
        {
            var apprenticeships = request.Apprenticeship.Where(m =>
                                                               m.StartDate != null &&
                                                               m.EndDate != null &&
                                                               !string.IsNullOrEmpty(m.ULN));
            
            if (!apprenticeships.Any())
            {
                return new GetOverlappingApprenticeshipsQueryResponse
                           {
                               Overlaps =
                                   Enumerable.Empty<OverlappingApprenticeship>()
                           };
            }
            
            if (apprenticeships.FirstOrDefault().ULN == "1112223331")
            {
                return OverlapStartDate();
            }

            if (apprenticeships.FirstOrDefault().ULN == "1112223332")
            {
                return OverlapEndDate();
            }

            if (apprenticeships.FirstOrDefault().ULN == "1112223333")
            {
                return OverlapEmbrace();
            }

            if (apprenticeships.FirstOrDefault().ULN == "1112223334")
            {
                return OverlapWithin();
            }

            if (apprenticeships.FirstOrDefault().ULN == "1112223335")
            {
                return OverlapWith2();
            }



            // ToDo: Make call if any valid
            //var result = _commitmentsApi.Validation.ValidateApprenticeship(request.Apprenticeship);

            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = Enumerable.Empty<OverlappingApprenticeship>()
            };
        }

        private GetOverlappingApprenticeshipsQueryResponse OverlapWith2()
        {
            var model = new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlappingApprenticeship>
                               {
                     new OverlappingApprenticeship
                                       {
                                           EmployerAccountId = 123456,
                                           LegalEntityName = "Legal entity name",
                                           ProviderId = 665544,
                                           ProviderName = "Provider name!",
                                           ValidationFailReason = ValidationFailReason.OverlappingStartDate,
                                           Apprenticeship = new Apprenticeship { ULN = "1112223331" }
                                       },
                                   new OverlappingApprenticeship
                                       {
                                           EmployerAccountId = 123456,
                                           LegalEntityName = "Legal entity name",
                                           ProviderId = 665544,
                                           ProviderName = "Provider name!",
                                           ValidationFailReason = ValidationFailReason.OverlappingStartDate,
                                           Apprenticeship = new Apprenticeship { ULN = "1112223335" }
                                       },
                                   new OverlappingApprenticeship
                                       {
                                           EmployerAccountId = 123456,
                                           LegalEntityName = "Legal entity name 2",
                                           ProviderId = 665544,
                                           ProviderName = "Provider name! 2",
                                           ValidationFailReason = ValidationFailReason.OverlappingEndDate,
                                           Apprenticeship = new Apprenticeship { ULN = "1112223337" }
                                       }
                               }
            };
            return model;
        }

        private GetOverlappingApprenticeshipsQueryResponse OverlapWithin()
        {
            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlappingApprenticeship>
                {
                    new OverlappingApprenticeship
                        {
                            EmployerAccountId = 123456,
                            LegalEntityName = "Legal entity name",
                            ProviderId = 665544,
                            ProviderName = "Provider name!",
                            ValidationFailReason = ValidationFailReason.DateWithin,
                            Apprenticeship = new Apprenticeship { ULN = "111999001" }
                        }
                }
            };
        }


        private GetOverlappingApprenticeshipsQueryResponse OverlapEmbrace()
        {
            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlappingApprenticeship>
                {
                    new OverlappingApprenticeship
                        {
                            EmployerAccountId = 123456,
                            LegalEntityName = "Legal entity name",
                            ProviderId = 665544,
                            ProviderName = "Provider name!",
                            ValidationFailReason = ValidationFailReason.DateEmbrace,
                            Apprenticeship = new Apprenticeship { ULN = "111999001" }
                        }
                }
            };
        }

        private GetOverlappingApprenticeshipsQueryResponse OverlapStartDate()
        {
            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlappingApprenticeship>
                {
                    new OverlappingApprenticeship
                        {
                            EmployerAccountId = 123456,
                            LegalEntityName = "Legal entity name",
                            ProviderId = 665544,
                            ProviderName = "Provider name!",
                            ValidationFailReason = ValidationFailReason.OverlappingStartDate,
                            Apprenticeship = new Apprenticeship { ULN = "111999001" }
                        }
                }
            };
        }

        private GetOverlappingApprenticeshipsQueryResponse OverlapEndDate()
        {
            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlappingApprenticeship>
                {
                    new OverlappingApprenticeship
                    {
                        EmployerAccountId = 123456,
                        LegalEntityName = "Legal entity name 2",
                        ProviderId = 665544,
                        ProviderName = "Provider name! 2",
                        ValidationFailReason = ValidationFailReason.OverlappingEndDate
                    }
                }
            };

        }
    }
}