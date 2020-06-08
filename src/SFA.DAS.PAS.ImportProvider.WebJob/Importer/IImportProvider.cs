using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Importer
{
    interface IImportProvider
    {
        Task Import();
    }
}
