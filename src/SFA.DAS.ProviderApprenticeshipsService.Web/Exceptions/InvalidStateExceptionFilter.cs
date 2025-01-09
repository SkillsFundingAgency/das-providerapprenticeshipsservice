using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

public class InvalidStateExceptionFilter : IExceptionFilter
{
    private readonly ILogger<InvalidStateExceptionFilter> _logger;

    public InvalidStateExceptionFilter(ILogger<InvalidStateExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception.GetType() == typeof(InvalidStateException))
        {
            _logger.LogInformation(context.Exception, "Invalid state exception");

            context.ExceptionHandled = true;
        }
    }
}