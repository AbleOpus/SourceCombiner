using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using SourceCombiner.Properties;

namespace SourceCombiner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            pathPickerOutput.SelectedPath = Settings.Default.Output;

            if (Settings.Default.Paths == null)
                Settings.Default.Paths = new StringCollection();

            listBox.AddStrings(Settings.Default.Paths);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Output = pathPickerOutput.SelectedPath;
            var SC = new StringCollection();
            SC.AddRange(listBox.GetStrings());
            Settings.Default.Paths = SC;
            Settings.Default.Save();
            base.OnClosing(e);
        }

        private void pathPickerOutput_PickButtonClicked(object sender, EventArgs e)
        {
            using (var dlgOpenFile = new OpenFileDialog())
                if (dlgOpenFile.ShowDialog() == DialogResult.OK) 
                    pathPickerOutput.SelectedPath = dlgOpenFile.FileName;
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (var dlgFolderBrowser = new FolderBrowserDialog())
            {
                dlgFolderBrowser.SelectedPath = Settings.Default.LastFolder;

                if (dlgFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    listBox.Items.Add(dlgFolderBrowser.SelectedPath);
                    Settings.Default.LastFolder = dlgFolderBrowser.SelectedPath;
                }
            }
        }

        private static void CombineSource(string outputPath, params string[] fileNames)
        {
            try
            {
                var SC = new SourceCombiner(outputPath);
                SC.SearchPattern = "*.cs";
                SC.AutoAppend(fileNames);
                SC.Save();
            }
            catch (UnauthorizedAccessException ex)
            {
                ex.ShowError();
            }
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            if (!pathPickerOutput.HasPath)
            {
                MessageBox.Show("The output path needs to be set", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string output = pathPickerOutput.SelectedPath;
            CombineSource(output, listBox.GetStrings());

            var DR = MessageBox.Show("Combining done. Would you like to open the result?", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DR == DialogResult.Yes)
            {
                try
                {
                    Process.Start(output);
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = listBox.SelectedIndex > -1;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            using (var dlgOpenFile = new OpenFileDialog())
            {
                dlgOpenFile.Filter = "All Files|*.*";

                if (dlgOpenFile.ShowDialog() == DialogResult.OK)
                {
                    listBox.Items.Add(dlgOpenFile.FileName);
                }
            }
        }
    }
}
