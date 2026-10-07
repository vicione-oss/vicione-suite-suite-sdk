using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

public class Titles
{
    [Key]
    public int emp_no { get; set; }

    [Key]
    public required string title { get; set; }

    [Key]
    public DateTimeOffset from_date { get; set; }

    public DateTimeOffset to_date { get; set; }
    public Employees employee { get; set; } = null!;
}
