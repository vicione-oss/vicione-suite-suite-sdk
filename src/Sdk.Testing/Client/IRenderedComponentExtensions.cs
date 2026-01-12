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

public static class IRenderedComponentExtensions
{
    public static IElement FindIconButton<T>(this IRenderedComponent<T> page, string iconCss) where T : IComponent
        => page.FindAll("button")
            .Single(k => k.InnerHtml.Contains(iconCss, StringComparison.Ordinal));

    public static IElement FindGridActionButton<TComponent>(this IRenderedComponent<TComponent> component, MonochromeIconName iconName, MonochromeIconSize? size = null)
        where TComponent : IComponent
    {
        var iconSize = size ?? MonochromeIconSize.SmallMedium;
        var iconCssClasses = iconName.GetCssClasses(iconSize).ToSpaceSeparated();

        var buttton = component
            .FindAll(".grid-action-button")
            .FirstOrDefault(bt => bt.FirstElementChild?.ClassName?.Contains(iconCssClasses, StringComparison.Ordinal) ?? false);

        if (buttton is null)
            throw new ElementNotFoundException($"Element with css class attribute containing '{iconCssClasses}' not found");

        return buttton;
    }

    public static IRenderedComponent<TChild> GetSettingsFieldChild<TComponent, TChild>(this IRenderedComponent<TComponent> component, string label)
        where TComponent : IComponent
        where TChild : IComponent
    {
        var field = component
            .FindComponents<SettingsField>()
            .First(k => Equals(k.Instance.Label, label));

        return field.FindComponent<TChild>();
    }

    public static IRenderedComponent<SettingsFieldTextBox> GetSettingsFieldTextBox<TComponent>(this IRenderedComponent<TComponent> component, string label)
        where TComponent : IComponent
        => component.GetSettingsFieldChild<TComponent, SettingsFieldTextBox>(label);

    public static IRenderedComponent<CheckBox<bool>> GetSettingsFieldCheckBox<TComponent>(this IRenderedComponent<TComponent> component, string label)
        where TComponent : IComponent
        => component.GetSettingsFieldChild<TComponent, CheckBox<bool>>(label);

    public static IRenderedComponent<SettingsFieldComboBox<TItem, TValue>> GetSettingsFieldComboBox<TComponent, TItem, TValue>(this IRenderedComponent<TComponent> component, string label)
        where TComponent : IComponent
        => component.GetSettingsFieldChild<TComponent, SettingsFieldComboBox<TItem, TValue>>(label);

    public static IRenderedComponent<SpinEdit<TValue, TInterval, TLimit>> GetSettingsFieldSpinEdit<TComponent, TValue, TInterval, TLimit>(this IRenderedComponent<TComponent> component, string label)
        where TComponent : IComponent
        => component.GetSettingsFieldChild<TComponent, SpinEdit<TValue, TInterval, TLimit>>(label);

    public static void AssertSettingsFieldTextBox<TComponent>(this IRenderedComponent<TComponent> component, string label, string? expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldTextBox(label).Instance.Value.Should().Be(expectedValue);

    public static void AssertSettingsFieldCheckBox<TComponent>(this IRenderedComponent<TComponent> component, string label, bool expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldCheckBox(label).Instance.Value.Should().Be(expectedValue);

    public static void AssertSettingsFieldComboBox<TComponent, TItem, TValue>(this IRenderedComponent<TComponent> component, string label, TValue expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldComboBox<TComponent, TItem, TValue>(label).Instance.Value.Should().Be(expectedValue);

    public static void AssertSettingsFieldComboBoxWithItem<TComponent, TValue>(this IRenderedComponent<TComponent> component, string label, TValue expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldComboBox<TComponent, ComboBoxItem<TValue, string>, TValue>(label).Instance.Value.Should().Be(expectedValue);

    public static void AssertSettingsFieldSpinEditInt<TComponent>(this IRenderedComponent<TComponent> component, string label, int expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldSpinEdit<TComponent, int, int, int>(label).Instance.Value.Should().Be(expectedValue);

    public static void AssertSettingsFieldSpinEditInt<TComponent, TValue, TInterval, TLimit>(this IRenderedComponent<TComponent> component, string label, TValue expectedValue)
        where TComponent : IComponent
        => component.GetSettingsFieldSpinEdit<TComponent, TValue, TInterval, TLimit>(label).Instance.Value.Should().Be(expectedValue);

    /// <summary>
    /// Triggers row selection state change to <see cref="select"/>
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="component"></param>
    /// <param name="select"></param>
    public static void TriggerGridFirstRowSelectionChange<TComponent>(this IRenderedComponent<TComponent> component, bool select)
        where TComponent : IComponent
    {
        // first checkbox in the grid
        component
            .FindAllGridSelectRows()
            .First()
            .GetGridSelectColumn()
            .TriggerOnInputEvent(select);
    }

    /// <summary>
    /// Triggers row selection state change to <see cref="select"/> for given <see cref="rowIndexes"/>    
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="component"></param>
    /// <param name="select">true will select the row</param>
    /// <param name="rowIndexes">0 for header row, 1 for first row, ..</param>
    /// <exception cref="ElementNotFoundException"></exception>
    public static void TriggerGridRowSelectionChange<TComponent>(this IRenderedComponent<TComponent> component, bool select, params IEnumerable<int> rowIndexes) where TComponent : IComponent
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

    private static IEnumerable<IElement> FindAllGridSelectRows<TComponent>(this IRenderedComponent<TComponent> component, bool includeHeaderRow = false) where TComponent : IComponent
    {
        const string ItemSelectCss = ".item-select-column";

        var rows = component.FindAll(ItemSelectCss);
        if (!rows.Any())
            throw new ElementNotFoundException($"No Grid rows with class '{ItemSelectCss}' found.");

        if (includeHeaderRow)
            return rows;

        return rows.Skip(1);
    }

    private static IElement GetGridSelectColumn(this IElement element)
        => element.FindDescendant<IHtmlInputElement>() ?? throw new ElementNotFoundException("Grid select checkbox not found.");

    private static void TriggerOnInputEvent(this IElement element, bool value)
        => element.TriggerEvent("oninput", new ChangeEventArgs
        {
            Value = value
        });
}
