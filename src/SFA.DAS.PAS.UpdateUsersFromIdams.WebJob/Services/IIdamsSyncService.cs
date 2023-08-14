using System.Threading.Tasks;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

interface IIdamsSyncService
{
    Task SyncUsers();
}