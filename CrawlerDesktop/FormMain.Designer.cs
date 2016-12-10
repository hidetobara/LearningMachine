namespace CrawlerDesktop
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.buttonStart = new System.Windows.Forms.Button();
			this.webBrowserMain = new System.Windows.Forms.WebBrowser();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.progressBarPages = new System.Windows.Forms.ProgressBar();
			this.textBoxPages = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxImages = new System.Windows.Forms.TextBox();
			this.progressBarImages = new System.Windows.Forms.ProgressBar();
			this.label5 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.chartResult = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.checkBoxHostFixed = new System.Windows.Forms.CheckBox();
			this.numericUpDownLowerSize = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownUpperSize = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownLimitRank = new System.Windows.Forms.NumericUpDown();
			this.textBoxImageDirectory = new System.Windows.Forms.TextBox();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartResult)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.Location = new System.Drawing.Point(267, 10);
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
			this.webBrowserMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserMain.CausesValidation = false;
			this.webBrowserMain.Location = new System.Drawing.Point(348, 12);
			this.webBrowserMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserMain.Name = "webBrowserMain";
			this.webBrowserMain.ScriptErrorsSuppressed = true;
			this.webBrowserMain.Size = new System.Drawing.Size(153, 175);
			this.webBrowserMain.TabIndex = 2;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(12, 193);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(489, 84);
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
			this.label3.Location = new System.Drawing.Point(167, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "Upper Size(KB):";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "Limit Rank:";
			// 
			// progressBarPages
			// 
			this.progressBarPages.Location = new System.Drawing.Point(55, 63);
			this.progressBarPages.Name = "progressBarPages";
			this.progressBarPages.Size = new System.Drawing.Size(157, 19);
			this.progressBarPages.Step = 1;
			this.progressBarPages.TabIndex = 11;
			// 
			// textBoxPages
			// 
			this.textBoxPages.Location = new System.Drawing.Point(218, 63);
			this.textBoxPages.Name = "textBoxPages";
			this.textBoxPages.ReadOnly = true;
			this.textBoxPages.Size = new System.Drawing.Size(100, 19);
			this.textBoxPages.TabIndex = 12;
			this.textBoxPages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBoxHostFixed);
			this.groupBox1.Controls.Add(this.numericUpDownLowerSize);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textBoxImages);
			this.groupBox1.Controls.Add(this.progressBarImages);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textBoxPages);
			this.groupBox1.Controls.Add(this.numericUpDownUpperSize);
			this.groupBox1.Controls.Add(this.progressBarPages);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.numericUpDownLimitRank);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 62);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 125);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Crawler";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 95);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 12);
			this.label6.TabIndex = 13;
			this.label6.Text = "Images:";
			// 
			// textBoxImages
			// 
			this.textBoxImages.Location = new System.Drawing.Point(218, 92);
			this.textBoxImages.Name = "textBoxImages";
			this.textBoxImages.ReadOnly = true;
			this.textBoxImages.Size = new System.Drawing.Size(100, 19);
			this.textBoxImages.TabIndex = 15;
			this.textBoxImages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// progressBarImages
			// 
			this.progressBarImages.Location = new System.Drawing.Point(55, 92);
			this.progressBarImages.Name = "progressBarImages";
			this.progressBarImages.Size = new System.Drawing.Size(157, 19);
			this.progressBarImages.Step = 1;
			this.progressBarImages.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 66);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 12);
			this.label5.TabIndex = 11;
			this.label5.Text = "Pages:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(85, 12);
			this.label7.TabIndex = 17;
			this.label7.Text = "Lower Size(KB):";
			// 
			// chartResult
			// 
			chartArea1.Name = "ChartArea1";
			this.chartResult.ChartAreas.Add(chartArea1);
			legend1.Name = "Legend1";
			this.chartResult.Legends.Add(legend1);
			this.chartResult.Location = new System.Drawing.Point(12, 283);
			this.chartResult.Name = "chartResult";
			series1.ChartArea = "ChartArea1";
			series1.Legend = "Legend1";
			series1.Name = "Series1";
			this.chartResult.Series.Add(series1);
			this.chartResult.Size = new System.Drawing.Size(489, 114);
			this.chartResult.TabIndex = 14;
			this.chartResult.Text = "chart1";
			// 
			// checkBoxHostFixed
			// 
			this.checkBoxHostFixed.AutoSize = true;
			this.checkBoxHostFixed.Checked = global::CrawlerDesktop.Properties.Settings.Default.HostFixed;
			this.checkBoxHostFixed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxHostFixed.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::CrawlerDesktop.Properties.Settings.Default, "HostFixed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkBoxHostFixed.Location = new System.Drawing.Point(169, 14);
			this.checkBoxHostFixed.Name = "checkBoxHostFixed";
			this.checkBoxHostFixed.Size = new System.Drawing.Size(80, 16);
			this.checkBoxHostFixed.TabIndex = 18;
			this.checkBoxHostFixed.Text = "Host Fixed";
			this.checkBoxHostFixed.UseVisualStyleBackColor = true;
			// 
			// numericUpDownLowerSize
			// 
			this.numericUpDownLowerSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LowerSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLowerSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownLowerSize.Location = new System.Drawing.Point(92, 38);
			this.numericUpDownLowerSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownLowerSize.Name = "numericUpDownLowerSize";
			this.numericUpDownLowerSize.Size = new System.Drawing.Size(60, 19);
			this.numericUpDownLowerSize.TabIndex = 16;
			this.numericUpDownLowerSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLowerSize.Value = global::CrawlerDesktop.Properties.Settings.Default.LowerSize;
			// 
			// numericUpDownUpperSize
			// 
			this.numericUpDownUpperSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "UpperSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownUpperSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownUpperSize.Location = new System.Drawing.Point(258, 38);
			this.numericUpDownUpperSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownUpperSize.Name = "numericUpDownUpperSize";
			this.numericUpDownUpperSize.Size = new System.Drawing.Size(60, 19);
			this.numericUpDownUpperSize.TabIndex = 7;
			this.numericUpDownUpperSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownUpperSize.Value = global::CrawlerDesktop.Properties.Settings.Default.UpperSize;
			// 
			// numericUpDownLimitRank
			// 
			this.numericUpDownLimitRank.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LimitRank", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLimitRank.Location = new System.Drawing.Point(92, 13);
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
			// textBoxImageDirectory
			// 
			this.textBoxImageDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxImageDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerImageDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxImageDirectory.Location = new System.Drawing.Point(75, 37);
			this.textBoxImageDirectory.Name = "textBoxImageDirectory";
			this.textBoxImageDirectory.Size = new System.Drawing.Size(186, 19);
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
			this.textBoxUrl.Size = new System.Drawing.Size(186, 19);
			this.textBoxUrl.TabIndex = 0;
			this.textBoxUrl.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerRootUrl;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(513, 409);
			this.Controls.Add(this.chartResult);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxImageDirectory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.webBrowserMain);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.textBoxUrl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.Text = "Crawler";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartResult)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).EndInit();
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
		private System.Windows.Forms.NumericUpDown numericUpDownUpperSize;
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
		private System.Windows.Forms.NumericUpDown numericUpDownLowerSize;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkBoxHostFixed;
		private System.Windows.Forms.DataVisualization.Charting.Chart chartResult;
	}
}

