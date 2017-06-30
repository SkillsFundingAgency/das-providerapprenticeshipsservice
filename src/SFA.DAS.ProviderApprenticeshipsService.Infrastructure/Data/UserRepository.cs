using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILog _logger;

        public UserRepository(IConfiguration config, ILog logger) : base(config.DatabaseConnectionString, logger)
        {
            _logger = logger;
        }

        public Task Upsert(User user)
        {
            throw new NotImplementedException();
        }
    }
}
