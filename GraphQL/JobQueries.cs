using Microsoft.EntityFrameworkCore;

public class JobQueries
{
    private readonly IDbContextFactory<JobTrackerDbContext> _contextFactory;

    public JobQueries(IDbContextFactory<JobTrackerDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Job>> GetJobsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Jobs.ToListAsync();
    }

    public async Task<Job?> GetJobAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Jobs.FindAsync(id);
    }
}
