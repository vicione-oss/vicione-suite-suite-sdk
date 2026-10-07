using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

public class Departments
{
    [Key]
    [MaxLength(4)]
    public required string dept_no { get; init; }

    [MaxLength(40)]
    public required string dept_name { get; init; }

    public IList<DepartmentManager> dept_manager { get; init; } = [];
    public IList<DepartmentEmployees> dept_empl { get; init; } = [];
}
