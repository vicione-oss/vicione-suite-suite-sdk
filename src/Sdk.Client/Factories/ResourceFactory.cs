using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Sdk.Client.Contracts;
using Sdk.Client.Modules;
using Sdk.Modules;

namespace Sdk.Client.Factories;

internal static class ResourceFactory
{
    /// <summary>
    /// creates resource with url /js/filename
    /// </summary>
    public static Resource CreateGlobalScript(string? bundle, string filename) => new()
    {
        ResourceType = ResourceType.Script,
        Bundle = bundle,
        Url = ModuleAssetHelper.GetGlobalJsUrl(filename, true),
        Declaration = ResourceDeclaration.Global
    };

    /// <summary>
    /// creates resource with url /css/filename
    /// </summary>
    public static Resource CreateGlobalStylesheet(string? bundle, string filename) => new()
    {
        Id = GetUniqueId(filename),
        ResourceType = ResourceType.Stylesheet,
        Bundle = bundle,
        Url = ModuleAssetHelper.GetGlobalCssUrl(filename, true),
        Declaration = ResourceDeclaration.Global
    };

    /// <summary>
    /// creates resource with url /_content/moduleDllName/relativeFilePath
    /// </summary>
    public static Resource CreateModuleScript<T>(string? bundle, string relativeFilePath, bool forceGlobal = false)
        where T : IModule => new()
        {
            ResourceType = ResourceType.Script,
            Bundle = bundle,
            Url = ModuleAssetHelper.GetModuleJsUrl<T>(relativeFilePath, true),
            Declaration = forceGlobal ? ResourceDeclaration.Global : ResourceDeclaration.Local
        };

    /// <summary>
    /// creates resource with url /_content/module/relativeFilePath
    /// </summary>
    public static Resource CreateModuleStylesheet<T>(string? bundle, string relativeFilePath, bool forceGlobal = false)
        where T : IModule => new()
        {
            Id = GetUniqueId(relativeFilePath),
            ResourceType = ResourceType.Stylesheet,
            Bundle = bundle,
            Url = ModuleAssetHelper.GetModuleCssUrl<T>(relativeFilePath, true),
            Declaration = forceGlobal ? ResourceDeclaration.Global : ResourceDeclaration.Local
        };

    public static Resource CreateScript(string? bundle, string jsUrl) => new()
    {
        ResourceType = ResourceType.Script,
        Bundle = bundle,
        Url = new Uri(jsUrl, UriKind.Relative),
        Declaration = ResourceDeclaration.Local
    };

    public static Resource CreateStylesheet(string? bundle, string stylesheetUrl) => new()
    {
        Id = GetUniqueId(stylesheetUrl),
        ResourceType = ResourceType.Stylesheet,
        Bundle = bundle,
        Url = new Uri(stylesheetUrl, UriKind.Relative),
        Declaration = ResourceDeclaration.Local
    };

    public static Resource CreateComponentScript(string? bundle, string relativeFilePath, Assembly assembly) =>
        CreateComponentResource(bundle, relativeFilePath, ResourceType.Script, assembly);

    public static Resource CreateComponentStylesheet(string? bundle, string relativeFilePath, Assembly assembly) =>
        CreateComponentResource(bundle, relativeFilePath, ResourceType.Stylesheet, assembly);

    private static Resource CreateComponentResource(string? bundle, string relativeFilePath, ResourceType resource,
        Assembly assembly)
    {
        var moduleName = ModuleAssetHelper.GetModuleManifestName(assembly);

        if (relativeFilePath.StartsWith('/'))
            relativeFilePath = relativeFilePath[1..];

        var url = new Uri($"/{ModuleAssetHelper.ContentPrefix}/{moduleName}/{relativeFilePath}", UriKind.Relative);

        return new Resource { Id = GetUniqueId(relativeFilePath), ResourceType = resource, Bundle = bundle, Url = url };
    }

    [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms")]
    private static uint GetUInt32HashCode(string strText)
    {
        if (string.IsNullOrEmpty(strText)) return 0;

        //Unicode Encode Covering all characterset
        var byteContents = Encoding.Unicode.GetBytes(strText);
        var hashText = SHA1.HashData(byteContents);
        var hashCodeStart = BitConverter.ToUInt32(hashText, 0);
        var hashCodeMedium = BitConverter.ToUInt32(hashText, 8);
        var hashCodeEnd = BitConverter.ToUInt32(hashText, 16);
        var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        return uint.MaxValue - hashCode;
    }

    /// <summary>
    /// used to identify css links by id
    /// </summary>
    private static string GetUniqueId(string filename) =>
        string.Format(CultureInfo.InvariantCulture, "i{0}", GetUInt32HashCode(filename));
}
