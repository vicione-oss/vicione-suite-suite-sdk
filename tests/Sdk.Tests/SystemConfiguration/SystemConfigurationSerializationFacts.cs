using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using AwesomeAssertions;
using Sdk.SystemConfiguration.Contracts;
using SysConfig = Sdk.SystemConfiguration.Contracts.SystemConfiguration;

namespace Sdk.Tests.SystemConfiguration;

/// <summary>
/// Ensures that <see cref="SysConfig"/> can be serialized and
/// deserialized using the source-generated <see cref="SystemConfigurationSourceGenerationContext"/>.
/// </summary>
public sealed class SystemConfigurationSerializationFacts
{
    private static readonly JsonSerializerOptions s_jsonOptions =
        SystemConfigurationSourceGenerationContext.Default.Options;

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
    public void Serialized_json_contains_expected_property_names()
    {
        // Arrange
        var config = CreateFullyPopulatedConfiguration();

        // Act
        var json = JsonSerializer.Serialize(config, s_jsonOptions);

        // Assert
        json.Should().Contain("\"Version\":");
        json.Should().Contain("\"NetworkInterfaces\":");
        json.Should().Contain("\"Dns\":");
        json.Should().Contain("\"Proxy\":");
        json.Should().Contain("\"Ntp\":");
        json.Should().Contain("\"Services\":");
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
        deserialized.Proxy.Socks.Should().BeNull();
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

