namespace pfolder
{
    partial class ProjectToolForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectToolForm));
            this.ProjectNo_Label = new System.Windows.Forms.Label();
            this.ProjectNo_TextBox = new System.Windows.Forms.TextBox();
            this.Button_ProjectFolder = new System.Windows.Forms.Button();
            this.Button_BD = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // ProjectNo_Label
            // 
            this.ProjectNo_Label.AutoSize = true;
            this.ProjectNo_Label.Location = new System.Drawing.Point(13, 13);
            this.ProjectNo_Label.Name = "ProjectNo_Label";
            this.ProjectNo_Label.Size = new System.Drawing.Size(100, 17);
            this.ProjectNo_Label.TabIndex = 0;
            this.ProjectNo_Label.Text = "BC Project No.";
            // 
            // ProjectNo_TextBox
            // 
            this.ProjectNo_TextBox.Location = new System.Drawing.Point(16, 39);
            this.ProjectNo_TextBox.Name = "ProjectNo_TextBox";
            this.ProjectNo_TextBox.Size = new System.Drawing.Size(254, 22);
            this.ProjectNo_TextBox.TabIndex = 1;
            this.ProjectNo_TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ProjectNo_TextBox_KeyUp);
            // 
            // Button_ProjectFolder
            // 
            this.Button_ProjectFolder.Location = new System.Drawing.Point(16, 67);
            this.Button_ProjectFolder.Name = "Button_ProjectFolder";
            this.Button_ProjectFolder.Size = new System.Drawing.Size(117, 30);
            this.Button_ProjectFolder.TabIndex = 2;
            this.Button_ProjectFolder.Text = "Project";
            this.Button_ProjectFolder.UseVisualStyleBackColor = true;
            this.Button_ProjectFolder.Click += new System.EventHandler(this.Button_ProjectFolder_Click);
            // 
            // Button_BD
            // 
            this.Button_BD.Location = new System.Drawing.Point(151, 67);
            this.Button_BD.Name = "Button_BD";
            this.Button_BD.Size = new System.Drawing.Size(119, 30);
            this.Button_BD.TabIndex = 3;
            this.Button_BD.Text = "BD";
            this.Button_BD.UseVisualStyleBackColor = true;
            this.Button_BD.Click += new System.EventHandler(this.Button_BD_Click);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(16, 103);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(254, 10);
            this.ProgressBar.TabIndex = 4;
            // 
            // ProjectToolForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(282, 119);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.Button_BD);
            this.Controls.Add(this.Button_ProjectFolder);
            this.Controls.Add(this.ProjectNo_TextBox);
            this.Controls.Add(this.ProjectNo_Label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(300, 166);
            this.MinimumSize = new System.Drawing.Size(300, 166);
            this.Name = "ProjectToolForm";
            this.Text = "Project Folder Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProjectNo_Label;
        private System.Windows.Forms.TextBox ProjectNo_TextBox;
        private System.Windows.Forms.Button Button_ProjectFolder;
        private System.Windows.Forms.Button Button_BD;
        private System.Windows.Forms.ProgressBar ProgressBar;
    }
}

