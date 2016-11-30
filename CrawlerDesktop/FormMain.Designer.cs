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
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.buttonStart = new System.Windows.Forms.Button();
			this.webBrowserMain = new System.Windows.Forms.WebBrowser();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownLimitSize = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxImageDirectory = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownLimitRank = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUrl.Location = new System.Drawing.Point(75, 12);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(161, 19);
			this.textBoxUrl.TabIndex = 0;
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.Location = new System.Drawing.Point(242, 10);
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
			this.webBrowserMain.Location = new System.Drawing.Point(12, 214);
			this.webBrowserMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserMain.Name = "webBrowserMain";
			this.webBrowserMain.ScriptErrorsSuppressed = true;
			this.webBrowserMain.Size = new System.Drawing.Size(305, 87);
			this.webBrowserMain.TabIndex = 2;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(12, 128);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(305, 80);
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
			// numericUpDownLimitSize
			// 
			this.numericUpDownLimitSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownLimitSize.Location = new System.Drawing.Point(152, 62);
			this.numericUpDownLimitSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownLimitSize.Name = "numericUpDownLimitSize";
			this.numericUpDownLimitSize.Size = new System.Drawing.Size(84, 19);
			this.numericUpDownLimitSize.TabIndex = 7;
			this.numericUpDownLimitSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLimitSize.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "Limit Size(KB):";
			// 
			// textBoxImageDirectory
			// 
			this.textBoxImageDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxImageDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerImageDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxImageDirectory.Location = new System.Drawing.Point(75, 37);
			this.textBoxImageDirectory.Name = "textBoxImageDirectory";
			this.textBoxImageDirectory.Size = new System.Drawing.Size(161, 19);
			this.textBoxImageDirectory.TabIndex = 5;
			this.textBoxImageDirectory.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerImageDirectory;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "Limit Rank:";
			// 
			// numericUpDownLimitRank
			// 
			this.numericUpDownLimitRank.Location = new System.Drawing.Point(152, 87);
			this.numericUpDownLimitRank.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownLimitRank.Name = "numericUpDownLimitRank";
			this.numericUpDownLimitRank.Size = new System.Drawing.Size(84, 19);
			this.numericUpDownLimitRank.TabIndex = 9;
			this.numericUpDownLimitRank.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLimitRank.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(329, 313);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numericUpDownLimitRank);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownLimitSize);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxImageDirectory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.webBrowserMain);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.textBoxUrl);
			this.Name = "FormMain";
			this.Text = "Crawler";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitSize)).EndInit();
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
		private System.Windows.Forms.NumericUpDown numericUpDownLimitSize;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownLimitRank;
	}
}

