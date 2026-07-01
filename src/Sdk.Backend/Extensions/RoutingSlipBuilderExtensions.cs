using MassTransit;
using Sdk.Backend.Messaging;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IRoutingSlipBuilder"/>.
/// </summary>
public static class RoutingSlipBuilderExtensions
{
    extension(IRoutingSlipBuilder builder)
    {
        /// <summary>
        /// Adds a compensating activity to the routing slip, dispatched to any instance hosting it.
        /// </summary>
        public void AddActivity<TActivity, TArguments, TLog>(TArguments arguments)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class, IActivityArgument
            where TLog : class
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(null), arguments);

        /// <summary>
        /// Adds a compensating activity to the routing slip, dispatched to the given instance.
        /// </summary>
        public void AddActivity<TActivity, TArguments, TLog>(TArguments arguments, Guid executionInstanceId)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class, IInstanceDependentActivityArgument
            where TLog : class
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(executionInstanceId), arguments);

        /// <summary>
        /// Adds an execute-only activity to the routing slip, dispatched to any instance hosting it.
        /// </summary>
        public void AddActivity<TActivity, TArguments>(TArguments arguments)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class, IActivityArgument
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(null), arguments);

        /// <summary>
        /// Adds an execute-only activity to the routing slip, dispatched to the given instance.
        /// </summary>
        public void AddActivity<TActivity, TArguments>(TArguments arguments, Guid executionInstanceId)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class, IInstanceDependentActivityArgument
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(executionInstanceId), arguments);
    }
}
