﻿namespace CrawlerDesktop
{
	partial class FormMain
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonStart = new System.Windows.Forms.Button();
			this.webBrowserMain = new System.Windows.Forms.WebBrowser();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownLimitRank = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownLimitSize = new System.Windows.Forms.NumericUpDown();
			this.textBoxImageDirectory = new System.Windows.Forms.TextBox();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.progressBarPages = new System.Windows.Forms.ProgressBar();
			this.textBoxPages = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxImages = new System.Windows.Forms.TextBox();
			this.progressBarImages = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitSize)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.Location = new System.Drawing.Point(279, 10);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(75, 23);
			this.buttonStart.TabIndex = 1;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// webBrowserMain
			// 
			this.webBrowserMain.AllowWebBrowserDrop = false;
			this.webBrowserMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserMain.CausesValidation = false;
			this.webBrowserMain.Location = new System.Drawing.Point(12, 289);
			this.webBrowserMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserMain.Name = "webBrowserMain";
			this.webBrowserMain.ScriptErrorsSuppressed = true;
			this.webBrowserMain.Size = new System.Drawing.Size(342, 66);
			this.webBrowserMain.TabIndex = 2;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(12, 170);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(342, 113);
			this.textBoxLog.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "URL:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "Directory:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "Limit Size(KB):";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(166, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "Limit Rank:";
			// 
			// numericUpDownLimitRank
			// 
			this.numericUpDownLimitRank.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LimitRank", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLimitRank.Location = new System.Drawing.Point(234, 13);
			this.numericUpDownLimitRank.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownLimitRank.Name = "numericUpDownLimitRank";
			this.numericUpDownLimitRank.Size = new System.Drawing.Size(60, 19);
			this.numericUpDownLimitRank.TabIndex = 9;
			this.numericUpDownLimitRank.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLimitRank.Value = global::CrawlerDesktop.Properties.Settings.Default.LimitRank;
			// 
			// numericUpDownLimitSize
			// 
			this.numericUpDownLimitSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LimitSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLimitSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownLimitSize.Location = new System.Drawing.Point(92, 13);
			this.numericUpDownLimitSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownLimitSize.Name = "numericUpDownLimitSize";
			this.numericUpDownLimitSize.Size = new System.Drawing.Size(60, 19);
			this.numericUpDownLimitSize.TabIndex = 7;
			this.numericUpDownLimitSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLimitSize.Value = global::CrawlerDesktop.Properties.Settings.Default.LimitSize;
			// 
			// textBoxImageDirectory
			// 
			this.textBoxImageDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxImageDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerImageDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxImageDirectory.Location = new System.Drawing.Point(75, 37);
			this.textBoxImageDirectory.Name = "textBoxImageDirectory";
			this.textBoxImageDirectory.Size = new System.Drawing.Size(198, 19);
			this.textBoxImageDirectory.TabIndex = 5;
			this.textBoxImageDirectory.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerImageDirectory;
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerRootUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxUrl.Location = new System.Drawing.Point(75, 12);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(198, 19);
			this.textBoxUrl.TabIndex = 0;
			this.textBoxUrl.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerRootUrl;
			// 
			// progressBarPages
			// 
			this.progressBarPages.Location = new System.Drawing.Point(55, 41);
			this.progressBarPages.Name = "progressBarPages";
			this.progressBarPages.Size = new System.Drawing.Size(157, 23);
			this.progressBarPages.Step = 1;
			this.progressBarPages.TabIndex = 11;
			// 
			// textBoxPages
			// 
			this.textBoxPages.Location = new System.Drawing.Point(218, 43);
			this.textBoxPages.Name = "textBoxPages";
			this.textBoxPages.ReadOnly = true;
			this.textBoxPages.Size = new System.Drawing.Size(100, 19);
			this.textBoxPages.TabIndex = 12;
			this.textBoxPages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textBoxImages);
			this.groupBox1.Controls.Add(this.progressBarImages);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textBoxPages);
			this.groupBox1.Controls.Add(this.numericUpDownLimitSize);
			this.groupBox1.Controls.Add(this.progressBarPages);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.numericUpDownLimitRank);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 62);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 102);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Crawler";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 12);
			this.label5.TabIndex = 11;
			this.label5.Text = "Pages:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 75);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 12);
			this.label6.TabIndex = 13;
			this.label6.Text = "Images:";
			// 
			// textBoxImages
			// 
			this.textBoxImages.Location = new System.Drawing.Point(218, 72);
			this.textBoxImages.Name = "textBoxImages";
			this.textBoxImages.ReadOnly = true;
			this.textBoxImages.Size = new System.Drawing.Size(100, 19);
			this.textBoxImages.TabIndex = 15;
			this.textBoxImages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// progressBarImages
			// 
			this.progressBarImages.Location = new System.Drawing.Point(55, 70);
			this.progressBarImages.Name = "progressBarImages";
			this.progressBarImages.Size = new System.Drawing.Size(157, 23);
			this.progressBarImages.Step = 1;
			this.progressBarImages.TabIndex = 14;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(366, 367);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxImageDirectory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.webBrowserMain);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.textBoxUrl);
			this.Name = "FormMain";
			this.Text = "Crawler";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitSize)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxUrl;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.WebBrowser webBrowserMain;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxImageDirectory;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownLimitSize;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownLimitRank;
		private System.Windows.Forms.ProgressBar progressBarPages;
		private System.Windows.Forms.TextBox textBoxPages;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxImages;
		private System.Windows.Forms.ProgressBar progressBarImages;
		private System.Windows.Forms.Label label5;
	}
}
