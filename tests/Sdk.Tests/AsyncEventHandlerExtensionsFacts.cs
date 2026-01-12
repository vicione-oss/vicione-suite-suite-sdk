using AwesomeAssertions;

namespace Sdk.Tests;

public class AsyncEventHandlerExtensionsFacts
{
    public class Invoke
    {
        private const string ExceptionMessage = "Exception erzwingen";

        private CallbackState? MethodState { get; set; }

        private event Func<TestMessage, Task>? TestEventFuncWithOneParameter;
        private event Func<TestMessage, TestMessage, Task>? TestEventFuncWithTwoParameters;
        private event Func<Task>? TestEventFuncWithoutParameter;

        [Fact]
        public async Task With_one_parameter_acceptance()
        {
            TestEventFuncWithOneParameter += TestMethodWithOneParameter;

            var testMessage = new TestMessage(CallbackState.CallbackSuccessful);
            await TestEventFuncWithOneParameter.Invoke(testMessage);

            Assert.Equal(CallbackState.CallbackSuccessful, testMessage.TestMessageStateAfter);
        }

        [Fact]
        public async Task With_one_parameter_exception_handling()
        {
            TestEventFuncWithOneParameter += TestMethodWithOneParameter;

            ServiceDelegateWithParameter createConnection = TestEventFuncWithOneParameter.Invoke;

            var testMessage = new TestMessage(CallbackState.CallbackException);
            var exception = await Assert.ThrowsAsync<AggregateException>(() => createConnection(testMessage));

            Assert.Contains(ExceptionMessage, exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task With_two_parameters_acceptance()
        {
            TestEventFuncWithTwoParameters += TestMethodWithTwoParameters;

            var firstMessage = new TestMessage(CallbackState.CallbackSuccessful);
            var secondMessage = new TestMessage(CallbackState.CallbackSuccessful);

            await TestEventFuncWithTwoParameters.Invoke(firstMessage, secondMessage);

            firstMessage.TestMessageStateAfter.Should().Be(CallbackState.CallbackSuccessful);
            secondMessage.TestMessageStateAfter.Should().Be(CallbackState.CallbackSuccessful);
        }

        [Theory]
        [InlineData(CallbackState.CallbackSuccessful, CallbackState.CallbackException)]
        [InlineData(CallbackState.CallbackException, CallbackState.CallbackSuccessful)]
        [InlineData(CallbackState.CallbackException, CallbackState.CallbackException)]
        public void With_two_parameters_exception(CallbackState firstState, CallbackState secondState)
        {
            TestEventFuncWithTwoParameters += TestMethodWithTwoParameters;

            var firstMessage = new TestMessage(firstState);
            var secondMessage = new TestMessage(secondState);

            ServiceDelegateTwoParameters createConnection = TestEventFuncWithTwoParameters.Invoke;

            createConnection
                .Invoking(y => y(firstMessage, secondMessage))
                .Should()
                .ThrowAsync<AggregateException>()
                .WithMessage(ExceptionMessage);
        }

        [Fact]
        public async Task Without_parameter_acceptance()
        {
            TestEventFuncWithoutParameter += TestMethodWithoutParameterAcceptance;
            MethodState = CallbackState.CallbackInit;

            await TestEventFuncWithoutParameter.Invoke();

            Assert.Equal(CallbackState.CallbackSuccessful, MethodState);
        }

        [Fact]
        public async Task Without_parameter_exception()
        {
            TestEventFuncWithoutParameter += TestMethodWithoutParameterException;
            MethodState = CallbackState.CallbackInit;

            ServiceDelegateWithoutParameter createConnection = TestEventFuncWithoutParameter.Invoke;

            var exception = await Assert.ThrowsAsync<AggregateException>(() => createConnection());

            Assert.Equal(CallbackState.CallbackException, MethodState);
            Assert.Contains(ExceptionMessage, exception.Message, StringComparison.Ordinal);
        }

        private Task TestMethodWithOneParameter(TestMessage message)
        {
            message.TestMessageStateAfter = message.TestMessageStateBefore == CallbackState.CallbackSuccessful
                ? message.TestMessageStateBefore
                : throw new AggregateException(ExceptionMessage);

            return Task.CompletedTask;
        }

        private Task TestMethodWithTwoParameters(TestMessage firstMessage, TestMessage secondMessage)
        {
            if (firstMessage.TestMessageStateBefore == CallbackState.CallbackSuccessful
                && secondMessage.TestMessageStateBefore == CallbackState.CallbackSuccessful)
            {
                firstMessage.TestMessageStateAfter = CallbackState.CallbackSuccessful;
                secondMessage.TestMessageStateAfter = CallbackState.CallbackSuccessful;
            }
            else
            {
                throw new AggregateException(ExceptionMessage);
            }
            return Task.CompletedTask;
        }

        private Task TestMethodWithoutParameterAcceptance()
        {
            MethodState = CallbackState.CallbackSuccessful;
            return Task.CompletedTask;
        }

        private Task TestMethodWithoutParameterException()
        {
            MethodState = CallbackState.CallbackException;
            throw new AggregateException(ExceptionMessage);
        }

        private delegate Task ServiceDelegateWithParameter(TestMessage message);
        private delegate Task ServiceDelegateTwoParameters(TestMessage first, TestMessage secondMessage);
        private delegate Task ServiceDelegateWithoutParameter();
    }

    public class TestMessage(CallbackState before)
    {
        public CallbackState TestMessageStateBefore { get; set; } = before;
        public CallbackState TestMessageStateAfter { get; set; } = CallbackState.CallbackInit;
    }

    public enum CallbackState
    {
        CallbackInit = 0,
        CallbackSuccessful = 1,
        CallbackException = 2
    }
}
