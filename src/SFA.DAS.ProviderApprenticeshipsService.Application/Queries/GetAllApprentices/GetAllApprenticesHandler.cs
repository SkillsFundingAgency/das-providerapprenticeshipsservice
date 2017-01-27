using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices
{
    public class GetAllApprenticesHandler : IAsyncRequestHandler<GetAllApprenticesRequest, GetAllApprenticesResponse>
    {
        public Task<GetAllApprenticesResponse> Handle(GetAllApprenticesRequest message)
        {
            var model = new GetAllApprenticesResponse
                            {
                                Apprenticeships = new List<Apprenticeship>
                                                      {
                                                          new Apprenticeship
                                                              {
                                                                  AgreementStatus = AgreementStatus.BothAgreed, 
                                                                  FirstName = "Chris",
                                                                  LastName = "Froberg",
                                                                  DateOfBirth = new DateTime(1998, 12, 08),
                                                                  Cost = 5000
                                                              }
                                                      }
                            };
            return Task.FromResult(model);
            // ToDo: Add method to commitments API // GetProviderApprenticeships(long providerId)
        }
    }
}