using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Sdk.Client.Samples.Shared.Pages.Components;

public sealed partial class FileDropPage : ComponentBase
{
    // Multiple switches the component between one file and several files per pick or drop. Without it, a drop of several
    // files never reaches OnChange: the component shows a message in its label and keeps the file picked before.
    private bool _multiple;

    // Counts the OnChange calls, so a drop that the component rejects or ignores is visibly not one of them.
    private int _changeCount;

    private IReadOnlyList<IBrowserFile> _files = [];

    private string ReceivedFiles => _changeCount == 0
        ? "nothing yet"
        : $"call {_changeCount}: {string.Join(", ", _files.Select(file => $"{file.Name} ({file.Size} bytes)"))}";

    private void ShowFiles(InputFileChangeEventArgs args)
    {
        _changeCount++;

        // Without Multiple, OnChange always carries exactly one file, so args.File is safe to read. With Multiple, read the
        // files through GetMultipleFiles: File throws for more than one file, and GetMultipleFiles throws for more files than
        // its limit (10 by default), hence the limit set to the actual count. The files are only references to the browser's
        // files; their content is read through OpenReadStream.
        _files = _multiple ? args.GetMultipleFiles(args.FileCount) : [args.File];
    }
}
