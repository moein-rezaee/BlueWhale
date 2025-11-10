using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly RegistryDbContext _context;
    private IUserRepository? _userRepository;
    private IActivityLogRepository? _activityLogRepository;
    private IAccessControlRepository? _accessControlRepository;
    private IRegistrySettingRepository? _registrySettingRepository;

    public UnitOfWork(RegistryDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public IActivityLogRepository ActivityLogs => _activityLogRepository ??= new ActivityLogRepository(_context);
    public IAccessControlRepository AccessControls => _accessControlRepository ??= new AccessControlRepository(_context);
    public IRegistrySettingRepository Settings => _registrySettingRepository ??= new RegistrySettingRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
