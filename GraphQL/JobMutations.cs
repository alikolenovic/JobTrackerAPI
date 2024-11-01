public class JobMutations
{
    private readonly JobTrackerDbContext _context;

    public JobMutations(JobTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Job> AddJob(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }
}
