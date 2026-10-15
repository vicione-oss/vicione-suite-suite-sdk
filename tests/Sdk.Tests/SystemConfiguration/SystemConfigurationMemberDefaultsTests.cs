using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using AwesomeAssertions;
using Sdk.SystemConfiguration.Contracts;
using SysConfig = Sdk.SystemConfiguration.Contracts.SystemConfiguration;

namespace Sdk.Tests.SystemConfiguration;

/// <summary>
/// Walks every member <see cref="SystemConfigurationSourceGenerationContext"/> serializes, because the source generator
/// passes <see langword="null"/> or the type's default for an optional member missing in the JSON.
/// </summary>
/// <remarks>
/// A new member that would lose its initializer fails here without a test of its own.
/// </remarks>
public sealed class SystemConfigurationMemberDefaultsTests
{
    public static readonly TheoryData<Type, string> NonNullableReferenceMembers =
        ToTheoryData(OptionalMembers().Where(p => !p.PropertyType.IsValueType && !p.IsSetNullable));

    public static readonly TheoryData<Type, string> OtherOptionalMembers =
        ToTheoryData(OptionalMembers().Where(p => p.PropertyType.IsValueType || p.IsSetNullable));

    [Theory]
    [MemberData(nameof(NonNullableReferenceMembers))]
    public void Non_nullable_reference_member_replaces_null_with_its_default(Type declaringType, string memberName)
    {
        // Arrange
        var instance = Activator.CreateInstance(declaringType)!;
        var property = declaringType.GetProperty(memberName)!;

        // Act
        property.SetValue(instance, null);

        // Assert
        property.GetValue(instance).Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(OtherOptionalMembers))]
    public void Other_optional_member_is_initialized_with_the_default_of_its_type(Type declaringType, string memberName)
    {
        // Arrange
        var property = declaringType.GetProperty(memberName)!;
        var defaultOfType = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;

        // Act
        var value = property.GetValue(Activator.CreateInstance(declaringType));

        // Assert
        value.Should().Be(defaultOfType);
    }

    [Fact]
    public void Optional_members_are_found_in_every_nested_record()
    {
        // Arrange
        Type[] nestedRecords =
        [
            typeof(SysConfig),
            typeof(NetworkInterface),
            typeof(DhcpLeaseInfo),
            typeof(DnsSettings),
            typeof(ProxySettings),
            typeof(ProxyInfo),
            typeof(NtpSettings),
        ];

        // Act
        var declaringTypes = OptionalMembers().Select(p => p.DeclaringType).Distinct();

        // Assert
        declaringTypes.Should().Contain(nestedRecords);
    }

    private static IEnumerable<JsonPropertyInfo> OptionalMembers()
    {
        var pending = new Queue<Type>([typeof(SysConfig)]);
        var visited = new HashSet<Type>();

        while (pending.TryDequeue(out var type))
        {
            var typeInfo = SystemConfigurationSourceGenerationContext.Default.GetTypeInfo(type);
            if (!visited.Add(type) || typeInfo is null)
                continue;

            if (typeInfo.ElementType is not null)
                pending.Enqueue(typeInfo.ElementType);

            if (typeInfo.Kind != JsonTypeInfoKind.Object)
                continue;

            foreach (var property in typeInfo.Properties)
            {
                // A member with its own converter, such as an IP address, is written as a single value.
                if (property.CustomConverter is null)
                    pending.Enqueue(property.PropertyType);

                if (!property.IsRequired)
                    yield return property;
            }
        }
    }

    private static TheoryData<Type, string> ToTheoryData(IEnumerable<JsonPropertyInfo> members)
    {
        var data = new TheoryData<Type, string>();
        foreach (var member in members)
            data.Add(member.DeclaringType, ((PropertyInfo)member.AttributeProvider!).Name);

        return data;
    }
}
