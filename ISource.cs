using System.Collections.Generic;

namespace vbstags {
    interface ISource {
        string FileName { get; }
        IEnumerable<string> CodeLines { get; }
    }
}
