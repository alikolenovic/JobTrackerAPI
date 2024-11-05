using Microsoft.EntityFrameworkCore;

public class JobTrackerDbContext : DbContext
{
    public JobTrackerDbContext(DbContextOptions<JobTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.Jobs)
            .WithOne(j => j.User)
            .HasForeignKey(j => j.UserId);  // Set up foreign key
    }
}