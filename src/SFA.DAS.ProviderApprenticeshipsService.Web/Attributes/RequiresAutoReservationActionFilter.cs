using SFA.DAS.Encoding;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class RequiresAutoReservationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (RequiresAutoReservation(filterContext, out var attribute))
            {
                var accountId = GetAccountId(filterContext, attribute);
                CheckCommitmentHasAutoReservationEnabled(filterContext, accountId);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        private bool RequiresAutoReservation(ActionExecutingContext filterContext, out RequiresAutoReservationAttribute attribute)
        {
            if (ActionRequiresAutoReservation(filterContext, out attribute) || ControllerRequiresAutoReservation(filterContext, out attribute))
            {
                return true;
            }

            return false;
        }

        private long GetAccountId(ActionExecutingContext filterContext, RequiresAutoReservationAttribute attribute)
        {
            var cohortId = GetCommitmentIdFromMethodParameters(filterContext, attribute);
            var providerId = GetProviderIdFromMethodParameters(filterContext, attribute);
            var accountId = GetAccountId(filterContext, cohortId, providerId);

            return accountId;
        }

        private bool ControllerRequiresAutoReservation(ActionExecutingContext filterContext, out RequiresAutoReservationAttribute attribute)
        {
            attribute = filterContext
                            .ActionDescriptor
                            .ControllerDescriptor
                            .GetCustomAttributes(typeof(RequiresAutoReservationAttribute), true)
                            .Cast<RequiresAutoReservationAttribute>()
                            .FirstOrDefault();

            return attribute != null;
        }

        private bool ActionRequiresAutoReservation(ActionExecutingContext filterContext, out RequiresAutoReservationAttribute attribute)
        {
            attribute = filterContext
                            .ActionDescriptor
                            .GetCustomAttributes(typeof(RequiresAutoReservationAttribute), true)
                            .Cast<RequiresAutoReservationAttribute>()
                            .FirstOrDefault();

            return attribute != null;
        }

        private void CheckCommitmentHasAutoReservationEnabled(ActionExecutingContext filterContext, long accountId)
        {
            if (GetAutoReservationStatus(filterContext, accountId))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "Current account is not authorized for automatic reservations");
            }
        }

        private string GetHashedCommitmentIdFromMethodParameters(ActionExecutingContext filterContext, RequiresAutoReservationAttribute attribute)
        {
            return GetNamedParameterFromAction(filterContext, attribute.HashedCommitmentIdField);
        }

        private long GetCommitmentIdFromMethodParameters(ActionExecutingContext filterContext, RequiresAutoReservationAttribute attribute)
        {
            var hashedCohortId = GetHashedCommitmentIdFromMethodParameters(filterContext, attribute);
            var cohortId = UnhashCommitmentId(filterContext, hashedCohortId);
            return cohortId;
        }

        private long GetProviderIdFromMethodParameters(ActionExecutingContext filterContext, RequiresAutoReservationAttribute attribute)
        {
            var s = GetNamedParameterFromAction(filterContext, attribute.ProviderIdField);
            if (!long.TryParse(s, out var providerId))
            {
                var message = $"The value supplied for provider id in field {attribute.ProviderIdField} (\"s\") could not be coerced into an integer.";
                throw new InvalidOperationException(message);
            }

            return providerId;
        }

        private long UnhashCommitmentId(ActionExecutingContext filterContext, string hashedCohortId)
        {
            var hashingService = GetService<IEncodingService>(filterContext);

            if (!hashingService.TryDecode(hashedCohortId, EncodingType.CohortReference, out long cohortId))
            {
                var message = $"The value specified for the cohort Id could not be decoded ({hashedCohortId})";
                throw new InvalidOperationException(message);
            }

            return cohortId;
        }

        private long GetAccountId(ActionExecutingContext filterContext, long providerId, long commitmentId)
        {
            var commitmentsApi = GetService<IProviderCommitmentsApi>(filterContext);
            var commitment = commitmentsApi
                .GetProviderCommitment(providerId, commitmentId);

            // blocking code
            return commitment.GetAwaiter().GetResult().EmployerAccountId;
        }

        private bool GetAutoReservationStatus(ActionExecutingContext filterContext, long accountId)
        {
            var reservationService = GetService<IReservationsService>(filterContext);

            // blocking code
            return reservationService.IsAutoReservationEnabled(accountId).GetAwaiter().GetResult();
        }

        private T GetService<T>(ActionExecutingContext filterContext) where T : class
        {
            var service = (T)filterContext.HttpContext.GetService(typeof(T));

            if (service == null)
            {
                var message = $"Failed to get an instance of {typeof(T).Name} from the current IoC container";
                throw new InvalidOperationException(message);
            }

            return service;
        }

        private string GetNamedParameterFromAction(ActionExecutingContext filterContext, string parameterName)
        {
            // TODO: need to support dotted paths
            if (!filterContext.ActionParameters.TryGetValue(parameterName, out var value))
            {
                var message = $"The attribute {nameof(RequiresAutoReservationAttribute)} " +
                              $"on {filterContext.ActionDescriptor.ControllerDescriptor.ControllerName} " +
                              $"specifies a parameter name of {parameterName} " +
                              "but a method parameter of that name could not be found.";

                throw new InvalidOperationException(message);
            }

            return (string)value;
        }
    }
}