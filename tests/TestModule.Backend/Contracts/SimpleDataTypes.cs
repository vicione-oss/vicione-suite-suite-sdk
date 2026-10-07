using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

public class SimpleDataTypes
{
    public long Id { get; init; }
    public Guid MachineId { get; init; }
    public required string MachineName { get; init; }
    public required string MachineDescription { get; init; }
    public int Users { get; init; }
    public DateTimeOffset Built { get; set; }
    public DateTimeOffset Created { get; set; }
}

public class SimpleEmployees
{
    public enum Gender
    {
        Mr,
        Mrs
    }

    public int emp_no { get; init; }

    [MaxLength(30)]
    public required string Name { get; init; }

    public Gender Salutaion { get; init; }
}
