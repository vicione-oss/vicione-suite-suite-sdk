using System.Text;
using Xunit.Abstractions;

namespace Sdk.Testing.Backend;

internal sealed class TestOutputHelperTextWriterAdapter(ITestOutputHelper output) : TextWriter
{
    private readonly AsyncLocal<string> _currentLine = new();
    private readonly ITestOutputHelper _output = output;

    public override Encoding Encoding => Encoding.Unicode;
    public bool Enabled { get; set; } = true;

    public override void Write(char value)
    {
        if (!Enabled)
            return;

        if (value == '\n')
            WriteCurrentLine();
        else
            _currentLine.Value += value;
    }

    private void WriteCurrentLine()
    {
        _output.WriteLine(_currentLine.Value);
        _currentLine.Value = "";
    }

    protected override void Dispose(bool disposing)
    {
        if (!string.IsNullOrEmpty(_currentLine.Value))
            WriteCurrentLine();

        base.Dispose(disposing);
    }
}
