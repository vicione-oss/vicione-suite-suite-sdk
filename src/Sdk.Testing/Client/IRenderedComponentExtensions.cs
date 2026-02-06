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
        /// Finds a button element that contains an icon with the specified CSS class.
        /// </summary>
        public IElement FindIconButton(string iconCss) => page.FindAll("button")
            .Single(k => k.InnerHtml.Contains(iconCss, StringComparison.Ordinal));

        /// <summary>
        /// Finds a grid action button by its monochrome icon.
        /// </summary>
        /// <exception cref="ElementNotFoundException">Thrown if no button with the specified icon is found.</exception>
        public IElement FindGridActionButton(MonochromeIconName iconName, MonochromeIconSize? size = null)
        {
            var iconSize = size ?? MonochromeIconSize.SmallMedium;
            var iconCssClasses = iconName.GetCssClasses(iconSize).ToSpaceSeparated();

            var buttton = page
                .FindAll(".grid-action-button")
                .FirstOrDefault(bt => bt.FirstElementChild?.ClassName?.Contains(iconCssClasses, StringComparison.Ordinal) ?? false);

            return buttton ?? throw new ElementNotFoundException($"Element with css class attribute containing '{iconCssClasses}' not found");
        }
    }

    extension(IRenderedComponent<IComponent> component)
    {
        /// <summary>
        /// Finds a <see cref="SettingsField"/> by its label and returns a rendered child component of a specific type.
        /// </summary>
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
        /// Asserts that the value of a <see cref="SettingsFieldComboBox{TItem, TValue}"/> with a specific label matches the expected value, assuming a standard <see cref="ComboBoxItem{TValue, TDisplay}"/>.
        /// </summary>
        public void AssertSettingsFieldComboBoxWithItem<TValue>(string label, TValue expectedValue)
            => component.GetSettingsFieldComboBox<ComboBoxItem<TValue, string>, TValue>(label).Instance.Value.Should().Be(expectedValue);

        /// <summary>
        /// Asserts that the integer value of a <see cref="SpinEdit{TValue, TInterval, TLimit}"/> with a specific label matches the expected value.
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
        /// Triggers a selection state change on the first data row of a grid.
        /// </summary>
        public void TriggerGridFirstRowSelectionChange(bool select) => component.FindAllGridSelectRows()
            .First()
            .GetGridSelectColumn() // first checkbox in the grid
            .TriggerOnInputEvent(select);

        /// <summary>
        /// Triggers a selection state change for one or more grid rows specified by their index.
        /// </summary>
        /// <exception cref="ElementNotFoundException">Thrown if a specified row index is out of range.</exception>
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
