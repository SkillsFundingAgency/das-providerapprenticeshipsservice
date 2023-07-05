using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Services;

public interface IImportProviderService
{
    Task Import();
}