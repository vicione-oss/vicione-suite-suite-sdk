using Sdk.Messaging;

namespace Sdk.UserManagement.Requests;

/// <summary>
/// Represents a request to retrieve one or all users with basic user information.
/// </summary>
/// <param name="UserName">The user to look up; <see langword="null"/> returns all users.</param>
public sealed record GetUserInformation(string? UserName = null) : IRequest<GetUserInformationResponse>;
