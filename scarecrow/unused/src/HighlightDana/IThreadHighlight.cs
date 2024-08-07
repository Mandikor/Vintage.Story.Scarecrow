using System.Threading;

namespace scarecrow.unused.src.HighlightDana;

public interface IThreadHighlight
{
    bool Enabled { get; set; }
    Thread OpThread { get; set; }
    string ThreadName { get; }
}