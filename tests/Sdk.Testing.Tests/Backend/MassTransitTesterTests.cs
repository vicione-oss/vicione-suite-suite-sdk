using System.Diagnostics;
using AwesomeAssertions;
using MassTransit;
using MassTransit.Courier.Contracts;
using NUnit.Framework;
using Sdk.Testing.Backend;
using TestModule.Backend.Consumer;
using Xunit;
using Assert = Xunit.Assert;

namespace Sdk.Testing.Tests.Backend;

public class MassTransitTesterTests
{
    public sealed class TestCommand : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestCommandConsumer>();

        [Fact]
        public async Task Should_consume_command()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestConsumerCommand();

            // Act + Assert
            await tester.TestCommand<TestConsumerCommand, TestCommandConsumer>(command);
        }

        [Fact]
        public async Task Should_throw_on_consume_exception()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestConsumerCommand
            {
                ThrowException = true
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tester.TestCommand<TestConsumerCommand, TestCommandConsumer>(command));
        }

        [Fact]
        public async Task Should_consume_command_and_return_correlated_event()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestConsumerCommand
            {
                FireEvent = true
            };

            // Act
            var firedEvent = await tester.TestCommand<TestConsumerCommand, TestCommandConsumer, TestConsumerEvent>(command);

            // Assert
            firedEvent.Should().NotBeNull();
            firedEvent.CorrelationId.Should().Be(command.CorrelationId);
        }

        [Fact]
        public async Task Should_consume_command_and_throw_if_expected_event_is_not_fired()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestConsumerCommand
            {
                FireEvent = false
            };

            // Act + Assert
            await Assert.ThrowsAsync<AssertionException>(() => tester.TestCommand<TestConsumerCommand, TestCommandConsumer, TestConsumerEvent>(command));
        }

        [Fact]
        public async Task Should_consume_multiple_commands_and_return_correlated_events()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var commands = new[]
            {
                new TestConsumerCommand
                {
                    FireEvent = true
                },
                new TestConsumerCommand
                {
                    FireEvent = true
                }
            };

            // Act
            var firedEvents = await tester.TestCommands<TestConsumerCommand, TestCommandConsumer, TestConsumerEvent>(commands);

            // Assert
            firedEvents.Should().HaveCount(commands.Length);
            firedEvents.Should().BeEquivalentTo(new List<TestConsumerEvent>
            {
                new(commands[0].CorrelationId), new(commands[1].CorrelationId),
            });
        }

        [Fact]
        public async Task Should_consume_multiple_commands_and_return_fired_events()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var commands = new[]
            {
                new TestConsumerCommand(),
                new TestConsumerCommand
                {
                    FireEvent = true
                }
            };

            // Act
            var firedEvents = await tester.TestCommands<TestConsumerCommand, TestCommandConsumer, TestConsumerEvent>(commands);

            // Assert
            firedEvents.Should().ContainSingle(k => k.CorrelationId == commands[1].CorrelationId);
        }
    }

    public sealed class TestCommandFault : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestCommandConsumer>();

        [Fact]
        public async Task Should_consume_command_fault()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestConsumerCommand
            {
                ThrowException = true
            };

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => tester.TestCommandFault<TestConsumerCommand, TestCommandConsumer>(command));

            // Assert
            Assert.True(await tester.Harness.Published.Any<TestConsumerFaultEvent>(tester.Harness.CancellationToken));
        }
    }

    public sealed class TestEvent : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestEventConsumer>();

        [Fact]
        public async Task Should_consume_event()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var testEvent = new TestConsumerEvent(Guid.NewGuid());

            // Act + Assert
            await tester.TestEvent<TestConsumerEvent, TestEventConsumer>(testEvent);
        }

        [Fact]
        public async Task Should_throw_if_consume_event_fails()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var testEvent = new TestConsumerEvent(Guid.NewGuid())
            {
                ThrowException = true
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tester.TestEvent<TestConsumerEvent, TestEventConsumer>(testEvent));
        }

        [Fact]
        public async Task Should_throw_if_event_is_not_consumed()
        {
            // Arrange
            await using var tester = new MassTransitTester(cfg => cfg.AddConsumer<TestCommandConsumer>());
            var testEvent = new TestConsumerEvent(Guid.NewGuid());

            // Act + Assert
            await Assert.ThrowsAsync<AssertionException>(() => tester.TestEvent<TestConsumerEvent, TestCommandConsumer>(testEvent));
        }
    }

    public sealed class TestInstanceDependentCommand : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestInstanceCommandConsumer>();

        [Fact]
        public async Task Should_consume_command()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestInstanceConsumerCommand();

            // Act + Assert
            await tester.TestInstanceDependentCommand<TestInstanceConsumerCommand, TestInstanceCommandConsumer>(command);
        }

        [Fact]
        public async Task Should_throw_on_consume_exception()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestInstanceConsumerCommand
            {
                ThrowException = true
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tester.TestInstanceDependentCommand<TestInstanceConsumerCommand, TestInstanceCommandConsumer>(command));
        }
    }

    public sealed class TestInstanceDependentCommandFault : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestInstanceCommandConsumer>();

        [Fact]
        public async Task Should_consume_command_fault()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new TestInstanceConsumerCommand
            {
                ThrowException = true
            };

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => tester.TestInstanceDependentCommandFault<TestInstanceConsumerCommand, TestInstanceCommandConsumer>(command));

            // Assert
            Assert.True(await tester.Harness.Published.Any<TestConsumerFaultEvent>(tester.Harness.CancellationToken));
        }
    }

    public sealed class TestRequest : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<TestRequestConsumer>();

        [Fact]
        public async Task Should_consume_request_and_return_response()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var request = new TestConsumerRequest(Guid.NewGuid());

            // Act
            var response = await tester.TestRequest<TestConsumerResponse, TestConsumerRequest>(request);

            // Assert
            response.RequestError.Should().BeNull();
            response.RequestId.Should().Be(request.RequestId);
        }

        [Fact]
        public async Task Should_return_response_with_error_info_on_failure()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var request = new TestConsumerRequest(Guid.NewGuid())
            {
                ThrowException = true
            };

            // Act
            var response = await tester.TestRequest<TestConsumerResponse, TestConsumerRequest>(request);

            // Assert
            response.RequestError.Should().NotBeNull();
            response.RequestId.Should().Be(request.RequestId);
        }

        [Fact]
        public async Task Should_return_without_waiting_for_bus_inactivity()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);
            var request = new TestConsumerRequest(Guid.NewGuid());
            var stopwatch = Stopwatch.StartNew();

            // Act
            await tester.TestRequest<TestConsumerResponse, TestConsumerRequest>(request);

            // Assert
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task Should_keep_timeouts_set_before_the_call()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromMilliseconds(300);
            tester.Harness.TestTimeout = TimeSpan.FromSeconds(20);
            var request = new TestConsumerRequest(Guid.NewGuid());

            // Act
            await tester.TestRequest<TestConsumerResponse, TestConsumerRequest>(request);

            // Assert
            tester.Harness.TestInactivityTimeout.Should().Be(TimeSpan.FromMilliseconds(300));
            tester.Harness.TestTimeout.Should().Be(TimeSpan.FromSeconds(20));
        }
    }

    public sealed class DelayedConsumer : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg => cfg.AddConsumer<DelayedCommandConsumer>();

        [Fact]
        public async Task Should_return_without_waiting_for_bus_inactivity()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);
            var stopwatch = Stopwatch.StartNew();

            // Act
            await tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(new DelayedCommand());

            // Assert
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task Should_wait_for_a_consumer_that_is_busy_longer_than_the_inactivity_timeout()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromMilliseconds(50);
            var command = new DelayedCommand { DelayMilliseconds = 300 };

            // Act
            var handled = await tester.TestCommand<DelayedCommand, DelayedCommandConsumer, DelayedCommandHandled>(command);

            // Assert
            handled.CorrelationId.Should().Be(command.CorrelationId);
        }

        [Fact]
        public async Task Should_throw_the_consumer_exception_of_a_second_call()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            await tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(new DelayedCommand());
            var command = new DelayedCommand { DelayMilliseconds = 300, ThrowException = true };

            // Act
            var act = () => tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(command);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Delayed failure");
        }

        [Fact]
        public async Task Should_wait_for_the_consumers_fault_handling()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var command = new DelayedCommand { DelayMilliseconds = 200, ThrowException = true };

            // Act
            var act = () => tester.TestCommandFault<DelayedCommand, DelayedCommandConsumer>(command);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            (await tester.Harness.Published.Any<DelayedCommandFailed>(k => k.Context.Message.CorrelationId == command.CorrelationId,
                tester.Harness.CancellationToken)).Should().BeTrue();
        }

        [Fact]
        public async Task Should_wait_for_another_consumer_of_a_published_event()
        {
            // Arrange
            await using var tester = new MassTransitTester(cfg =>
            {
                cfg.AddConsumer<DelayedCommandConsumer>();
                cfg.AddConsumer<DelayedCommandHandledConsumer>();
            });
            var command = new DelayedCommand();

            // Act
            await tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(command);

            // Assert
            var consumerHarness = tester.Harness.GetConsumerHarness<DelayedCommandHandledConsumer>();
            (await consumerHarness.Consumed.Any<DelayedCommandHandled>(tester.Harness.CancellationToken)).Should().BeTrue();
        }

        [Fact]
        public async Task Should_throw_a_timeout_exception_if_the_consumer_takes_longer_than_the_test_timeout()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestTimeout = TimeSpan.FromMilliseconds(200);
            var command = new DelayedCommand { DelayMilliseconds = 1_000 };

            // Act
            var act = () => tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(command);

            // Assert
            await act.Should().ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task Should_not_wait_for_a_missing_message_after_the_call()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);
            await tester.TestCommand<DelayedCommand, DelayedCommandConsumer>(new DelayedCommand());
            var stopwatch = Stopwatch.StartNew();

            // Act
            var failed = await tester.Harness.Published.Any<DelayedCommandFailed>(tester.Harness.CancellationToken);

            // Assert
            failed.Should().BeFalse();
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
        }
    }

    public sealed class TestExecuteActivity : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices =
            cfg => cfg.AddExecuteActivity<DelayedActivity, DelayedActivityArguments>();

        [Fact]
        public async Task Should_return_without_waiting_for_bus_inactivity()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);
            var stopwatch = Stopwatch.StartNew();

            // Act
            await tester.TestExecuteActivity<DelayedActivity, DelayedActivityArguments>(new DelayedActivityArguments());

            // Assert
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task Should_wait_for_an_activity_that_is_busy_longer_than_the_inactivity_timeout()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            tester.Harness.TestInactivityTimeout = TimeSpan.FromMilliseconds(50);
            var arguments = new DelayedActivityArguments { DelayMilliseconds = 300 };

            // Act
            await tester.TestExecuteActivity<DelayedActivity, DelayedActivityArguments>(arguments);

            // Assert
            (await tester.Harness.Sent.Any<RoutingSlipCompleted>(tester.Harness.CancellationToken)).Should().BeTrue();
        }

        [Fact]
        public async Task Should_wait_until_the_routing_slip_of_a_failing_activity_has_faulted()
        {
            // Arrange
            await using var tester = new MassTransitTester(_configureServices);
            var arguments = new DelayedActivityArguments { DelayMilliseconds = 300, ThrowException = true };

            // Act
            await tester.TestExecuteActivity<DelayedActivity, DelayedActivityArguments>(arguments);

            // Assert
            (await tester.Harness.Sent.Any<RoutingSlipFaulted>(tester.Harness.CancellationToken)).Should().BeTrue();
        }
    }
}
