using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    public class ForgePropertiesDbContext: DbContext
    {
        public string Filename { get; set; }

        private readonly ILoggerFactory _loggerFactory;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlite(new SqliteConnectionStringBuilder { DataSource = Filename }.ToString());
        }

        public ForgePropertiesDbContext(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ForgePropertyObjectEav>()
                .HasOne(x => x.Attribute)
                .WithMany(x => x.Eavs)
                .HasForeignKey(x => x.AttributeId);

            modelBuilder
                .Entity<ForgePropertyObjectEav>()
                .HasOne(x => x.Entity)
                .WithMany(x => x.Eavs)
                .HasForeignKey(x => x.EntityId);

            modelBuilder
                .Entity<ForgePropertyObjectEav>()
                .HasOne(x => x.Value)
                .WithMany(x => x.Eavs)
                .HasForeignKey(x => x.ValueId);
        }

        public DbSet<ForgePropertyObject> Objects { get; set; }
        public DbSet<ForgePropertyObjectAttribute> ObjectAttributes { get; set; }
        public DbSet<ForgePropertyObjectValue> ObjectValues { get; set; }
        public DbSet<ForgePropertyObjectEav> ObjectEavs { get; set; }
    }
}
