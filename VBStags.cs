using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace vbstags
{
    class VBStags
    {
        static int Main(string[] args)
        {
            var vbstags = new VBStags();
            return vbstags.StartProcessing(args);
        }

        private static readonly string version = "0.1";
        private string directory = ".";
        private bool recursive = false;
        private string tagfilename = "tags";

        private VBStags() {
        }

        private int StartProcessing(string[] args) {
            bool retval = false;
            retval &= ProcessOptions(args);
            retval &= CreateTagFile();
            return (retval) ? (0) : (1);
        }

        private bool ProcessOptions(string[] args) {
            bool show_help = false;
            bool show_version = false;

            var optparser = new OptionSet() {
                "",
                "Usage: dotnet run -- [OPTIONS] [directory]",
                "",
                "Create tags file for Visual Basic Script files.",
                "If no directory is specified the tags file will be ",
                "generated for the vbs files in the current directory.",
                "",
                "Options:",
                { "f=", "set the tag file name (most commonly tags or .tags)", v => tagfilename = v },
                { "r|recursive", "search recursively for vbs files", v => recursive = (v != null) },
                { "h|help", "show this message and exit", v => show_help = (v != null) },
                { "v|version", "show the version number of this tool", v => show_version = (v != null) }
            };

            List<string> extras;
            try {
                extras = optparser.Parse(args);
            }
            catch (OptionException oe) {
                Console.WriteLine(oe.Message);
                Console.WriteLine("Try dotnet run --help for more information about available options.");
                return false;
            }

            if (show_help) {
                optparser.WriteOptionDescriptions(Console.Out);
                return true;
            }

            if (show_version) {
                Console.WriteLine(version);
                return true;
            }

            switch (extras.Count) {
                case 0:
                    return true;
                case 1:
                    directory = extras[0];
                    return true;
                default:
                    optparser.WriteOptionDescriptions(Console.Out);
                    return false;
            }
        }

        private bool CreateTagFile() {            
            using (StreamWriter tagwriter = new StreamWriter(Path.Combine(directory, tagfilename))) {
                tagwriter.WriteLine("!_TAG_PROGRAM_AUTHOR	lacerto	//");
                tagwriter.WriteLine("!_TAG_PROGRAM_NAME	vbs-tags	//");
                tagwriter.WriteLine("!_TAG_PROGRAM_URL	https://github.com/lacerto/vbs-tags	/github repo/");
                tagwriter.WriteLine("!_TAG_PROGRAM_VERSION	{0}	//", version);
            }
            return true;
        }
    }
}
