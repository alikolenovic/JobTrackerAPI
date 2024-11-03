using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

public class JobQueries
{
    private readonly IDbContextFactory<JobTrackerDbContext> _contextFactory;

     private readonly IDistributedCache _cache;

    public JobQueries(IDbContextFactory<JobTrackerDbContext> contextFactory, IDistributedCache cache)
    {
        _contextFactory = contextFactory;
        _cache = cache;
    }

    public async Task<JobResponse> GetJobsAsync()
    {
        const string cacheKey = "jobs"; // Cache key to identify the data
        var cachedJobs = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedJobs))
        {
            // Deserialize and return cached data if available
            return new JobResponse {
                Jobs = JsonSerializer.Deserialize<List<Job>>(cachedJobs),
                IsCacheHit = true
            };
        }

        // If cache is empty, fetch data from the database
        using var context = _contextFactory.CreateDbContext();
        var jobs = await context.Jobs.ToListAsync();

        // Set cache options (e.g., expiration time)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
        };

        // Serialize and store data in the cache
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(jobs), cacheOptions);

        return new JobResponse {
            Jobs = jobs,
            IsCacheHit = false
        };
    }

    public async Task<Job?> GetJobAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Jobs.FindAsync(id);
    }
}
