using Sdk.Messaging;

namespace Sdk.UserManagement.Requests;

/// <summary>
/// Represents a request to retrieve all user roles.
/// </summary>
public sealed record GetRoles : IRequest<GetRolesResponse>;
