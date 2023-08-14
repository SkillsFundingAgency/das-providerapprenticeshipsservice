using NLog;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

public class InvalidStateExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception.GetType() == typeof(InvalidStateException))
        {
            LogManager.GetCurrentClassLogger().Info(context.Exception, "Invalid state exception");

            context.ExceptionHandled = true;
        }
    }
}