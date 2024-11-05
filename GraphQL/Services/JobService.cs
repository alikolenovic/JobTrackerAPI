using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Web;

public class JobService
{
    private readonly IDbContextFactory<JobTrackerDbContext> _contextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache;

    public JobService(IDbContextFactory<JobTrackerDbContext> contextFactory, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
    {
        _contextFactory = contextFactory;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<JobResponse> GetJobsAsync()
    {
        var oid = _httpContextAccessor.HttpContext?.User?.GetObjectId();
        string cacheKey = $"user/{oid}/jobs"; // Cache key to identify the data
        var cachedJobs = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedJobs))
        {
            // Deserialize and return cached data if available
            return new JobResponse {
                Jobs = JsonSerializer.Deserialize<List<Job>>(cachedJobs),
                IsCacheHit = true
            };
        }

        // Fetch a new context instance without a using statement
        var context = _contextFactory.CreateDbContext();

        // Retrieve jobs from the database
        var jobs = await context.Jobs.ToListAsync();

        // Set cache options (e.g., expiration time)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
        };

        // Serialize and store data in the cache
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(jobs), cacheOptions);

        return new JobResponse
        {
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
