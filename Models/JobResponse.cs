public class JobResponse
{
    public List<Job> Jobs { get; set; } = new List<Job>();
    public bool IsCacheHit { get; set; }
}