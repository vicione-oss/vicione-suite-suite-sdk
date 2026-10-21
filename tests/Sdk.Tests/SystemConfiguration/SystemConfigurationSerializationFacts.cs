using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using AwesomeAssertions;
using Sdk.Messaging;
using Sdk.SystemConfiguration.Contracts;
using SysConfig = Sdk.SystemConfiguration.Contracts.SystemConfiguration;

namespace Sdk.Tests.SystemConfiguration;

/// <summary>
/// Ensures that <see cref="SysConfig"/> can be serialized and deserialized using the source-generated
/// <see cref="SystemConfigurationSourceGenerationContext"/>, and keeps its defaults without it.
/// </summary>
public sealed class SystemConfigurationSerializationFacts
{
    // Mirrors CreateFullyPopulatedConfiguration; a difference between the two is a change of the JSON format.
    private const string FullyPopulatedJson =
        """
        {
          "Version": 1,
          "NetworkInterfaces": [
            {
              "Name": "lan1",
              "PhysicalAddress": "00:11:22:33:44:55",
              "Enabled": true,
              "IPv4Address": "192.168.1.100",
              "IPv4Netmask": "255.255.255.0",
              "IPv4Gateway": "192.168.1.1",
              "DhcpLease": {
                "LeaseObtained": "2026-02-24T10:00:00+01:00",
                "LeaseExpires": "2026-02-25T10:00:00+01:00"
              },
              "Vlan": { "Id": 42 },
              "AdditionalAddresses": [
                { "AddressFamily": "InterNetwork", "IpAddress": "10.0.0.5", "Netmask": "255.255.255.0" },
                { "AddressFamily": "InterNetworkV6", "IpAddress": "2001:4860:4860::8888", "Netmask": "2001:db8:abcd::12" }
              ]
            }
          ],
          "Dns": {
            "Hostname": "myhost",
            "MulticastDnsEnabled": true,
            "NameServers": ["8.8.8.8", "1.1.1.1"],
            "DnsSuffix": "example.local",
            "SearchDomains": ["search.local"],
            "StaticHosts": [
              { "IpAddress": "127.0.0.1", "Hostname": "localhost" }
            ]
          },
          "Proxy": {
            "Http": { "Server": "proxy.example.com", "Port": 8080, "Username": "user", "Password": "pass" },
            "Https": { "Server": "proxy.example.com", "Port": 8443, "Username": null, "Password": null },
            "Ftp": null,
            "Sftp": null,
            "Socks": null,
            "DoNotProxyList": ["192.168.1.0/24", "internal.example.com"]
          },
          "Ntp": {
            "Servers": ["0.pool.ntp.org", "1.pool.ntp.org"],
            "FallbackServers": ["2.pool.ntp.org"]
          },
          "Services": [
            { "Name": "nginx", "State": "Enabled" },
            { "Name": "ssh", "State": "Disabled" }
          ]
        }
        """;

    private static readonly JsonSerializerOptions s_jsonOptions =
        SystemConfigurationSourceGenerationContext.Default.Options;

    // The SDK's shared options do not use the context, so they exercise the reflection-based serializer.
    private static readonly JsonSerializerOptions s_reflectionOptions = DefaultJsonSerializerSettings.Default;

