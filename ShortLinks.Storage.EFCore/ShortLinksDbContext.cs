using Microsoft.EntityFrameworkCore;
using ShortLinks.Abstractions.Models;

namespace ShortLinks.Storage.EFCore
{
    public sealed class ShortLinksDbContext : DbContext
    {
        public ShortLinksDbContext(DbContextOptions<ShortLinksDbContext> options) : base(options) { }

        public DbSet<ShortLink> ShortLinks => Set<ShortLink>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var e = modelBuilder.Entity<ShortLink>();

            //e.ToTable("ShortLinks");
            e.HasKey(x => x.Id);

            e.Property(x => x.Code).HasMaxLength(32).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();

            e.Property(x => x.OriginalUrl).HasMaxLength(1000).IsRequired();

            e.Property(x => x.CreatedAtUtc).IsRequired();
            e.Property(x => x.ExpireAtUtc);

            e.Property(x => x.HitCount).IsRequired();
            e.Property(x => x.LastAccessedAtUtc);

            e.Property(x => x.MaxUses);
            e.Property(x => x.UsedCount);

            base.OnModelCreating(modelBuilder);
        }
    }

}
