using Sdk.Messaging;
using Sdk.UserManagement.Contracts;

namespace Sdk.UserManagement.Requests;

/// <summary>
/// Represents the response to a <see cref="GetRoles"/> request.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GetRolesResponse(List<Role> Roles, ErrorInfo? RequestError = null) : IResponse;
