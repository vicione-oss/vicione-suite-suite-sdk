using NSubstitute;
using Sdk.Client.Infrastructure;
using Sdk.Connections.Contracts;
using Sdk.Connections.Requests;

namespace Sdk.Testing.Client;

public static class IUiMediatorExtensions
{
    public static IUiMediator SetupGetConnections(this IUiMediator mediator,
        List<Connection>? connections = null)
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

    public static IUiMediator SetupGetSingleConnection(this IUiMediator mediator,
        Connection connection)
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
