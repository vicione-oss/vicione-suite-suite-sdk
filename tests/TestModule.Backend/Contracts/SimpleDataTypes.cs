using System.ComponentModel.DataAnnotations;

namespace TestModule.Backend.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class SimpleDataTypes
{
    public long Id { get; init; }
    public Guid MachineId { get; init; }
    public string MachineName { get; init; }
    public string MachineDescription { get; init; }
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
    public string Name { get; init; }

    public Gender Salutaion { get; init; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
