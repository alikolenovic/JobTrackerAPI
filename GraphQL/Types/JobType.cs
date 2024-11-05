// GraphQL/JobType.cs
public class JobType : ObjectType<Job>
{
    protected override void Configure(IObjectTypeDescriptor<Job> descriptor)
    {
        descriptor.Field(t => t.JobTitle).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Company).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(t => t.JobDescription).Type<StringType>();
        descriptor.Field(t => t.Status).Type<StringType>();
        descriptor.Field(t => t.UserId).Type<StringType>();

        descriptor.Field(t => t.JobTitle)
                  .Description("The job title or position.");
        descriptor.Field(t => t.Company)
                  .Description("The company to which the job application was sent.");
        descriptor.Field(t => t.CreatedAt)
                  .Description("The date the job application was submitted.");
    }
}
