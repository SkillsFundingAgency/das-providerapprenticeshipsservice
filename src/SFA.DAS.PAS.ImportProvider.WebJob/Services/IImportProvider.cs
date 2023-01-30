using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Services
{
    interface IImportProvider
    {
        Task Import();
    }
}
