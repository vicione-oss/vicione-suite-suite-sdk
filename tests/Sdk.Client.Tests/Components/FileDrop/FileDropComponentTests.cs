using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Sdk.Client.Components.FileDrop;
using Sdk.Client.Services;
using Xunit;

namespace Sdk.Client.Tests.Components.FileDrop;

public sealed class FileDropComponentTests
{
    [Fact]
    public async Task Should_invoke_on_change_with_one_file()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b.Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt");

        // Assert
        changes.Should().ContainSingle().Which.File.Name.Should().Be("a.txt");
    }

    [Fact]
    public async Task Should_not_invoke_on_change_with_several_files()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b.Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt", "b.txt");

        // Assert
        changes.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_not_invoke_on_change_without_files()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b.Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component);

        // Assert
        changes.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_invoke_on_change_with_several_files_when_multiple_is_set()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b
            .Add(p => p.Multiple, true)
            .Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt", "b.txt");

        // Assert
        changes.Should().ContainSingle().Which.GetMultipleFiles().Select(file => file.Name).Should().Equal("a.txt", "b.txt");
    }

    [Fact]
    public async Task Should_not_invoke_on_change_without_files_when_multiple_is_set()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b
            .Add(p => p.Multiple, true)
            .Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component);

        // Assert
        changes.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Should_let_the_file_picker_select_several_files_only_when_multiple_is_set(bool multiple)
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);

        // Act
        var component = ctx.Render<FileDropComponent>(b => b.Add(p => p.Multiple, multiple));

        // Assert
        component.Find("input[type=file]").HasAttribute("multiple").Should().Be(multiple);
    }

    [Fact]
    public async Task Should_clear_the_file_name_after_on_change_when_clear_after_change_is_set()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b
            .Add(p => p.ClearAfterChange, true)
            .Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt");

        // Assert
        changes.Should().ContainSingle();
        fileDrop.Invocations.Should().ContainSingle(identifier => identifier == "clear");
    }

    [Fact]
    public async Task Should_clear_the_file_name_when_on_change_throws_and_clear_after_change_is_set()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var component = ctx.Render<FileDropComponent>(b => b
            .Add(p => p.ClearAfterChange, true)
            .Add(p => p.OnChange, _ => throw new InvalidOperationException("Not readable.")));

        // Act
#pragma warning disable CA2025 // The delegate is awaited by ThrowAsync before the context is disposed.
        var change = () => ChangeFiles(component, "a.txt");
#pragma warning restore CA2025

        // Assert
        await change.Should().ThrowAsync<InvalidOperationException>();
        fileDrop.Invocations.Should().ContainSingle(identifier => identifier == "clear");
    }

    [Fact]
    public async Task Should_keep_the_file_name_after_on_change_by_default()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b.Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt");

        // Assert
        changes.Should().ContainSingle();
        fileDrop.Invocations.Should().NotContain("clear");
    }

    [Fact]
    public async Task Should_keep_the_rejection_message_of_several_files_when_clear_after_change_is_set()
    {
        // Arrange
        await using var fileDrop = new FileDropScript();
        await using var ctx = CreateContext(fileDrop);
        var changes = new List<InputFileChangeEventArgs>();
        var component = ctx.Render<FileDropComponent>(b => b
            .Add(p => p.ClearAfterChange, true)
            .Add(p => p.OnChange, changes.Add));

        // Act
        await ChangeFiles(component, "a.txt", "b.txt");

        // Assert
        changes.Should().BeEmpty();
        fileDrop.Invocations.Should().NotContain("clear");
    }

    private static BunitContext CreateContext(FileDropScript fileDrop)
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        var jsInterop = Substitute.For<IJsInterop>();
        jsInterop.IncludeModuleScript(default!).ReturnsForAnyArgs(fileDrop);
        ctx.Services.AddSingleton(jsInterop);

        return ctx;
    }

    private static Task ChangeFiles(IRenderedComponent<FileDropComponent> component, params string[] filenames)
    {
        var files = filenames.Select(filename =>
        {
            var file = Substitute.For<IBrowserFile>();
            file.Name.Returns(filename);

            return file;
        }).ToList();

        var inputFile = component.FindComponent<InputFile>();

        return inputFile.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs(files)));
    }

    /// <summary>
    /// Stands for the imported script module and for the FileDrop object constructed from it, and records the invoked methods.
    /// </summary>
    private sealed class FileDropScript : IJSObjectReference
    {
        public List<string> Invocations { get; } = [];

        public ValueTask<IJSObjectReference> InvokeConstructorAsync(string identifier, object?[]? args)
            => ValueTask.FromResult<IJSObjectReference>(this);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Invocations.Add(identifier);

            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
