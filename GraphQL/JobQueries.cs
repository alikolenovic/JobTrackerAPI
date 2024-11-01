public class JobQueries
{
    private readonly JobTrackerDbContext _context;

    public JobQueries(JobTrackerDbContext context)
    {
        _context = context;
    }

    [UseProjection]
    public IQueryable<Job> GetJobs() => _context.Jobs;

    public Job GetJob(int id) => _context.Jobs.Find(id);
}
