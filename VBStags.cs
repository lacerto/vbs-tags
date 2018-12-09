using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly string vbsSearchPattern = "*.vbs";
        private string directory = ".";
        private bool recursive = false;
        private string tagfilename = "tags";

        private VBStags() {
        }

        private int StartProcessing(string[] args) {
            bool retval = false;
            ProcessOptions(args);
            retval = CreateTagFile();
            return (retval) ? (0) : (1);
        }

        private void ProcessOptions(string[] args) {
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
                { "r|recursive", "search recursively for vbs files (not supported yet)", v => recursive = (v != null) },
                { "h|help", "show this message and exit", v => show_help = (v != null) },
                { "v|version", "show the version number of this tool", v => show_version = (v != null) }
            };

            List<string> extras = new List<string>();
            try {
                extras = optparser.Parse(args);
            }
            catch (OptionException oe) {
                Console.WriteLine(oe.Message);
                Console.WriteLine("Try running with --help for more information about available options.");
                System.Environment.Exit(2);
            }

            if (show_help) {
                optparser.WriteOptionDescriptions(Console.Out);
                System.Environment.Exit(0);
            }

            if (show_version) {
                Console.WriteLine(version);
                System.Environment.Exit(0);
            }

            switch (extras.Count) {
                case 0:
                    return;
                case 1:
                    directory = extras[0];
                    return;
                default:
                    optparser.WriteOptionDescriptions(Console.Out);
                    break;
            }
            System.Environment.Exit(0);
        }

        private bool CreateTagFile() {
            if (!Directory.Exists(directory)) {
                Console.WriteLine("{0}: directory does not exist.", directory);
                return false;
            }

            List<string> tagLines = new List<string>();
            var vbsFiles = Directory.EnumerateFiles(directory, vbsSearchPattern);
            foreach (string filePath in vbsFiles) {
                tagLines.AddRange(GetTagLines(filePath));
            }

            if (tagLines.Count == 0) return true;

            var tagFilePath = Path.Combine(directory, tagfilename);
            using (StreamWriter tagwriter = new StreamWriter(tagFilePath)) {
                tagwriter.WriteLine("!_TAG_PROGRAM_AUTHOR	lacerto	//");
                tagwriter.WriteLine("!_TAG_PROGRAM_NAME	vbs-tags	//");
                tagwriter.WriteLine("!_TAG_PROGRAM_URL	https://github.com/lacerto/vbs-tags	/github repo/");
                tagwriter.WriteLine("!_TAG_PROGRAM_VERSION	{0}	//", version);
                foreach (var line in tagLines) {
                    tagwriter.WriteLine(line);
                }
            }
            return true;
        }

        private List<string> GetTagLines(string filePath) {
            List<string> tagLines = new List<string>();
            var codeLines = File.ReadLines(filePath);
            Regex rx = new Regex(
                @"^[\t ]*(Function|Sub)[\t ]*(\w*).*$", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
            string fileName = Path.GetFileName(filePath);
            int lineNum = 1;
            foreach (string line in codeLines) {
                var matches = rx.Matches(line);
                foreach (Match match in matches) {
                    tagLines.Add(match.Groups[2].Value + '\t' + fileName + "\t" + lineNum);
                }
                lineNum++;
            }
            tagLines.Sort(StringComparer.InvariantCulture);
            return tagLines;
        }
    }
}
