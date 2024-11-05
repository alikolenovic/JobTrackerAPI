namespace JobTrackerAPI.GraphQL.Mutations
{
    public class Mutation(JobMutations jobMutations, UserMutations userMutations)
    {
        public JobMutations JobMutations { get; } = jobMutations;
        public UserMutations UserMutations { get; } = userMutations;
    }
}