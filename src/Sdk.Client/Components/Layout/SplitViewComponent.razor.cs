using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Sdk.Client.Components.Layout;

/// <summary>
/// A layout component that creates two resizable panes separated by a draggable splitter.
/// </summary>
public sealed partial class SplitViewComponent : ComponentBase
{
    private bool _isFirstRender = true;
    private int? _lastRegisteredPosition;
    private int _leftSideContentWidth;
    private int _leftSideGhostContentWidth;

    /// <summary>
    /// Gets or sets a value indicating whether the resizing is applied live while dragging.
    /// If false, the resize is only applied after the mouse button is released.
    /// </summary>
    [Parameter]
    public bool IsLive { get; set; } = true;

    /// <summary>
    /// Gets or sets the content to be displayed in the left pane.
    /// </summary>
    [Parameter]
    public RenderFragment? LeftSideContent { get; set; }

    /// <summary>
    /// Gets or sets the initial width of the left pane in pixels.
    /// </summary>
    [Parameter]
    public int LeftSideContentWidth { get; set; }

    /// <summary>
    /// Gets or sets the minimum width of the left pane in pixels.
    /// </summary>
    [Parameter]
    public uint LeftSideMinWidth { get; set; } = 25;

    /// <summary>
    /// Gets or sets the content to be displayed in the right pane.
    /// </summary>
    [Parameter]
    public RenderFragment? RightSideContent { get; set; }

    /// <summary>
    /// Gets or sets the width of the draggable splitter bar in pixels.
    /// </summary>
    [Parameter]
    public int SplitterWidth { get; set; } = 10;

    private void OnMouseDown(MouseEventArgs args)
    {
        _lastRegisteredPosition = Convert.ToInt32(args.ClientX);

        if (!IsLive)
            _leftSideGhostContentWidth = _leftSideContentWidth;
    }

    private async void OnMouseMove(MouseEventArgs args)
    {
        if (args.Buttons == 0)
            OnMouseUp(args);

        if (_lastRegisteredPosition.HasValue)
        {
            var mouseX = Convert.ToInt32(args.ClientX);

            if (IsLive)
            {
                var newLeftSideWidth = _leftSideContentWidth - (_lastRegisteredPosition.Value - mouseX);
                _leftSideContentWidth = newLeftSideWidth < LeftSideMinWidth ? (int)LeftSideMinWidth : newLeftSideWidth;

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                var newLeftSideWidth = _leftSideGhostContentWidth - (_lastRegisteredPosition.Value - mouseX);
                _leftSideGhostContentWidth = newLeftSideWidth < LeftSideMinWidth ? (int)LeftSideMinWidth : newLeftSideWidth;
            }

            var saveLastPos = (mouseX >= 0) &&
                              (_leftSideContentWidth >= LeftSideMinWidth) &&
                              (_leftSideGhostContentWidth >= LeftSideMinWidth);

            if (saveLastPos)
                _lastRegisteredPosition = mouseX;
        }
    }

    private async void OnMouseUp(MouseEventArgs _)
    {
        _lastRegisteredPosition = null;

        if (!IsLive)
        {
            _leftSideContentWidth = _leftSideGhostContentWidth;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (_isFirstRender)
        {
            _leftSideContentWidth = LeftSideContentWidth;
            _leftSideGhostContentWidth = LeftSideContentWidth;
            _isFirstRender = false;
        }
    }
}
