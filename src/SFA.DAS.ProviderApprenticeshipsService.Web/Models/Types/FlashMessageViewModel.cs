using Newtonsoft.Json;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

public class FlashMessageViewModel
{
    public static FlashMessageViewModel GetViewModel(string message)
    {
        if (!string.IsNullOrEmpty(message))
            return JsonConvert.DeserializeObject<FlashMessageViewModel>(message);

        return new FlashMessageViewModel { Message = string.Empty, Severity = FlashMessageSeverityLevel.None };
    }

    public string Message { get; set; }

    public FlashMessageSeverityLevel Severity { get; set; }
}