using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public System.Guid UserId { get; set; }  // Stores the `oid` from the access token as the unique identifier

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date the user was created

    // --- Additional properties can be added as needed ---
    
    // Navigation property for related Jobs
    public ICollection<Job> Jobs { get; set; }

}