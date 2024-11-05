public class UserResponse
{
    public string oid { get; set; }

    public User userObj { get; set; }

    public bool IsCacheHit { get; set; }

}