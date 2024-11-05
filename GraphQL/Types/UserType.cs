
public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(t => t.UserId).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.CreatedAt).Type<NonNullType<DateTimeType>>();

        descriptor.Field(t => t.UserId)
                  .Description("The oid of the user.");
        descriptor.Field(t => t.CreatedAt)
                  .Description("When the user was created.");
    }
}
