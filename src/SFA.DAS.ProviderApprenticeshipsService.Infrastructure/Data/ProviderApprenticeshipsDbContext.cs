using System.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public class ProviderApprenticeshipsDbContext : DbContext
{
    private readonly ProviderApprenticeshipsServiceConfiguration _configuration;
    private readonly SqlConnection _connection;
    private readonly AzureServiceTokenProvider _tokenProvider;

    public ProviderApprenticeshipsDbContext(DbContextOptions options) : base(options)
    {
    }

    public ProviderApprenticeshipsDbContext(SqlConnection connection, DbContextOptions options,
        ProviderApprenticeshipsServiceConfiguration configuration,
        AzureServiceTokenProvider tokenProvider) : base(options)
    {
        _connection = connection;
        _configuration = configuration;
        _tokenProvider = tokenProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration == null || _tokenProvider == null) return;

        optionsBuilder.UseSqlServer(_connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("employer_account");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProviderApprenticeshipsDbContext).Assembly);
    }
}