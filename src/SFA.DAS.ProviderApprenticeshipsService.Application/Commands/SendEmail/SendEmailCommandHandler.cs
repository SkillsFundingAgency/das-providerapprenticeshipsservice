using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendEmail
{
    public sealed class SendEmailCommandHandler : AsyncRequestHandler<SendEmailCommand>
    {
        protected override Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            //todo validation?
            throw new NotImplementedException();
        }
    }
}
