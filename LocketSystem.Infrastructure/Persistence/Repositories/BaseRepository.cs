using LocketMini.Domain.Common;
using LocketMini.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LocketMini.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<TEntity> Set;

    protected BaseRepository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        => await Set.FindAsync(new object[] { id }, ct);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public virtual void Update(TEntity entity)
        => Set.Update(entity);

    public virtual void Remove(TEntity entity)
        => Set.Remove(entity);
}
