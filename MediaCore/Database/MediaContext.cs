using Microsoft.EntityFrameworkCore;

namespace MediaCore.Database
{

    // a database context, using MediaData and MediaItem
    // Uses MS Sql Server
    // One MediaItem  has one MediaData
    // One MediaData has many MediaItem
    public class MediaContext : DbContext
    {
        public MediaContext() : base()
        {
        }

        public MediaContext(DbContextOptions<MediaContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // add IConfigurationRoot  to get connection string 
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("MediaCore"));
            }
        }

        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<MediaData> MediaDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.ToTable("MediaItem");
                entity.HasOne(p => p.MediaData)
                    .WithMany(b => b.MediaItemS)
                    .HasForeignKey(p => p.MediaDataId);
                entity.HasIndex(s => s.Resource);
                entity.HasKey(s => s.Id);

            });

            modelBuilder.Entity<MediaData>(entity =>
            {
                entity.ToTable("MediaData");
                entity.HasMany(p => p.MediaItemS)
                    .WithOne(b => b.MediaData)
                    .HasForeignKey(p => p.MediaDataId);

                entity.HasIndex(s => s.Hash);
            });

        }
    }
}
