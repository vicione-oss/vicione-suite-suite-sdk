using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

public class Salaries
{
    [Key]
    public int emp_no { get; set; }

    public int salary { get; set; }

    [Key]
    public DateTimeOffset from_date { get; set; }

    public DateTimeOffset to_date { get; set; }
    public Employees employee { get; set; } = null!;
}
