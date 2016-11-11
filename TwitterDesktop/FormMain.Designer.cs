namespace TwitterDesktop
{
	partial class FormMain
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
			this.TextBoxRootPath = new System.Windows.Forms.TextBox();
			this.ButtonRootPath = new System.Windows.Forms.Button();
			this.ButtonLearn = new System.Windows.Forms.Button();
			this.FolderBrowserDialogMain = new System.Windows.Forms.FolderBrowserDialog();
			this.BackgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
			this.TextBoxLog = new System.Windows.Forms.TextBox();
			this.TimerMain = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "Root Path:";
			// 
			// TextBoxRootPath
			// 
			this.TextBoxRootPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextBoxRootPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TwitterDesktop.Properties.Settings.Default, "RootPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.TextBoxRootPath.Location = new System.Drawing.Point(75, 12);
			this.TextBoxRootPath.Name = "TextBoxRootPath";
			this.TextBoxRootPath.Size = new System.Drawing.Size(216, 19);
			this.TextBoxRootPath.TabIndex = 1;
			this.TextBoxRootPath.Text = global::TwitterDesktop.Properties.Settings.Default.RootPath;
			// 
			// ButtonRootPath
			// 
			this.ButtonRootPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonRootPath.Location = new System.Drawing.Point(297, 10);
			this.ButtonRootPath.Name = "ButtonRootPath";
			this.ButtonRootPath.Size = new System.Drawing.Size(75, 23);
			this.ButtonRootPath.TabIndex = 2;
			this.ButtonRootPath.Text = "Select";
			this.ButtonRootPath.UseVisualStyleBackColor = true;
			this.ButtonRootPath.Click += new System.EventHandler(this.ButtonRootPath_Click);
			// 
			// ButtonLearn
			// 
			this.ButtonLearn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ButtonLearn.ForeColor = System.Drawing.Color.Black;
			this.ButtonLearn.Location = new System.Drawing.Point(75, 226);
			this.ButtonLearn.Name = "ButtonLearn";
			this.ButtonLearn.Size = new System.Drawing.Size(75, 23);
			this.ButtonLearn.TabIndex = 3;
			this.ButtonLearn.Text = "Learn";
			this.ButtonLearn.UseVisualStyleBackColor = true;
			this.ButtonLearn.Click += new System.EventHandler(this.ButtonLearn_Click);
			// 
			// BackgroundWorkerMain
			// 
			this.BackgroundWorkerMain.WorkerReportsProgress = true;
			this.BackgroundWorkerMain.WorkerSupportsCancellation = true;
			this.BackgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerMain_DoWork);
			this.BackgroundWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerMain_RunWorkerCompleted);
			// 
			// TextBoxLog
			// 
			this.TextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextBoxLog.Location = new System.Drawing.Point(12, 74);
			this.TextBoxLog.Multiline = true;
			this.TextBoxLog.Name = "TextBoxLog";
			this.TextBoxLog.Size = new System.Drawing.Size(360, 146);
			this.TextBoxLog.TabIndex = 4;
			// 
			// TimerMain
			// 
			this.TimerMain.Interval = 5000;
			this.TimerMain.Tick += new System.EventHandler(this.TimerMain_Tick);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 261);
			this.Controls.Add(this.TextBoxLog);
			this.Controls.Add(this.ButtonLearn);
			this.Controls.Add(this.ButtonRootPath);
			this.Controls.Add(this.TextBoxRootPath);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "Learn from twitter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBoxRootPath;
        private System.Windows.Forms.Button ButtonRootPath;
        private System.Windows.Forms.Button ButtonLearn;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogMain;
		private System.ComponentModel.BackgroundWorker BackgroundWorkerMain;
		private System.Windows.Forms.TextBox TextBoxLog;
		private System.Windows.Forms.Timer TimerMain;
	}
}

