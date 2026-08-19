using Sdk.Messaging;

namespace Sdk.UserManagement.Requests;

/// <summary>
/// Represents a request to retrieve one or all users with basic user information.
/// </summary>
/// <param name="UserName">The username of the user to retrieve information for. If null, information for all users will be retrieved.</param>
public sealed record GetUserInformation(string? UserName = null) : IRequest<GetUserInformationResponse>;
