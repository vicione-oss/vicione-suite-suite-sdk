using Sdk.Client.Infrastructure;
using Sdk.Connections.Contracts;
using Sdk.Connections.Requests;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for <see cref="IUiMediator"/> to simplify setting up mock responses for connection-related requests.
/// </summary>
public static class IUiMediatorExtensions
{
    extension(IUiMediator mediator)
    {
        /// <summary>
        /// Sets up a mocked <see cref="IUiMediator"/> to return a predefined list of connections and their associated tags.
        /// </summary>
        public IUiMediator SetupGetConnections(List<Connection>? connections = null)
        {
            mediator.Request<GetConnections, GetConnectionsResponse>(
                    Arg.Any<GetConnections>(), Arg.Any<CancellationToken>())
                .Returns(new GetConnectionsResponse(connections ?? []));

            var tags = connections?.SelectMany(k => k.Tags).ToList() ?? [];

            mediator.Request<GetTags, GetTagsResponse>(
                    Arg.Any<GetTags>(), Arg.Any<CancellationToken>())
                .Returns(new GetTagsResponse(tags));

            return mediator;
        }

        /// <summary>
        /// Sets up a mocked <see cref="IUiMediator"/> to return a single, specific connection and its associated tags.
        /// </summary>
        public IUiMediator SetupGetSingleConnection(Connection connection)
        {
            mediator.Request<GetConnections, GetConnectionsResponse>(
                    Arg.Any<GetConnections>(), Arg.Any<CancellationToken>())
                .Returns(new GetConnectionsResponse([connection]));

            var tags = connection.Tags.ToList();

            mediator.Request<GetTags, GetTagsResponse>(
                    Arg.Any<GetTags>(), Arg.Any<CancellationToken>())
                .Returns(new GetTagsResponse(tags));

            return mediator;
        }
    }
}
