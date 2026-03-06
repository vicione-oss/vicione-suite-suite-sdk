using NSubstitute;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Infrastructure;
using Sdk.Messaging;
using Xunit;

namespace Sdk.Client.Tests.ControlPanels.Services;

public static class ControlPanelSaveHandlerBaseTests
{
    private sealed record TestCommand : ICommand
    {
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
    }

    private sealed class TestState : IControlPanelState
    {
        public int? ActivePageIndex { get; set; }
        public bool IsLoading { get; private set; }
        public int UpdateLock { get; private set; }

#pragma warning disable CS0067
        public event Action<ControlPanelStateChangedEventArgs>? Changed;
#pragma warning restore CS0067

        public void BeginLoading() => IsLoading = true;

        public void EndLoading() => IsLoading = false;

        public void BeginUpdate() => UpdateLock++;

        public void EndUpdate() => UpdateLock--;
    }

    /// <summary>
    /// Minimal concrete subclass that exposes the protected mechanics for testing.
    /// </summary>
    private sealed class TestableHandler(IUiMediator mediator) : ControlPanelSaveHandlerBase<TestState>(mediator)
    {
        private readonly TestCommand _command = new();

        public Guid CommandCorrelationId => _command.CorrelationId;

        public override Task<ISaveResult> Save(TestState state, CancellationToken cancellationToken = default)
            => SendAndWaitForCompletion(_command, cancellationToken);

        public bool SimulateSuccess(Guid correlationId) => CompleteWithSuccess(correlationId);

        public bool SimulateError(Guid correlationId, ErrorInfo? errorInfo) => CompleteWithError(correlationId, errorInfo);
    }

    public class Save
    {
        [Fact]
        public async Task Returns_SaveSuccessResult_when_backend_signals_success()
        {
            // Arrange
            var mediator = Substitute.For<IUiMediator>();
            using var sut = new TestableHandler(mediator);
            var state = new TestState();

            mediator.When(m => m.Send(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>()))
                .Do(_ => sut.SimulateSuccess(sut.CommandCorrelationId));

            // Act
            var result = await sut.Save(state, TestContext.Current.CancellationToken);

            // Assert
            await mediator.Received().Send(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
            Assert.IsType<SaveSuccessResult>(result);
        }

        [Fact]
        public async Task Returns_SaveErrorResult_when_backend_signals_error()
        {
            // Arrange
            var mediator = Substitute.For<IUiMediator>();
            using var sut = new TestableHandler(mediator);
            var state = new TestState();
            var errorInfo = new ErrorInfo(42, "Something went wrong");

            mediator.When(m => m.Send(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>()))
                .Do(_ => sut.SimulateError(sut.CommandCorrelationId, errorInfo));

            // Act
            var result = await sut.Save(state, TestContext.Current.CancellationToken);

            // Assert
            await mediator.Received().Send(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
            Assert.IsType<SaveErrorResult>(result);
        }

        [Fact]
        public async Task SaveErrorResult_contains_error_message_and_code_from_backend()
        {
            // Arrange
            var mediator = Substitute.For<IUiMediator>();
            using var sut = new TestableHandler(mediator);
            var state = new TestState();
            var errorInfo = new ErrorInfo(42, "Something went wrong");

            mediator.When(m => m.Send(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>()))
                .Do(_ => sut.SimulateError(sut.CommandCorrelationId, errorInfo));

            // Act
            var result = await sut.Save(state, TestContext.Current.CancellationToken);

            // Assert
            var errorResult = Assert.IsType<SaveErrorResult>(result);
            Assert.Equal(errorInfo.Message, errorResult.Message);
            Assert.Equal(errorInfo.ErrorCode, errorResult.ErrorCode);
        }
    }
}
