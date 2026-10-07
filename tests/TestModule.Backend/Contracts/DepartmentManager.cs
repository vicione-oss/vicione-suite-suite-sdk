using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

public class DepartmentManager
{
    [Key]
    public int emp_no { get; set; }

    [Key]
    public required string dept_no { get; set; }

    public DateTimeOffset from_date { get; set; }
    public DateTimeOffset to_date { get; set; }
    public Employees employee { get; set; } = null!;
    public Departments department { get; set; } = null!;
}
