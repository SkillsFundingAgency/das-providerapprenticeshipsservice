using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendEmail
{
    public class SendEmailCommand : IRequest
    {
        public IEnumerable<Email> Emails { get; set; }
    }
}
