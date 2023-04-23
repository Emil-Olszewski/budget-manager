using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Application.Interfaces
{
    public interface IContext
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Add([NotNull] object entity);
        EntityEntry Remove([NotNull] object entity);
        void AddRange([NotNull] params object[] entities);
        void RemoveRange([NotNull] IEnumerable<object> entities);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}