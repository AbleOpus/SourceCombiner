using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AboControls.Controls.Input
{
    [DefaultEvent("PickButtonClicked")]
    public class PathPicker : Control
    {
        private readonly Button btnClear = new Button();
        private readonly Button btnPick = new Button();
        private readonly Label lblPath = new Label();

        #region Properties
        protected override Size DefaultSize
        {
            get { return new Size(400, 23); }
        }

        [Description("Whether to show the clear button")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool ShowClearButton
        {
            get { return btnClear.Visible; }
            set { btnClear.Visible = value; }
        }

        [Description("Occurs when the value of the SelectedPath property changes")]
        public event EventHandler SelectedPathChanged = delegate {};
        private string _selectedPath;
        [Description("The filename or directory set to display")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (value == _selectedPath) return;
                _selectedPath = value;
                ShowPath();
                SelectedPathChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets whether there is a path selected
        /// </summary>
        [Browsable(false)]
        public bool HasPath
        {
            get { return !String.IsNullOrWhiteSpace(_selectedPath); }
        }

        private string _noPathCaption;
        [Description("The caption to display when there is no caption")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public string NoPathCaption
        {
            get { return _noPathCaption; }
            set
            {
                _noPathCaption = value;
                ShowPath();
            }
        }

        private bool _shortened;
        [Description("Determines whether the path is shortened when its displayed")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool Shortened
        {
            get { return _shortened; }
            set
            {
                _shortened = value;
                ShowPath();
            }
        }
        #endregion

        public PathPicker()
        {
            // lblPath
            lblPath.BorderStyle = BorderStyle.Fixed3D;
            lblPath.Dock = DockStyle.Fill;
            lblPath.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lblPath);
            // btnClear
            btnClear.Dock = DockStyle.Right;
            btnClear.Width = 75;
            btnClear.Text = "Clear";
            btnClear.Visible = false;
            btnClear.Click += delegate { SelectedPath = null; };
            Controls.Add(btnClear);
            // btnPick
            btnPick.Dock = DockStyle.Right;
            btnPick.Width = 75;
            btnPick.Text = "...";
            btnPick.Click += btnPick_Click;
            Controls.Add(btnPick);
        }

        protected virtual void ShowPath()
        {
            if (String.IsNullOrWhiteSpace(SelectedPath))
            {
                lblPath.Text = NoPathCaption;
            }
            else if (_shortened)
            {
                lblPath.Text = Path.GetFileName(_selectedPath);
            }
            else
            {
                lblPath.Text = _selectedPath;
            }
        }

        [Description("Occurs when the pick button is clicked")]
        public event EventHandler PickButtonClicked;
        /// <summary>
        /// Raises the PickButtonClicked event
        /// </summary>
        protected virtual void OnPickButtonClicked()
        {
            if (PickButtonClicked != null)
                PickButtonClicked(this, EventArgs.Empty);
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            OnPickButtonClicked();
        }
    }
}
