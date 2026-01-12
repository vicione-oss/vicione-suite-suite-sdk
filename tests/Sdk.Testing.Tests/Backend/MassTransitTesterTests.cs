using AwesomeAssertions;
using MassTransit;
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
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestCommandConsumer>();
        };

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
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestCommandConsumer>();
        };

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
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestEventConsumer>();
        };

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
            await using var tester = new MassTransitTester(cfg =>
            {
                cfg.AddConsumer<TestCommandConsumer>();
            });
            var testEvent = new TestConsumerEvent(Guid.NewGuid());

            // Act + Assert
            await Assert.ThrowsAsync<AssertionException>(() => tester.TestEvent<TestConsumerEvent, TestCommandConsumer>(testEvent));
        }
    }

    public sealed class TestInstanceDependentCommand : MassTransitTesterTests
    {
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestInstanceCommandConsumer>();
        };

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
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestInstanceCommandConsumer>();
        };

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
        private readonly Action<IBusRegistrationConfigurator> _configureServices = cfg =>
        {
            cfg.AddConsumer<TestRequestConsumer>();
        };

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
    }
}
