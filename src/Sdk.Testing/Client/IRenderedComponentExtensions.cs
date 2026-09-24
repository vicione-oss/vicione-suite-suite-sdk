using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Sdk.Client.Components.Settings;
using ViciOne.Ui.Blazor.Components.CheckBox;
using ViciOne.Ui.Blazor.Components.ComboBox;
using ViciOne.Ui.Blazor.Components.SpinEdit;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using ViciOne.Ui.MonochromeIcons.Core.Extensions;

namespace Sdk.Testing.Client;

/// <summary>
/// Provides extension methods for bUnit's <see cref="IRenderedComponent{T}"/> to simplify component testing.
/// </summary>
public static class IRenderedComponentExtensions
{
    extension<T>(IRenderedComponent<T> page) where T : IComponent
    {
        /// <summary>
        /// Finds the only button whose inner HTML contains <paramref name="iconCss"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no button or more than one button matches.</exception>
        public IElement FindIconButton(string iconCss) => page.FindAll("button")
            .Single(k => k.InnerHtml.Contains(iconCss, StringComparison.Ordinal));

        /// <summary>
        /// Finds the first grid action button showing <paramref name="iconName"/> at <paramref name="size"/>.
        /// </summary>
        /// <param name="iconName">The icon to look for.</param>
        /// <param name="size">The icon size; <see langword="null"/> means <see cref="MonochromeIconSize.SmallMedium"/>.</param>
        /// <exception cref="ElementNotFoundException">Thrown if no button with the specified icon is found.</exception>
        public IElement FindGridActionButton(MonochromeIconName iconName, MonochromeIconSize? size = null)
        {
            var iconSize = size ?? MonochromeIconSize.SmallMedium;
            var iconCssClasses = iconName.GetCssClasses(iconSize).ToSpaceSeparated();

            var button = page
                .FindAll(".grid-action-button")
                .FirstOrDefault(bt => bt.FirstElementChild?.ClassName?.Contains(iconCssClasses, StringComparison.Ordinal) ?? false);

            return button ?? throw new ElementNotFoundException($"Element with css class attribute containing '{iconCssClasses}' not found");
        }
    }

    extension(IRenderedComponent<IComponent> component)
    {
        /// <summary>
        /// Returns the <typeparamref name="TChild"/> inside the first <see cref="SettingsField"/> labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no settings field has that label.</exception>
        public IRenderedComponent<TChild> GetSettingsFieldChild<TChild>(string label)
            where TChild : IComponent
        {
            var field = component
                .FindComponents<SettingsField>()
                .First(k => Equals(k.Instance.Label, label));

            return field.FindComponent<TChild>();
        }

        /// <summary>
        /// Gets the <see cref="SettingsFieldTextBox"/> component associated with a specific label.
        /// </summary>
        public IRenderedComponent<SettingsFieldTextBox> GetSettingsFieldTextBox(string label)
            => component.GetSettingsFieldChild<SettingsFieldTextBox>(label);

        /// <summary>
        /// Gets the <see cref="CheckBox{T}"/> component associated with a specific label.
        /// </summary>
        public IRenderedComponent<CheckBox<bool>> GetSettingsFieldCheckBox(string label)
            => component.GetSettingsFieldChild<CheckBox<bool>>(label);

        /// <summary>
        /// Gets the <see cref="SettingsFieldComboBox{TItem, TValue}"/> component associated with a specific label.
        /// </summary>
        public IRenderedComponent<SettingsFieldComboBox<TItem, TValue>> GetSettingsFieldComboBox<TItem, TValue>(string label)
            => component.GetSettingsFieldChild<SettingsFieldComboBox<TItem, TValue>>(label);

        /// <summary>
        /// Gets the <see cref="SpinEdit{TValue, TInterval, TLimit}"/> component associated with a specific label.
        /// </summary>
        public IRenderedComponent<SpinEdit<TValue, TInterval, TLimit>> GetSettingsFieldSpinEdit<TValue, TInterval, TLimit>(string label)
            => component.GetSettingsFieldChild<SpinEdit<TValue, TInterval, TLimit>>(label);

