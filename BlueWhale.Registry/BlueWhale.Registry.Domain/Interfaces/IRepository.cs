namespace BlueWhale.Registry.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<Entities.User>
{
    Task<Entities.User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}

public interface IActivityLogRepository : IRepository<Entities.ActivityLog>
{
    Task<IEnumerable<Entities.ActivityLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.ActivityLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}

public interface IAccessControlRepository : IRepository<Entities.AccessControl>
{
    Task<IEnumerable<Entities.AccessControl>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Entities.AccessControl?> GetByUserAndRepositoryAsync(Guid userId, string repository, CancellationToken cancellationToken = default);
}

public interface IRegistrySettingRepository : IRepository<Entities.RegistrySetting>
{
    Task<Entities.RegistrySetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.RegistrySetting>> GetByCategoryAsync(Entities.SettingCategory category, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IActivityLogRepository ActivityLogs { get; }
    IAccessControlRepository AccessControls { get; }
    IRegistrySettingRepository Settings { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
