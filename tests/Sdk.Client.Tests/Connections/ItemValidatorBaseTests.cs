using AwesomeAssertions;
using Sdk.Client.Connections;
using Xunit;

namespace Sdk.Client.Tests.Connections;

public sealed class ItemValidatorBaseTests
{
    private sealed class TestItem
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class TestValidator : ItemValidatorBase<TestItem>
    {
        protected override void ValidateInternal(TestItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                AddError(nameof(TestItem.Name), "Name is required.");

            if (item.Age < 0)
                AddError(nameof(TestItem.Age), "Age must be non-negative.");
        }
    }

    [Fact]
    public void Validate_should_return_empty_when_valid()
    {
        var validator = new TestValidator();

        var errors = validator.Validate(new TestItem { Name = "Alice", Age = 30 });

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_should_return_errors_for_invalid_item()
    {
        var validator = new TestValidator();

        var errors = validator.Validate(new TestItem { Name = null, Age = -1 });

        errors.Should().ContainKey(nameof(TestItem.Name));
        errors.Should().ContainKey(nameof(TestItem.Age));
        errors[nameof(TestItem.Name)].Should().Contain("Name is required.");
        errors[nameof(TestItem.Age)].Should().Contain("Age must be non-negative.");
    }

    [Fact]
    public void Validate_object_overload_should_dispatch_to_typed_validate()
    {
        var validator = new TestValidator();
        object item = new TestItem { Name = null, Age = 5 };

        var errors = validator.Validate(item);

        errors.Should().ContainKey(nameof(TestItem.Name));
        errors.Should().NotContainKey(nameof(TestItem.Age));
    }

    [Fact]
    public void Validate_object_overload_should_throw_for_wrong_type()
    {
        var validator = new TestValidator();
        object wrongItem = "not a TestItem";

        var act = () => validator.Validate(wrongItem);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Validate_should_clear_errors_between_invocations()
    {
        var validator = new TestValidator();

        var firstErrors = validator.Validate(new TestItem { Name = null, Age = -1 });
        firstErrors.Should().HaveCount(2);

        var secondErrors = validator.Validate(new TestItem { Name = "Bob", Age = 25 });
        secondErrors.Should().BeEmpty();
    }

    [Fact]
    public void AddError_should_accumulate_multiple_errors_for_same_key()
    {
        var multiValidator = new MultiErrorValidator();

        var errors = multiValidator.Validate(new TestItem { Name = null, Age = -1 });

        errors[nameof(TestItem.Name)].Should().HaveCount(2);
    }

    private sealed class MultiErrorValidator : ItemValidatorBase<TestItem>
    {
        protected override void ValidateInternal(TestItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                AddError(nameof(TestItem.Name), "Name is required.");
                AddError(nameof(TestItem.Name), "Name cannot be whitespace.");
            }
        }
    }
}

