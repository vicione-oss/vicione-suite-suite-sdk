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
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="byte"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldByteSpinEdit()
            => services.AddByteSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="byte"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableByteSpinEdit()
            => services.AddNullableByteSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="sbyte"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldSignedByteSpinEdit()
            => services.AddSignedByteSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="sbyte"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableSignedByteSpinEdit()
            => services.AddNullableSignedByteSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="ushort"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedShortSpinEdit()
            => services.AddUnsignedShortSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="ushort"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedShortSpinEdit()
            => services.AddNullableUnsignedShortSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="uint"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedIntSpinEdit()
            => services.AddUnsignedIntSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="uint"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedIntSpinEdit()
            => services.AddNullableUnsignedIntSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="ulong"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldUnsignedLongSpinEdit()
            => services.AddUnsignedLongSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="ulong"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableUnsignedLongSpinEdit()
            => services.AddNullableUnsignedLongSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="short"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldShortSpinEdit()
            => services.AddShortSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="short"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableShortSpinEdit()
            => services.AddNullableShortSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="int"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldIntSpinEdit()
            => services.AddIntSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="int"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableIntSpinEdit()
            => services.AddNullableIntSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="long"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldLongSpinEdit()
            => services.AddLongSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="long"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableLongSpinEdit()
            => services.AddNullableLongSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="decimal"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldDecimalSpinEdit()
            => services.AddDecimalSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="decimal"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableDecimalSpinEdit()
            => services.AddNullableDecimalSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="double"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldDoubleSpinEdit()
            => services.AddDoubleSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="double"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableDoubleSpinEdit()
            => services.AddNullableDoubleSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="float"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldFloatSpinEdit()
            => services.AddFloatSpinEdit();

        /// <summary>
        /// Adds the necessary services for <see cref="SettingsFieldSpinEdit{TValue, TInterval, TLimit}"/> for <see cref="Nullable{T}"/> of <see cref="float"/>.
        /// </summary>
        public IServiceCollection AddSettingsFieldNullableFloatSpinEdit()
            => services.AddNullableFloatSpinEdit();
    }
}
