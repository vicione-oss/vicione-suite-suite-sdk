using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Requests;

/// <summary>
/// Represents the response to a <see cref="GetUserInformation"/> request.
/// </summary>
public sealed record GetUserInformationResponse(List<UserInformation> Users, ErrorInfo? RequestError = null) : IResponse;
