using System.Threading.Tasks;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob
{
    interface IIdamsSyncService
    {
        Task SyncUsers();
    }
}
