using Core.Application.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence
{
    internal sealed class Context : DbContext, IContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                SetAuditInfo(entry);
            }

            return base.SaveChangesAsync(ct);
        }
        
        private void SetAuditInfo(EntityEntry<AuditableBaseEntity> entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Transaction>()
                .OwnsOne(x => x.TransactionDetails, x =>
                {
                    x.Property(y => y.Amount).HasColumnName("Amount");
                    x.Property(y => y.TransactionType).HasColumnName("TransactionType");
                });
            
            base.OnModelCreating(builder);
        }
    }
}