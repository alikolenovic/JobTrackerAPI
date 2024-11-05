using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Web;

namespace JobTrackerAPI.GraphQL.Mutations
{
    public class JobMutations
    {
        private readonly IDbContextFactory<JobTrackerDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cache;

        public JobMutations(IDbContextFactory<JobTrackerDbContext> contextFactory, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task<Job> AddJob(string JobTitle, string JobDescription, string Company)
        {
            var oid = _httpContextAccessor.HttpContext?.User?.GetObjectId();

            if (string.IsNullOrEmpty(oid))
            {
                throw new Exception("OID not found in access token claims.");
            }

            string cacheKey = $"user/{oid}/jobs";
            await _cache.RemoveAsync(cacheKey);

            var job = new Job
            {
                JobTitle = JobTitle,
                JobDescription = JobDescription,
                Company = Company,
                CreatedAt = DateTime.UtcNow,
                Status = "active",
                UserId = new Guid(oid)
            };
            
            using var context = _contextFactory.CreateDbContext();
            context.Jobs.Add(job);
            await context.SaveChangesAsync();
            return job;
        }
    }
}
