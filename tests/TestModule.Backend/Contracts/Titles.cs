using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Titles
{
    [Key]
    public int emp_no { get; set; }

    [Key]
    public string title { get; set; }

    [Key]
    public DateTimeOffset from_date { get; set; }

    public DateTimeOffset to_date { get; set; }
    public Employees employee { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

