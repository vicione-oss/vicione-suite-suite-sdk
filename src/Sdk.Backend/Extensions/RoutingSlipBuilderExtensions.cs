using MassTransit;
using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IRoutingSlipBuilder"/>.
/// </summary>
public static class RoutingSlipBuilderExtensions
{
    extension(IRoutingSlipBuilder builder)
    {
        /// <summary>
        /// Ads a compensating activity to the routing slip.
        /// </summary>
        public void AddActivity<TActivity, TArguments, TLog>(TArguments arguments, Guid? executionInstanceId = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class, IActivityArgument
            where TLog : class
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(executionInstanceId), arguments);

        /// <summary>
        /// Adds an execute-only activity to the routing slip.
        /// </summary>
        public void AddActivity<TActivity, TArguments>(TArguments arguments, Guid? executionInstanceId = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class, IActivityArgument
            => builder.AddActivity(typeof(TActivity).FullName ?? typeof(TActivity).Name, MessagingHelper.GetActivityEndpointAddress<TArguments>(executionInstanceId), arguments);
    }
}
