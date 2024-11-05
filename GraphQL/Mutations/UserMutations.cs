using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Web;

namespace JobTrackerAPI.GraphQL.Mutations
{
    public class UserMutations
    {
        private readonly IDbContextFactory<JobTrackerDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cache;

        public UserMutations(IDbContextFactory<JobTrackerDbContext> contextFactory, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task<UserResponse> FindOrCreateUserAsync()
        {
            // Retrieve the `oid` from the access token claims
            var oid = _httpContextAccessor.HttpContext?.User?.GetObjectId();

            if (string.IsNullOrEmpty(oid))
            {
                throw new Exception("OID not found in access token claims.");
            }

            string cacheKey = $"user/{oid}";

            var cachedJobs = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedJobs)) {
                return new UserResponse
                {
                    oid = oid,
                    userObj = JsonSerializer.Deserialize<User>(cachedJobs),
                    IsCacheHit = true
                };
            }

            using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FirstOrDefaultAsync(user => user.UserId.ToString() == oid);

            // Set cache options (e.g., expiration time)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for hour
            };

            if (user != null) {
                await _cache.SetStringAsync($"user/{oid}", JsonSerializer.Serialize<User>(user));
                return new UserResponse
                            {
                                oid = oid,
                                userObj = user
                            };
            }

            user = new User
            {
                UserId = new Guid(oid),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            await _cache.SetStringAsync($"user/{oid}", JsonSerializer.Serialize<User>(user));

            return new UserResponse
                            {
                                oid = oid,
                                userObj = user
                            };
        }
    }
}