using Microsoft.EntityFrameworkCore;
using BlueWhale.Registry.Domain.Entities;
using BlueWhale.Registry.Domain.Interfaces;

namespace BlueWhale.Registry.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly RegistryDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(RegistryDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly RegistryDbContext _context;

    public UserRepository(RegistryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }
}

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    private readonly RegistryDbContext _context;

    public ActivityLogRepository(RegistryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.ActivityLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.ActivityLogs
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }
}

public class AccessControlRepository : Repository<AccessControl>, IAccessControlRepository
{
    private readonly RegistryDbContext _context;

    public AccessControlRepository(RegistryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccessControl>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AccessControls
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<AccessControl?> GetByUserAndRepositoryAsync(Guid userId, string repository, CancellationToken cancellationToken = default)
    {
        return await _context.AccessControls
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Repository == repository, cancellationToken);
    }
}

public class RegistrySettingRepository : Repository<RegistrySetting>, IRegistrySettingRepository
{
    private readonly RegistryDbContext _context;

    public RegistrySettingRepository(RegistryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RegistrySetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<IEnumerable<RegistrySetting>> GetByCategoryAsync(SettingCategory category, CancellationToken cancellationToken = default)
    {
        return await _context.Settings
            .Where(s => s.Category == category)
            .ToListAsync(cancellationToken);
    }
}
