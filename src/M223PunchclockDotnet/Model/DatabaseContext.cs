using Microsoft.EntityFrameworkCore;

namespace M223PunchclockDotnet.Model;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Entry> Entries => Set<Entry>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entry>().ToTable("Entry");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Tag>().ToTable("Tag");

        modelBuilder.Entity<Entry>()
            .HasOne(entry => entry.Category)
            .WithMany(category => category.Entries)
            .HasForeignKey(entry => entry.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Entry>()
            .HasMany(entry => entry.Tags)
            .WithMany(tag => tag.Entries)
            .UsingEntity<Dictionary<string, object>>(
                "EntryTag",
                tag => tag.HasOne<Tag>().WithMany().HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Restrict),
                entry => entry.HasOne<Entry>().WithMany().HasForeignKey("EntryId")
                    .OnDelete(DeleteBehavior.Cascade));
    }
}
