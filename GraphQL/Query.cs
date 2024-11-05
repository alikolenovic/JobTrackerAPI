public class Query
{
    // Simple query to get a job by its ID
    public async Task<JobResponse> GetAllJobsAsync([Service] JobService jobService)
    {
        return await jobService.GetJobsAsync();
    }

    // Add other queries as needed
};