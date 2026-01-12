using System.ComponentModel.DataAnnotations;

namespace Sdk.Backend.Tests;

internal sealed class TestOptions
{
    public bool BoolValue { get; set; }

    [StringLength(10)]
    public string? StringValue { get; set; }

    [Range(1, 1000)]
    public int? IntValue { get; set; }
}
