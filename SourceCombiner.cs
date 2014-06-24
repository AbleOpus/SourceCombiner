using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SourceCombiner
{
    /// <summary>
    /// Represents a mechanism to combine source code into one file
    /// </summary>
    class SourceCombiner
    {
        private readonly string _outputPath;

        /// <summary>
        /// Gets the result of the appendages as a StringBuilder
        /// </summary>
        public StringBuilder Content { get; protected set; }

        /// <summary>
        /// Gets or sets the search pattern to use for directory iteration
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        /// Gets or sets whether to search all sub-directories
        /// </summary>
        public SearchOption SearchOption { get; set; }

        /// <summary>
        /// Occurs before the contents is saved to file
        /// </summary>
        public event EventHandler Saving = delegate { };
        /// <summary>
        /// Raises the Saving event
        /// </summary>
        protected virtual void OnSaving()
        {
            Saving(this, EventArgs.Empty);
        }


        public SourceCombiner(string outputPath)
        {
            Content = new StringBuilder();
            SearchPattern = "*.*"; // Files must have an extension by default
            SearchOption = SearchOption.AllDirectories;
            _outputPath = outputPath;
        }

        /// <summary>
        /// Append the contents of the specified files to the result
        /// </summary>
        public void AppendFiles(params string[] fileNames)
        {
            foreach (string name in fileNames)
            {
                string content = File.ReadAllText(name);
                Content.Append(content);
                Content.AppendLine();
            }
        }

        /// <summary>
        /// Append the contents of the specified directory to the result.
        /// Using the directory search options set for this instance
        /// </summary>
        public void AppendDirectories(params string[] directories)
        {
            foreach (string dir in directories)
            {
                var files = Directory.GetFiles(dir, SearchPattern, SearchOption);
                AppendFiles(files);
            }
        }

        /// <summary>
        /// Automatically determines whether an appendage is a directory or a file path.
        /// If it is a directory, the set search option and pattern  will be used to load from it
        /// </summary>
        public void AutoAppend(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    AppendDirectories(path);
                }
                else
                {
                    AppendFiles(path);
                }
            }
        }

        /// <summary>
        /// Saves the appended content to file
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void Save()
        {
            OnSaving();
            string dir = Path.GetDirectoryName(_outputPath);

            if (!String.IsNullOrEmpty(dir) &&
                !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_outputPath, Content.ToString());
        }
    }
}
