using System;
using System.Collections.Generic;
using Mono.Options;

namespace vbstags
{
    class VBStags
    {
        static int Main(string[] args)
        {
            var vbstags = new VBStags();
            return vbstags.ProcessOptions(args);
        }

        private VBStags() {
        }

        private int ProcessOptions(string[] args) {
            bool show_help = false;

            var optparser = new OptionSet() {
                "",
                "Usage: dotnet run -- [OPTIONS] directory",
                "",
                "Create tags file for Visual Basic Script files.",
                "If no directory is specified the tags file will be ",
                "generated for the vbs files in the current directory.",
                "",
                "Options:",
                { "h|help", "show this message and exit", v => show_help = (v != null) }
            };

            List<string> extras;
            try {
                extras = optparser.Parse(args);
            }
            catch (OptionException oe) {
                Console.WriteLine(oe.Message);
                Console.WriteLine("Try dotnet run --help for more information about available options.");
                return 1;
            }

            if (show_help) {
                optparser.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (extras.Count > 0) {
                foreach (var item in extras) {
                    Console.WriteLine(item);
                }
                return 0;
            }

            return 0;
        }
    }
}
