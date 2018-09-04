﻿using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IBackgroundNotificationService
    {
        Task SendEmail(Email email);
    }
}