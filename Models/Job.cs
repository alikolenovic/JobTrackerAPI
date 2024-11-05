using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Job
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int JobId { get; set; }
    public required string JobTitle { get; set; }
    public required string JobDescription { get; set; }
    public required string Company { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // Foreign key to User
    public System.Guid UserId { get; set; }

    public User User {get; set;}
}