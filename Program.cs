using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SourceCombiner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ProcessArgs(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void ProcessArgs(IEnumerable<string> args)
        {
            string output = null;
            var dirs = new Stack<string>();
            var files = new Stack<string>();
            string searchPattern = "*.*";
            var option = SearchOption.AllDirectories;

            foreach (string arg in args)
            {
                string argument = arg.ToLower().Trim();

                // Get output path
                Match match = Regex.Match(argument, @"/output=(?<Path>[^""]+)");
                {
                    if (match.Success)
                    {
                        output = match.Groups["Path"].Value;
                        continue;
                    }
                }
                // Get input files
                match = Regex.Match(argument, @"/inputfile=(?<Path>[^""]+)");
                if (match.Success)
                {
                    files.Push(match.Groups["Path"].Value);
                    continue;
                }
                // Get input files
                match = Regex.Match(argument, @"/inputdir=(?<Path>[^""]+)");
                if (match.Success)
                {
                    dirs.Push(match.Groups["Path"].Value);
                    continue;
                }
                // Get search option
                if (argument == "/true")
                {
                    option = SearchOption.AllDirectories;
                } 
                else if (argument == "/false")
                {
                    option = SearchOption.TopDirectoryOnly;
                }
                // Get search pattern
                match = Regex.Match(argument, @"/searchpattern=(?<Pat>[^""]+)");
                if (match.Success)
                {
                    searchPattern = match.Groups["Pat"].Value;
                }
            }

            if (output == null)
            {
                ShowErrorMessage("No output path specified in arguments");
                return;
            }

            if (files.Count == 0 && dirs.Count == 0)
            {
                ShowErrorMessage("No paths to content specified");
                return;
            }

            foreach (string dir in dirs.Where(d => !Directory.Exists(d)))
            {
                ShowErrorMessage(@"The directory """ + dir + @""" does not exist");
                return;
            }

            foreach (string file in files.Where(f => !Directory.Exists(f)))
            {
                ShowErrorMessage(@"The file """ + file + @""" does not exist");
                return;
            }

            var combiner = new SourceCombiner(output);
            combiner.SearchOption = option;
            combiner.SearchPattern = searchPattern;
            combiner.AppendFiles(files.ToArray());
            combiner.AppendDirectories(dirs.ToArray());
            combiner.Save();
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Application.ProductName, 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Gets the input directories among command line arguments
        /// </summary>
        /// <returns>An empty array, if no argument found</returns>
        private static string[] GetInputDirectories(IEnumerable<string> args)
        {
            var dirs = new Stack<string>();

            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, @"/inputdir=""(?<Path>[^""]+)""", RegexOptions.IgnoreCase);
                if (match.Success) dirs.Push(match.Groups["Path"].Value);
            }

            return dirs.ToArray();
        }

        /// <summary>
        /// Gets the output directory among command line arguments
        /// </summary>
        /// <returns>Null, if no argument found</returns>
        private static string GetOutputDirectory(IEnumerable<string> args)
        {
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, @"/output=""(?<Path>[^""]+)""", RegexOptions.IgnoreCase);
                if (match.Success) return match.Groups["Path"].Value;
            }

            return null;
        }
    }
}