        /// <summary>
        /// Asserts that the value of a <see cref="SettingsFieldTextBox"/> with a specific label matches the expected value.
        /// </summary>
        public void AssertSettingsFieldTextBox(string label, string? expectedValue)
            => component.GetSettingsFieldTextBox(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts that the value of a <see cref="CheckBox{T}"/> with a specific label matches the expected value.
        /// </summary>
        public void AssertSettingsFieldCheckBox(string label, bool expectedValue)
            => component.GetSettingsFieldCheckBox(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts that the value of a <see cref="SettingsFieldComboBox{TItem, TValue}"/> with a specific label matches the expected value.
        /// </summary>
        public void AssertSettingsFieldComboBox<TItem, TValue>(string label, TValue expectedValue)
            => component.GetSettingsFieldComboBox<TItem, TValue>(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts the value of a <see cref="SettingsFieldComboBox{TItem, TValue}"/> with a specific label, for a combo box
        /// whose items are <see cref="ComboBoxItem{TValue, TDisplay}"/> with a <see cref="string"/> display.
        /// </summary>
        public void AssertSettingsFieldComboBoxWithItem<TValue>(string label, TValue expectedValue)
            => component.GetSettingsFieldComboBox<ComboBoxItem<TValue, string>, TValue>(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts the value of an <see cref="int"/> <see cref="SpinEdit{TValue, TInterval, TLimit}"/> with a specific label.
        /// </summary>
        public void AssertSettingsFieldSpinEditInt(string label, int expectedValue)
            => component.GetSettingsFieldSpinEdit<int, int, int>(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts that the value of a <see cref="SpinEdit{TValue, TInterval, TLimit}"/> with a specific label matches the expected value.
        /// </summary>
        public void AssertSettingsFieldSpinEditInt<TValue, TInterval, TLimit>(string label, TValue expectedValue)
            => component.GetSettingsFieldSpinEdit<TValue, TInterval, TLimit>(label).Instance.Value.Should().Be(expectedValue);
    }

    extension<TComponent>(IRenderedComponent<TComponent> component) where TComponent : IComponent
    {
        /// <summary>
        /// Raises <c>oninput</c> on the selection checkbox of the first data row, skipping the header row.
        /// </summary>
        /// <exception cref="ElementNotFoundException">Thrown if the grid has no selection column.</exception>
        [Obsolete("No replacement; implement your own helper if needed. This method will be removed in the next major version.")]
        public void TriggerGridFirstRowSelectionChange(bool select) => component.FindAllGridSelectRows()
            .First()
            .GetGridSelectColumn()
            .TriggerOnInputEvent(select);

        /// <summary>
        /// Raises <c>oninput</c> on the selection checkbox of each listed row. Index 0 is the header row,
        /// so data rows start at 1.
        /// </summary>
        /// <exception cref="ElementNotFoundException">Thrown if a specified row index is out of range.</exception>
        [Obsolete("No replacement; implement your own helper if needed. This method will be removed in the next major version.")]
        public void TriggerGridRowSelectionChange(bool select, params IEnumerable<int> rowIndexes)
        {
            var rows = component.FindAllGridSelectRows(true).ToArray();

            foreach (var rowIndex in rowIndexes)
            {
                if (rowIndex < 0 || rowIndex >= rows.Length)
                    throw new ElementNotFoundException($"RowIndex {rowIndex} is out of row range of {rows.Length}");

                rows[rowIndex]
                    .GetGridSelectColumn()
                    .TriggerOnInputEvent(select);
            }
        }

        private IEnumerable<IElement> FindAllGridSelectRows(bool includeHeaderRow = false)
        {
            const string ItemSelectCss = ".item-select-column";

            var rows = component.FindAll(ItemSelectCss);
            if (!rows.Any())
                throw new ElementNotFoundException($"No Grid rows with class '{ItemSelectCss}' found.");

            if (includeHeaderRow)
                return rows;

            return rows.Skip(1);
        }
    }

    extension(IElement element)
    {
        private IElement GetGridSelectColumn()
            => element.FindDescendant<IHtmlInputElement>() ?? throw new ElementNotFoundException("Grid select checkbox not found.");

        private void TriggerOnInputEvent(bool value)
            => element.TriggerEvent("oninput", new ChangeEventArgs
            {
                Value = value
            });
    }
}
