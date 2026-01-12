using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Sdk.Client.Components.Layout;

public sealed partial class SplitViewComponent : ComponentBase
{
    private bool _isFirstRender = true;
    private int? _lastRegisteredPosition;
    private int _leftSideContentWidth;
    private int _leftSideGhostContentWidth;

    [Parameter]
    public bool IsLive { get; set; } = true;

    [Parameter]
    public RenderFragment? LeftSideContent { get; set; }

    [Parameter]
    public int LeftSideContentWidth { get; set; }

    [Parameter]
    public uint LeftSideMinWidth { get; set; } = 25;

    [Parameter]
    public RenderFragment? RightSideContent { get; set; }

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

    private async void OnMouseUp(MouseEventArgs args)
    {
        _lastRegisteredPosition = null;

        if (!IsLive)
        {
            _leftSideContentWidth = _leftSideGhostContentWidth;
            await InvokeAsync(StateHasChanged);
        }
    }

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
