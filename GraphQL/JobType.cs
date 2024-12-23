// GraphQL/JobType.cs
public class JobType : ObjectType<Job>
{
    protected override void Configure(IObjectTypeDescriptor<Job> descriptor)
    {
        descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
        descriptor.Field(t => t.Title).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Company).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.DateApplied).Type<NonNullType<DateTimeType>>();

        descriptor.Field(t => t.Title)
                  .Description("The job title or position.");
        descriptor.Field(t => t.Company)
                  .Description("The company to which the job application was sent.");
        descriptor.Field(t => t.DateApplied)
                  .Description("The date the job application was submitted.");
    }
}
