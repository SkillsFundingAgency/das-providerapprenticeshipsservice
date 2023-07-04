namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models;

public interface IPaginationViewModel
{
    int PageNumber { get; }

    int TotalPages { get; }

    int PageSize { get; }

    int TotalResults { get; }
}