using Microsoft.Extensions.DependencyInjection;
using ViciOne.Ui.Blazor.Components.SpinEdit.Extensions;

namespace Sdk.Client.Components.Settings.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register services related to settings components.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="byte"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldByteSpinEdit()
            => services.AddByteSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="byte"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableByteSpinEdit()
            => services.AddNullableByteSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="sbyte"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldSignedByteSpinEdit()
            => services.AddSignedByteSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="sbyte"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableSignedByteSpinEdit()
            => services.AddNullableSignedByteSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="ushort"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedShortSpinEdit()
            => services.AddUnsignedShortSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="ushort"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedShortSpinEdit()
            => services.AddNullableUnsignedShortSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="uint"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedIntSpinEdit()
            => services.AddUnsignedIntSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="uint"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedIntSpinEdit()
            => services.AddNullableUnsignedIntSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="ulong"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedLongSpinEdit()
            => services.AddUnsignedLongSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="ulong"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedLongSpinEdit()
            => services.AddNullableUnsignedLongSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="short"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldShortSpinEdit()
            => services.AddShortSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="short"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableShortSpinEdit()
            => services.AddNullableShortSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="int"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldIntSpinEdit()
            => services.AddIntSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="int"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableIntSpinEdit()
            => services.AddNullableIntSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="long"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldLongSpinEdit()
            => services.AddLongSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="long"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableLongSpinEdit()
            => services.AddNullableLongSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="decimal"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldDecimalSpinEdit()
            => services.AddDecimalSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="decimal"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableDecimalSpinEdit()
            => services.AddNullableDecimalSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="double"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldDoubleSpinEdit()
            => services.AddDoubleSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="double"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableDoubleSpinEdit()
            => services.AddNullableDoubleSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for <see cref="float"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldFloatSpinEdit()
            => services.AddFloatSpinEdit();

        /// <summary>
        /// Registers the <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> services for nullable <see cref="float"/> values.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableFloatSpinEdit()
            => services.AddNullableFloatSpinEdit();
    }
}
