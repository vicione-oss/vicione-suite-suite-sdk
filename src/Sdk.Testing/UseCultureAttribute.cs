using System.Globalization;
using Xunit.v3;

namespace Sdk.Testing;

/// <summary>
/// Runs the decorated test, or every test of the decorated class, under the given culture and UI culture,
/// restoring the thread's previous cultures afterwards.
/// </summary>
/// <param name="culture">Culture name for <see cref="CultureInfo.CurrentCulture"/>, e.g. <c>de-DE</c>.</param>
/// <param name="uiCulture">Culture name for <see cref="CultureInfo.CurrentUICulture"/>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
[method: SuppressMessage("Design", "CA1019:AvoidUncalledPrivateCode", Justification = "use of lazy")]
public class UseCultureAttribute(string culture, string uiCulture) : BeforeAfterTestAttribute
{
    private readonly Lazy<CultureInfo> _culture = new(() => new CultureInfo(culture, false));
    private readonly Lazy<CultureInfo> _uiCulture = new(() => new CultureInfo(uiCulture, false));

    private CultureInfo? _originalCulture;
    private CultureInfo? _originalUICulture;

    /// <summary>
    /// Gets the culture applied to <see cref="CultureInfo.CurrentCulture"/>; created on first access, without user overrides.
    /// </summary>
    public CultureInfo Culture => _culture.Value;

    /// <summary>
    /// Gets the culture applied to <see cref="CultureInfo.CurrentUICulture"/>; created on first access, without user overrides.
    /// </summary>
    public CultureInfo UiCulture => _uiCulture.Value;

    /// <summary>
    /// Uses <paramref name="culture"/> for both <see cref="Culture"/> and <see cref="UiCulture"/>.
    /// </summary>
    public UseCultureAttribute(string culture)
        : this(culture, culture)
    {
    }

    /// <summary>
    /// Saves the current thread's cultures and applies <see cref="Culture"/> and <see cref="UiCulture"/>.
    /// </summary>
    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
        _originalUICulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentCulture = Culture;
        Thread.CurrentThread.CurrentUICulture = UiCulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }

    /// <summary>
    /// Restores the cultures saved by <see cref="Before"/>.
    /// </summary>
    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (_originalCulture is not null)
            Thread.CurrentThread.CurrentCulture = _originalCulture;

        if (_originalUICulture is not null)
            Thread.CurrentThread.CurrentUICulture = _originalUICulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }
}