    [Fact]
    public void Default_SystemConfiguration_roundtrips_through_json()
    {
        // Arrange
        var config = new SysConfig();

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void Fully_populated_SystemConfiguration_roundtrips_through_json()
    {
        // Arrange
        var config = CreateFullyPopulatedConfiguration();

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void NetworkInterface_with_dhcp_lease_roundtrips_through_json()
    {
        // Arrange
        var iface = new NetworkInterface
        {
            Name = "lan1",
            PhysicalAddress = "00:11:22:33:44:55",
            Enabled = true,
            IPv4Address = IPAddress.Parse("192.168.1.100"),
            IPv4Netmask = IPAddress.Parse("255.255.255.0"),
            IPv4Gateway = IPAddress.Parse("192.168.1.1"),
            DhcpLease = new DhcpLeaseInfo
            {
                LeaseObtained = new DateTimeOffset(2026, 2, 24, 10, 0, 0, TimeSpan.FromHours(1)),
                LeaseExpires = new DateTimeOffset(2026, 2, 25, 10, 0, 0, TimeSpan.FromHours(1)),
            },
        };

        var config = new SysConfig
        {
            NetworkInterfaces = [iface],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.NetworkInterfaces.Should().HaveCount(1);
        deserialized.NetworkInterfaces[0].DhcpLease.Should().NotBeNull();
        deserialized.NetworkInterfaces[0].DhcpLease!.LeaseObtained.Should().Be(iface.DhcpLease.LeaseObtained);
        deserialized.NetworkInterfaces[0].DhcpLease!.LeaseExpires.Should().Be(iface.DhcpLease.LeaseExpires);
    }

    [Fact]
    public void NetworkInterface_without_dhcp_lease_has_null_after_roundtrip()
    {
        // Arrange
        var iface = new NetworkInterface
        {
            Name = "lan1",
            IPv4Address = IPAddress.Parse("10.0.0.1"),
            IPv4Netmask = IPAddress.Parse("255.255.0.0"),
        };

        var config = new SysConfig
        {
            NetworkInterfaces = [iface],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.NetworkInterfaces[0].DhcpLease.Should().BeNull();
    }

    [Fact]
    public void NetworkInterface_with_vlan_roundtrips_through_json()
    {
        // Arrange
        var iface = new NetworkInterface
        {
            Name = "lan1",
            Vlan = new VlanInfo { Id = 100 },
        };

        var config = new SysConfig
        {
            NetworkInterfaces = [iface],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.NetworkInterfaces[0].Vlan.Should().NotBeNull();
        deserialized.NetworkInterfaces[0].Vlan!.Id.Should().Be(100);
    }

    [Fact]
    public void NetworkInterface_without_vlan_has_null_after_roundtrip()
    {
        // Arrange
        var iface = new NetworkInterface { Name = "lan1" };

        var config = new SysConfig
        {
            NetworkInterfaces = [iface],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.NetworkInterfaces[0].Vlan.Should().BeNull();
    }

    [Fact]
    public void Proxy_settings_with_null_proxies_roundtrip_through_json()
    {
        // Arrange
        var config = new SysConfig
        {
            Proxy = new ProxySettings(),
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.Proxy.Http.Should().BeNull();
        deserialized.Proxy.Https.Should().BeNull();
        deserialized.Proxy.Ftp.Should().BeNull();
        deserialized.Proxy.Sftp.Should().BeNull();
#pragma warning disable CS0618 // Covers the property until its removal in the next major version.
        deserialized.Proxy.Socks.Should().BeNull();
#pragma warning restore CS0618
    }

    [Fact]
    public void ServiceState_enum_is_serialized_as_string()
    {
        // Arrange
        var config = new SysConfig
        {
            Services =
            [
                new ServiceInfo { Name = "myservice", State = ServiceState.Enabled },
            ],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);

        // Assert
        json.Should().Contain("\"Enabled\"");
    }

    [Fact]
    public void Dns_settings_with_nameservers_roundtrip_through_json()
    {
        // Arrange
        var config = new SysConfig
        {
            Dns = new DnsSettings
            {
                Hostname = "myhost",
                MulticastDnsEnabled = true,
                NameServers = [IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.4.4")],
                DnsSuffix = "example.local",
                SearchDomains = ["search1.local", "search2.local"],
                StaticHosts =
                [
                    new StaticHost
                    {
                        IpAddress = IPAddress.Parse("127.0.0.1"),
                        Hostname = "localhost",
                    },
                ],
            },
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.Dns.Hostname.Should().Be("myhost");
        deserialized.Dns.MulticastDnsEnabled.Should().BeTrue();
        deserialized.Dns.NameServers.Should().HaveCount(2);
        deserialized.Dns.DnsSuffix.Should().Be("example.local");
        deserialized.Dns.SearchDomains.Should().HaveCount(2);
        deserialized.Dns.StaticHosts.Should().HaveCount(1);
        deserialized.Dns.StaticHosts[0].Hostname.Should().Be("localhost");
    }

    [Fact]
    public void Additional_addresses_roundtrip_through_json()
    {
        // Arrange
        var iface = new NetworkInterface
        {
            Name = "lan1",
            AdditionalAddresses =
            [
                new IpAddressInfo
                {
                    AddressFamily = AddressFamily.InterNetwork,
                    IpAddress = IPAddress.Parse("10.0.0.2"),
                    Netmask = IPAddress.Parse("255.255.255.0"),
                },
                new IpAddressInfo
                {
                    AddressFamily = AddressFamily.InterNetwork,
                    IpAddress = IPAddress.Parse("10.0.0.3"),
                    Netmask = IPAddress.Parse("255.255.255.0"),
                },
            ],
        };

        var config = new SysConfig
        {
            NetworkInterfaces = [iface],
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.NetworkInterfaces[0].AdditionalAddresses.Should().HaveCount(2);
        deserialized.NetworkInterfaces[0].AdditionalAddresses[0].IpAddress.Should()
            .Be(IPAddress.Parse("10.0.0.2"));
    }

    [Fact]
    public void IPv6_dns_addresses_roundtrip_through_json()
    {
        // Arrange
        var config = new SysConfig
        {
            Dns = new DnsSettings
            {
                NameServers = [IPAddress.Parse("2001:4860:4860::8888"), IPAddress.Parse("2001:4860:4860::8844")],
                StaticHosts =
                [
                    new StaticHost { IpAddress = IPAddress.Parse("::1"), Hostname = "ip6-localhost" },
                ],
            },
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.Dns.NameServers.Should().HaveCount(2);
        deserialized.Dns.NameServers[0].Should().Be(IPAddress.Parse("2001:4860:4860::8888"));
        deserialized.Dns.NameServers[1].Should().Be(IPAddress.Parse("2001:4860:4860::8844"));
        deserialized.Dns.StaticHosts[0].IpAddress.Should().Be(IPAddress.Parse("::1"));
    }

    [Fact]
    public void IPv6_link_local_static_host_roundtrips_through_json()
    {
        // Arrange
        var config = new SysConfig
        {
            Dns = new DnsSettings
            {
                StaticHosts =
                [
                    new StaticHost { IpAddress = IPAddress.Parse("fe80::1"), Hostname = "link-local-host" },
                ],
            },
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.Dns.StaticHosts[0].IpAddress.Should().Be(IPAddress.Parse("fe80::1"));
    }

    [Fact]
    public void Mixed_IPv4_and_IPv6_nameservers_roundtrip_through_json()
    {
        // Arrange
        var config = new SysConfig
        {
            Dns = new DnsSettings
            {
                NameServers =
                [
                    IPAddress.Parse("8.8.8.8"),
                    IPAddress.Parse("2001:4860:4860::8888"),
                    IPAddress.Parse("1.1.1.1"),
                    IPAddress.Parse("2606:4700:4700::1111"),
                ],
            },
        };

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized!.Dns.NameServers.Should().HaveCount(4);
        deserialized.Dns.NameServers[0].AddressFamily.Should().Be(AddressFamily.InterNetwork);
        deserialized.Dns.NameServers[1].AddressFamily.Should().Be(AddressFamily.InterNetworkV6);
        deserialized.Dns.NameServers[2].AddressFamily.Should().Be(AddressFamily.InterNetwork);
        deserialized.Dns.NameServers[3].AddressFamily.Should().Be(AddressFamily.InterNetworkV6);
    }

    [Fact]
    public void Fully_populated_SystemConfiguration_is_written_as_the_reference_document()
    {
        // Arrange
        var config = CreateFullyPopulatedConfiguration();
        var expected = JsonNode.Parse(FullyPopulatedJson);

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);

        // Assert
        JsonNode.DeepEquals(JsonNode.Parse(json), expected).Should().BeTrue(json);
    }

    [Fact]
    public void Fully_populated_SystemConfiguration_is_read_from_the_reference_document()
    {
        // Arrange
        var expected = CreateFullyPopulatedConfiguration();

        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>(FullyPopulatedJson, s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1"}]}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","IPv4Address":null,"IPv4Netmask":null,"IPv4Gateway":null}]}""")]
    public void Missing_or_null_optional_address_is_read_as_null(string json)
    {
        // Arrange
        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        var networkInterface = deserialized!.NetworkInterfaces.Should().ContainSingle().Subject;
        networkInterface.IPv4Address.Should().BeNull();
        networkInterface.IPv4Netmask.Should().BeNull();
        networkInterface.IPv4Gateway.Should().BeNull();
    }

    [Fact]
    public void Document_with_only_a_version_is_read_with_defaults()
    {
        // Arrange
        var expected = new SysConfig();

        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>("""{"Version":1}""", s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Nested_objects_without_optional_members_are_read_with_defaults()
    {
        // Arrange
        var expected = new SysConfig
        {
            NetworkInterfaces = [new NetworkInterface { Name = "lan1" }],
        };

        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>(
            """{"Version":1,"NetworkInterfaces":[{"Name":"lan1"}],"Dns":{},"Proxy":{},"Ntp":{}}""", s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("""{}""")]
    [InlineData("""{"NetworkInterfaces":[]}""")]
    public void Document_without_version_is_rejected(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*missing required properties*Version*");
    }

    [Theory]
    [InlineData("""{"Version":1,"Unknown":1}""", """{"Version":1}""")]
    [InlineData("""{"Version":1,"version":2}""", """{"Version":1}""")]
    [InlineData("""{"Version":1,"Dns":{"Hostname":"myhost","Unknown":1}}""", """{"Version":1,"Dns":{"Hostname":"myhost"}}""")]
    [InlineData(
        """{"Version":1,"NetworkInterfaces":[{"Name":"lan1","Unknown":{"Nested":[1]}}]}""",
        """{"Version":1,"NetworkInterfaces":[{"Name":"lan1"}]}""")]
    public void Unknown_member_is_ignored(string json, string jsonWithoutUnknownMember)
    {
        // Arrange
        var expected = JsonSerializer.Deserialize<SysConfig>(jsonWithoutUnknownMember, s_jsonOptions);

        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{}]}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","Vlan":{}}]}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","AdditionalAddresses":[{"AddressFamily":"InterNetwork","IpAddress":"10.0.0.2"}]}]}""")]
    [InlineData("""{"Version":1,"Dns":{"StaticHosts":[{"Hostname":"localhost"}]}}""")]
    [InlineData("""{"Version":1,"Proxy":{"Http":{"Server":"proxy.example.com"}}}""")]
    [InlineData("""{"Version":1,"Services":[{"Name":"ssh"}]}""")]
    public void Document_without_required_member_is_rejected(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*missing required properties*");
    }

    [Theory]
    [InlineData("""{"Version":1,"NetworkInterfaces":null}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":null}]}""")]
    [InlineData("""{"Version":1,"Dns":null}""")]
    [InlineData("""{"Version":1,"Dns":{"NameServers":null}}""")]
    [InlineData("""{"Version":1,"Dns":{"StaticHosts":[{"IpAddress":null,"Hostname":"localhost"}]}}""")]
    [InlineData("""{"Version":1,"Proxy":null}""")]
    [InlineData("""{"Version":1,"Ntp":{"Servers":null}}""")]
    public void Document_with_null_for_non_nullable_member_is_rejected(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*doesn't allow null values*");
    }

    [Theory]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","IPv4Address":""}]}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","IPv4Address":"not-an-address"}]}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":[{"Name":"lan1","IPv4Address":5}]}""")]
    [InlineData("""{"Version":1,"Dns":{"NameServers":"8.8.8.8"}}""")]
    [InlineData("""{"Version":1,"Dns":{"NameServers":["not-an-address"]}}""")]
    [InlineData("""{"Version":1,"Services":[{"Name":"ssh","State":"Bogus"}]}""")]
    public void Document_with_malformed_value_is_rejected(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<SysConfig>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Null_assigned_to_a_defaulted_member_is_replaced_by_its_default()
    {
        // Arrange
        var expected = new SysConfig
        {
            NetworkInterfaces = [new NetworkInterface { Name = "lan1" }],
        };

        // Act
        var config = new SysConfig
        {
            NetworkInterfaces = [new NetworkInterface { Name = "lan1", PhysicalAddress = null!, AdditionalAddresses = null! }],
            Dns = new DnsSettings
            {
                Hostname = null!,
                NameServers = null!,
                DnsSuffix = null!,
                SearchDomains = null!,
                StaticHosts = null!,
            },
            Proxy = new ProxySettings { DoNotProxyList = null! },
            Ntp = new NtpSettings { Servers = null!, FallbackServers = null! },
            Services = null!,
        };

        // Assert
        config.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Null_assigned_through_a_with_expression_is_replaced_by_its_default()
    {
        // Arrange
        var config = CreateFullyPopulatedConfiguration();

        // Act
        var result = config with { NetworkInterfaces = null!, Dns = null!, Proxy = null!, Ntp = null!, Services = null! };

        // Assert
        result.Should().BeEquivalentTo(new SysConfig());
    }

    [Fact]
    public void Fully_populated_SystemConfiguration_roundtrips_through_a_reflection_based_serializer()
    {
        // Arrange
        var config = CreateFullyPopulatedConfiguration();

        // Act
        var json = JsonSerializer.Serialize(config, s_reflectionOptions);
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_reflectionOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(config);
    }

    [Theory]
    [InlineData("""{"Version":1}""")]
    [InlineData("""{"Version":1,"NetworkInterfaces":null,"Dns":null,"Proxy":null,"Ntp":null,"Services":null}""")]
    public void Reflection_based_serializer_reads_missing_and_null_members_with_defaults(string json)
    {
        // Arrange
        var expected = new SysConfig();

        // Act
        var deserialized = JsonSerializer.Deserialize<SysConfig>(json, s_reflectionOptions);

        // Assert
        deserialized.Should().BeEquivalentTo(expected);
    }

    private static SysConfig CreateFullyPopulatedConfiguration() => new()
    {
        Version = 1,
        NetworkInterfaces =
        [
            new NetworkInterface
            {
                Name = "lan1",
                PhysicalAddress = "00:11:22:33:44:55",
                Enabled = true,
                IPv4Address = IPAddress.Parse("192.168.1.100"),
                IPv4Netmask = IPAddress.Parse("255.255.255.0"),
                IPv4Gateway = IPAddress.Parse("192.168.1.1"),
                DhcpLease = new DhcpLeaseInfo
                {
                    LeaseObtained = new DateTimeOffset(2026, 2, 24, 10, 0, 0, TimeSpan.FromHours(1)),
                    LeaseExpires = new DateTimeOffset(2026, 2, 25, 10, 0, 0, TimeSpan.FromHours(1)),
                },
                Vlan = new VlanInfo { Id = 42 },
                AdditionalAddresses =
                [
                    new IpAddressInfo
                    {
                        AddressFamily = AddressFamily.InterNetwork,
                        IpAddress = IPAddress.Parse("10.0.0.5"),
                        Netmask = IPAddress.Parse("255.255.255.0"),
                    },
                    new IpAddressInfo
                    {
                        AddressFamily = AddressFamily.InterNetworkV6,
                        IpAddress = IPAddress.Parse("2001:4860:4860::8888"),
                        Netmask = IPAddress.Parse("2001:db8:abcd::0012"),
                    },
                ],
            },
        ],
        Dns = new DnsSettings
        {
            Hostname = "myhost",
            MulticastDnsEnabled = true,
            NameServers = [IPAddress.Parse("8.8.8.8"), IPAddress.Parse("1.1.1.1")],
            DnsSuffix = "example.local",
            SearchDomains = ["search.local"],
            StaticHosts =
            [
                new StaticHost { IpAddress = IPAddress.Parse("127.0.0.1"), Hostname = "localhost" },
            ],
        },
        Proxy = new ProxySettings
        {
            Http = new ProxyInfo { Server = "proxy.example.com", Port = 8080, Username = "user", Password = "pass" },
            Https = new ProxyInfo { Server = "proxy.example.com", Port = 8443 },
            DoNotProxyList = ["192.168.1.0/24", "internal.example.com"],
        },
        Ntp = new NtpSettings
        {
            Servers = ["0.pool.ntp.org", "1.pool.ntp.org"],
            FallbackServers = ["2.pool.ntp.org"],
        },
        Services =
        [
            new ServiceInfo { Name = "nginx", State = ServiceState.Enabled },
            new ServiceInfo { Name = "ssh", State = ServiceState.Disabled },
        ],
    };
}

