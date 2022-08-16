// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using StructureMap;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Configuration;
using SFA.DAS.Reservations.Api.Client.DependencyResolution;
using SFA.DAS.Authorization.DependencyResolution;
using SFA.DAS.Authorization.ProviderPermissions.DependencyResolution;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.ProviderPermissions.DependencyResolution.StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{

    public static class IoC {
        public static IContainer Initialize() {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<AccountApiConfiguration>("SFA.DAS.EmployerAccountAPI"));
                c.Policies.Add(new ConfigurationPolicy<ProviderApprenticeshipsServiceConfiguration>("SFA.DAS.ProviderApprenticeshipsService"));
                c.Policies.Add(new ConfigurationPolicy<AccountApiConfiguration>("SFA.DAS.EmployerAccountAPI"));
                c.Policies.Add(new ConfigurationPolicy<RoatpCourseManagementWebConfiguration>("SFA.DAS.Roatp.CourseManagement.Web"));
                c.Policies.Add<CurrentDatePolicy>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<ProviderPermissionsAuthorizationRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<ReservationsApiClientRegistry>();
                c.AddRegistry<LinkGeneratorRegistry>();
                c.AddRegistry<EncodingRegistry>();
                c.AddRegistry<ContentApiClientRegistry>();
                c.AddRegistry<RoatpCourseManagementWebRegistry>();
            });
        }
    }
}