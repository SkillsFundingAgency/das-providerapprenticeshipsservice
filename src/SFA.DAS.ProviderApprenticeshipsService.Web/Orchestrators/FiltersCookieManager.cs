﻿using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class FiltersCookieManager : IFiltersCookieManager
    {
        private readonly ICookieStorageService<ApprenticeshipFiltersViewModel> _filterCookieStorageService;

        public FiltersCookieManager(ICookieStorageService<ApprenticeshipFiltersViewModel> filterCookieStorageService)
        {
            _filterCookieStorageService = filterCookieStorageService;
        }

        public ApprenticeshipFiltersViewModel CheckForCookie(ApprenticeshipFiltersViewModel filtersViewModel)
        {
            var tempFilters = _filterCookieStorageService.Get(nameof(ApprenticeshipFiltersViewModel))
                              ?? new ApprenticeshipFiltersViewModel();
            var cookieExists = tempFilters.HasValues();
            if (filtersViewModel.CheckCookie && cookieExists)
            {
                filtersViewModel = tempFilters;
            }
            if (!filtersViewModel.HasValues())
            {
                _filterCookieStorageService.Delete(nameof(ApprenticeshipFiltersViewModel));
                return filtersViewModel;
            }

            _filterCookieStorageService.Delete(nameof(ApprenticeshipFiltersViewModel));
            _filterCookieStorageService.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel));
            return filtersViewModel;
        }

        public ApprenticeshipFiltersViewModel GetCookie()
        {
            return _filterCookieStorageService.Get(nameof(ApprenticeshipFiltersViewModel))
                   ?? new ApprenticeshipFiltersViewModel();
        }

        public void SetCookie(ApprenticeshipFiltersViewModel filtersViewModel)
        {
            _filterCookieStorageService.Delete(nameof(ApprenticeshipFiltersViewModel));
            _filterCookieStorageService.Create(filtersViewModel, nameof(ApprenticeshipFiltersViewModel));
        }
    }
}