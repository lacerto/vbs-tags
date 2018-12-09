using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vbstags {
    class SourceFile : ISource
    {
        private string path = "";
        private string fileName = "";
        private IEnumerable<string> codeLines = Enumerable.Empty<string>();

        public string FileName {
            get { return fileName; }
        }

        public IEnumerable<string> CodeLines {
            get { return codeLines; }
        }

        internal SourceFile(string path) {
            this.path = path; 
            fileName = Path.GetFileName(path);            
        } 

        internal void Load() {
            codeLines = File.ReadLines(path);
        }
    }
}