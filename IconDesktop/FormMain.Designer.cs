﻿namespace IconDesktop
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
			this.label1 = new System.Windows.Forms.Label();
			this.ButtonNeuroDirectory = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ButtonInputDirectory = new System.Windows.Forms.Button();
			this.TextBoxInputDirectory = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ButtonRunTraining = new System.Windows.Forms.Button();
			this.TextBoxLog = new System.Windows.Forms.TextBox();
			this.FolderBrowserDialogMain = new System.Windows.Forms.FolderBrowserDialog();
			this.BackgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
			this.TextBoxNeuroDirectory = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ButtonRunForecast = new System.Windows.Forms.Button();
			this.ButtonForecastPath = new System.Windows.Forms.Button();
			this.TextBoxForecast = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.OpenFileDialogMain = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "Training Directory:";
			// 
			// ButtonNeuroDirectory
			// 
			this.ButtonNeuroDirectory.Location = new System.Drawing.Point(297, 12);
			this.ButtonNeuroDirectory.Name = "ButtonNeuroDirectory";
			this.ButtonNeuroDirectory.Size = new System.Drawing.Size(75, 23);
			this.ButtonNeuroDirectory.TabIndex = 2;
			this.ButtonNeuroDirectory.Text = "Select";
			this.ButtonNeuroDirectory.UseVisualStyleBackColor = true;
			this.ButtonNeuroDirectory.Click += new System.EventHandler(this.ButtonDirectory_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ButtonInputDirectory);
			this.groupBox1.Controls.Add(this.TextBoxInputDirectory);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.ButtonRunTraining);
			this.groupBox1.Location = new System.Drawing.Point(12, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(360, 80);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Training";
			// 
			// ButtonInputDirectory
			// 
			this.ButtonInputDirectory.Location = new System.Drawing.Point(279, 18);
			this.ButtonInputDirectory.Name = "ButtonInputDirectory";
			this.ButtonInputDirectory.Size = new System.Drawing.Size(75, 23);
			this.ButtonInputDirectory.TabIndex = 5;
			this.ButtonInputDirectory.Text = "Select";
			this.ButtonInputDirectory.UseVisualStyleBackColor = true;
			this.ButtonInputDirectory.Click += new System.EventHandler(this.ButtonDirectory_Click);
			// 
			// TextBoxInputDirectory
			// 
			this.TextBoxInputDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::IconDesktop.Properties.Settings.Default, "InputDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.TextBoxInputDirectory.Location = new System.Drawing.Point(114, 20);
			this.TextBoxInputDirectory.Name = "TextBoxInputDirectory";
			this.TextBoxInputDirectory.Size = new System.Drawing.Size(159, 19);
			this.TextBoxInputDirectory.TabIndex = 4;
			this.TextBoxInputDirectory.Text = global::IconDesktop.Properties.Settings.Default.InputDirectory;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "Input Directory:";
			// 
			// ButtonRunTraining
			// 
			this.ButtonRunTraining.Location = new System.Drawing.Point(279, 47);
			this.ButtonRunTraining.Name = "ButtonRunTraining";
			this.ButtonRunTraining.Size = new System.Drawing.Size(75, 23);
			this.ButtonRunTraining.TabIndex = 0;
			this.ButtonRunTraining.Text = "Run";
			this.ButtonRunTraining.UseVisualStyleBackColor = true;
			this.ButtonRunTraining.Click += new System.EventHandler(this.ButtonRun_Click);
			// 
			// TextBoxLog
			// 
			this.TextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextBoxLog.Location = new System.Drawing.Point(12, 237);
			this.TextBoxLog.Multiline = true;
			this.TextBoxLog.Name = "TextBoxLog";
			this.TextBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TextBoxLog.Size = new System.Drawing.Size(360, 113);
			this.TextBoxLog.TabIndex = 4;
			// 
			// BackgroundWorkerMain
			// 
			this.BackgroundWorkerMain.WorkerReportsProgress = true;
			this.BackgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerMain_DoWork);
			this.BackgroundWorkerMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerMain_ProgressChanged);
			this.BackgroundWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerMain_RunWorkerCompleted);
			// 
			// TextBoxNeuroDirectory
			// 
			this.TextBoxNeuroDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::IconDesktop.Properties.Settings.Default, "TrainingDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.TextBoxNeuroDirectory.Location = new System.Drawing.Point(131, 14);
			this.TextBoxNeuroDirectory.Name = "TextBoxNeuroDirectory";
			this.TextBoxNeuroDirectory.Size = new System.Drawing.Size(160, 19);
			this.TextBoxNeuroDirectory.TabIndex = 1;
			this.TextBoxNeuroDirectory.Text = global::IconDesktop.Properties.Settings.Default.TrainingDirectory;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ButtonRunForecast);
			this.groupBox2.Controls.Add(this.ButtonForecastPath);
			this.groupBox2.Controls.Add(this.TextBoxForecast);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(12, 127);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(360, 80);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Forecast";
			// 
			// ButtonRunForecast
			// 
			this.ButtonRunForecast.Location = new System.Drawing.Point(279, 47);
			this.ButtonRunForecast.Name = "ButtonRunForecast";
			this.ButtonRunForecast.Size = new System.Drawing.Size(75, 23);
			this.ButtonRunForecast.TabIndex = 9;
			this.ButtonRunForecast.Text = "Run";
			this.ButtonRunForecast.UseVisualStyleBackColor = true;
			this.ButtonRunForecast.Click += new System.EventHandler(this.ButtonRun_Click);
			// 
			// ButtonForecastPath
			// 
			this.ButtonForecastPath.Location = new System.Drawing.Point(279, 18);
			this.ButtonForecastPath.Name = "ButtonForecastPath";
			this.ButtonForecastPath.Size = new System.Drawing.Size(75, 23);
			this.ButtonForecastPath.TabIndex = 8;
			this.ButtonForecastPath.Text = "Select";
			this.ButtonForecastPath.UseVisualStyleBackColor = true;
			this.ButtonForecastPath.Click += new System.EventHandler(this.ButtonPath_Click);
			// 
			// TextBoxForecast
			// 
			this.TextBoxForecast.Location = new System.Drawing.Point(114, 20);
			this.TextBoxForecast.Name = "TextBoxForecast";
			this.TextBoxForecast.Size = new System.Drawing.Size(159, 19);
			this.TextBoxForecast.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "Input Path:";
			// 
			// OpenFileDialogMain
			// 
			this.OpenFileDialogMain.Filter = "*.png|*.png|*.*|*.*";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 362);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.TextBoxLog);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ButtonNeuroDirectory);
			this.Controls.Add(this.TextBoxNeuroDirectory);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "Deep Learning";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TextBoxNeuroDirectory;
		private System.Windows.Forms.Button ButtonNeuroDirectory;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button ButtonRunTraining;
		private System.Windows.Forms.TextBox TextBoxLog;
		private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogMain;
		private System.ComponentModel.BackgroundWorker BackgroundWorkerMain;
		private System.Windows.Forms.Button ButtonInputDirectory;
		private System.Windows.Forms.TextBox TextBoxInputDirectory;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button ButtonRunForecast;
		private System.Windows.Forms.Button ButtonForecastPath;
		private System.Windows.Forms.TextBox TextBoxForecast;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.OpenFileDialog OpenFileDialogMain;
	}
}

