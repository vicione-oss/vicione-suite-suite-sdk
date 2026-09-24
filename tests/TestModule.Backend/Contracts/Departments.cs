using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Departments
{
    [Key]
    [MaxLength(4)]
    public string dept_no { get; init; }

    [MaxLength(40)]
    public string dept_name { get; init; }

    public IList<DepartmentManager> dept_manager { get; init; }
    public IList<DepartmentEmployees> dept_empl { get; init; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
