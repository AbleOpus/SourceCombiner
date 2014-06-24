namespace SourceCombiner
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCombine = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.pathPickerOutput = new AboControls.Controls.Input.PathPicker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Output";
            // 
            // btnCombine
            // 
            this.btnCombine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCombine.Location = new System.Drawing.Point(712, 422);
            this.btnCombine.Name = "btnCombine";
            this.btnCombine.Size = new System.Drawing.Size(196, 36);
            this.btnCombine.TabIndex = 3;
            this.btnCombine.Text = "Combine";
            this.btnCombine.UseVisualStyleBackColor = true;
            this.btnCombine.Click += new System.EventHandler(this.btnCombine_Click);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.IntegralHeight = false;
            this.listBox.ItemHeight = 20;
            this.listBox.Location = new System.Drawing.Point(12, 84);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(896, 332);
            this.listBox.TabIndex = 6;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Source Paths:";
            // 
            // btnAddFile
            // 
            this.btnAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFile.BackgroundImage = global::SourceCombiner.Properties.Resources._077_AddFile_24x24_72;
            this.btnAddFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddFile.Location = new System.Drawing.Point(469, 422);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(75, 36);
            this.btnAddFile.TabIndex = 0;
            this.toolTip.SetToolTip(this.btnAddFile, "Add a file to append");
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.BackgroundImage = global::SourceCombiner.Properties.Resources._112_Minus_Orange_24x24_72;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(631, 422);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 36);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFolder.BackgroundImage = global::SourceCombiner.Properties.Resources._042b_AddCategory_24x24_72;
            this.btnAddFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddFolder.Location = new System.Drawing.Point(550, 422);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(75, 36);
            this.btnAddFolder.TabIndex = 0;
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // pathPickerOutput
            // 
            this.pathPickerOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathPickerOutput.Location = new System.Drawing.Point(89, 12);
            this.pathPickerOutput.Name = "pathPickerOutput";
            this.pathPickerOutput.Size = new System.Drawing.Size(819, 34);
            this.pathPickerOutput.TabIndex = 0;
            this.pathPickerOutput.Text = "pathPicker1";
            this.pathPickerOutput.PickButtonClicked += new System.EventHandler(this.pathPickerOutput_PickButtonClicked);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnCombine;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 470);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnCombine);
            this.Controls.Add(this.pathPickerOutput);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(480, 275);
            this.Name = "MainForm";
            this.Text = "Source Combiner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private AboControls.Controls.Input.PathPicker pathPickerOutput;
        private System.Windows.Forms.Button btnCombine;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

