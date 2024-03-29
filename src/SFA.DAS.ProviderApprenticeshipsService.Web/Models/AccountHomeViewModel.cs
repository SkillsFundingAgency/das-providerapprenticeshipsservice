﻿using Microsoft.AspNetCore.Html;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models;

public class AccountHomeViewModel
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public string Message { get; set; }
    public bool ShowAcademicYearBanner { get; set; }
    public bool ShowTraineeshipLink { get; set; }
    public bool ShowEarningsReport { get; set; }
    public HtmlString BannerContent { get; set; }
    public HtmlString CovidSectionContent { get; set; }
}

public enum AccountStatus
{
    Active,
    NotListed, // if details are not found in the provider lookup service
    NoAgreement // if a provider agreement hasn't been signed (though could be in progress)
}