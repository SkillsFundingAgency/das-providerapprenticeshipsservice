using System.Threading.Tasks;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

public interface IIdamsSyncService
{
    Task SyncUsers();
}