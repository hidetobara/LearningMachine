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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.buttonStart = new System.Windows.Forms.Button();
			this.webBrowserMain = new System.Windows.Forms.WebBrowser();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.progressBarPages = new System.Windows.Forms.ProgressBar();
			this.textBoxPages = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxImages = new System.Windows.Forms.TextBox();
			this.progressBarImages = new System.Windows.Forms.ProgressBar();
			this.label5 = new System.Windows.Forms.Label();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.panelOut = new System.Windows.Forms.Panel();
			this.tabControlNode = new System.Windows.Forms.TabControl();
			this.tabPageMain = new System.Windows.Forms.TabPage();
			this.textBoxMainWhite = new System.Windows.Forms.TextBox();
			this.labelMainWhite = new System.Windows.Forms.Label();
			this.textBoxImageDirectory = new System.Windows.Forms.TextBox();
			this.numericUpDownLimitRank = new System.Windows.Forms.NumericUpDown();
			this.tabPageImage = new System.Windows.Forms.TabPage();
			this.numericUpDownLowerSize = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownUpperSize = new System.Windows.Forms.NumericUpDown();
			this.tabPageXml = new System.Windows.Forms.TabPage();
			this.textBoxXmlBear = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonJump = new System.Windows.Forms.Button();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.panelOut.SuspendLayout();
			this.tabControlNode.SuspendLayout();
			this.tabPageMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).BeginInit();
			this.tabPageImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperSize)).BeginInit();
			this.tabPageXml.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonStart
			// 
			this.buttonStart.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonStart.Location = new System.Drawing.Point(11, 263);
			this.buttonStart.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(133, 34);
			this.buttonStart.TabIndex = 1;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// webBrowserMain
			// 
			this.webBrowserMain.AllowWebBrowserDrop = false;
			this.webBrowserMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserMain.CausesValidation = false;
			this.webBrowserMain.Location = new System.Drawing.Point(5, 48);
			this.webBrowserMain.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.webBrowserMain.MinimumSize = new System.Drawing.Size(33, 30);
			this.webBrowserMain.Name = "webBrowserMain";
			this.webBrowserMain.ScriptErrorsSuppressed = true;
			this.webBrowserMain.Size = new System.Drawing.Size(411, 552);
			this.webBrowserMain.TabIndex = 2;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(5, 76);
			this.textBoxLog.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(602, 215);
			this.textBoxLog.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 10);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 18);
			this.label2.TabIndex = 6;
			this.label2.Text = "Directory:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(275, 10);
			this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(126, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Upper Size(KB):";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 137);
			this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(91, 18);
			this.label4.TabIndex = 10;
			this.label4.Text = "Limit Rank:";
			// 
			// progressBarPages
			// 
			this.progressBarPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarPages.Location = new System.Drawing.Point(86, 4);
			this.progressBarPages.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.progressBarPages.Name = "progressBarPages";
			this.progressBarPages.Size = new System.Drawing.Size(347, 28);
			this.progressBarPages.Step = 1;
			this.progressBarPages.TabIndex = 11;
			// 
			// textBoxPages
			// 
			this.textBoxPages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPages.Location = new System.Drawing.Point(443, 6);
			this.textBoxPages.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.textBoxPages.Name = "textBoxPages";
			this.textBoxPages.ReadOnly = true;
			this.textBoxPages.Size = new System.Drawing.Size(164, 25);
			this.textBoxPages.TabIndex = 12;
			this.textBoxPages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 10);
			this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(127, 18);
			this.label7.TabIndex = 17;
			this.label7.Text = "Lower Size(KB):";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 44);
			this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(63, 18);
			this.label6.TabIndex = 13;
			this.label6.Text = "Images:";
			// 
			// textBoxImages
			// 
			this.textBoxImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxImages.Location = new System.Drawing.Point(443, 41);
			this.textBoxImages.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.textBoxImages.Name = "textBoxImages";
			this.textBoxImages.ReadOnly = true;
			this.textBoxImages.Size = new System.Drawing.Size(164, 25);
			this.textBoxImages.TabIndex = 15;
			this.textBoxImages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// progressBarImages
			// 
			this.progressBarImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarImages.Location = new System.Drawing.Point(86, 40);
			this.progressBarImages.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.progressBarImages.Name = "progressBarImages";
			this.progressBarImages.Size = new System.Drawing.Size(347, 28);
			this.progressBarImages.Step = 1;
			this.progressBarImages.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 9);
			this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 18);
			this.label5.TabIndex = 11;
			this.label5.Text = "Pages:";
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMain.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.splitContainerMain.Name = "splitContainerMain";
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.panelOut);
			this.splitContainerMain.Panel1.Controls.Add(this.tabControlNode);
			this.splitContainerMain.Panel1.Controls.Add(this.buttonStart);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.Controls.Add(this.buttonJump);
			this.splitContainerMain.Panel2.Controls.Add(this.textBoxUrl);
			this.splitContainerMain.Panel2.Controls.Add(this.webBrowserMain);
			this.splitContainerMain.Size = new System.Drawing.Size(1132, 614);
			this.splitContainerMain.SplitterDistance = 634;
			this.splitContainerMain.SplitterWidth = 7;
			this.splitContainerMain.TabIndex = 15;
			// 
			// panelOut
			// 
			this.panelOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelOut.Controls.Add(this.label5);
			this.panelOut.Controls.Add(this.progressBarPages);
			this.panelOut.Controls.Add(this.textBoxLog);
			this.panelOut.Controls.Add(this.textBoxPages);
			this.panelOut.Controls.Add(this.label6);
			this.panelOut.Controls.Add(this.progressBarImages);
			this.panelOut.Controls.Add(this.textBoxImages);
			this.panelOut.Location = new System.Drawing.Point(11, 304);
			this.panelOut.Name = "panelOut";
			this.panelOut.Size = new System.Drawing.Size(614, 297);
			this.panelOut.TabIndex = 15;
			// 
			// tabControlNode
			// 
			this.tabControlNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlNode.Controls.Add(this.tabPageMain);
			this.tabControlNode.Controls.Add(this.tabPageImage);
			this.tabControlNode.Controls.Add(this.tabPageXml);
			this.tabControlNode.Location = new System.Drawing.Point(11, 11);
			this.tabControlNode.Name = "tabControlNode";
			this.tabControlNode.SelectedIndex = 0;
			this.tabControlNode.Size = new System.Drawing.Size(614, 212);
			this.tabControlNode.TabIndex = 14;
			// 
			// tabPageMain
			// 
			this.tabPageMain.Controls.Add(this.textBoxMainWhite);
			this.tabPageMain.Controls.Add(this.labelMainWhite);
			this.tabPageMain.Controls.Add(this.label2);
			this.tabPageMain.Controls.Add(this.textBoxImageDirectory);
			this.tabPageMain.Controls.Add(this.numericUpDownLimitRank);
			this.tabPageMain.Controls.Add(this.label4);
			this.tabPageMain.Location = new System.Drawing.Point(4, 28);
			this.tabPageMain.Name = "tabPageMain";
			this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMain.Size = new System.Drawing.Size(606, 180);
			this.tabPageMain.TabIndex = 0;
			this.tabPageMain.Text = "Main";
			this.tabPageMain.UseVisualStyleBackColor = true;
			// 
			// textBoxMainWhite
			// 
			this.textBoxMainWhite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMainWhite.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "MainWhiteReg", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxMainWhite.Location = new System.Drawing.Point(106, 39);
			this.textBoxMainWhite.Multiline = true;
			this.textBoxMainWhite.Name = "textBoxMainWhite";
			this.textBoxMainWhite.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxMainWhite.Size = new System.Drawing.Size(492, 89);
			this.textBoxMainWhite.TabIndex = 12;
			this.textBoxMainWhite.Text = global::CrawlerDesktop.Properties.Settings.Default.MainWhiteReg;
			// 
			// labelMainWhite
			// 
			this.labelMainWhite.AutoSize = true;
			this.labelMainWhite.Location = new System.Drawing.Point(8, 42);
			this.labelMainWhite.Name = "labelMainWhite";
			this.labelMainWhite.Size = new System.Drawing.Size(53, 18);
			this.labelMainWhite.TabIndex = 11;
			this.labelMainWhite.Text = "White:";
			// 
			// textBoxImageDirectory
			// 
			this.textBoxImageDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxImageDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerImageDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxImageDirectory.Location = new System.Drawing.Point(106, 7);
			this.textBoxImageDirectory.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.textBoxImageDirectory.Name = "textBoxImageDirectory";
			this.textBoxImageDirectory.Size = new System.Drawing.Size(492, 25);
			this.textBoxImageDirectory.TabIndex = 5;
			this.textBoxImageDirectory.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerImageDirectory;
			// 
			// numericUpDownLimitRank
			// 
			this.numericUpDownLimitRank.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LimitRank", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLimitRank.Location = new System.Drawing.Point(106, 135);
			this.numericUpDownLimitRank.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.numericUpDownLimitRank.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownLimitRank.Name = "numericUpDownLimitRank";
			this.numericUpDownLimitRank.Size = new System.Drawing.Size(100, 25);
			this.numericUpDownLimitRank.TabIndex = 9;
			this.numericUpDownLimitRank.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownLimitRank.Value = global::CrawlerDesktop.Properties.Settings.Default.LimitRank;
			// 
			// tabPageImage
			// 
			this.tabPageImage.Controls.Add(this.label7);
			this.tabPageImage.Controls.Add(this.label3);
			this.tabPageImage.Controls.Add(this.numericUpDownLowerSize);
			this.tabPageImage.Controls.Add(this.numericUpDownUpperSize);
			this.tabPageImage.Location = new System.Drawing.Point(4, 28);
			this.tabPageImage.Name = "tabPageImage";
			this.tabPageImage.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageImage.Size = new System.Drawing.Size(606, 180);
			this.tabPageImage.TabIndex = 1;
			this.tabPageImage.Text = "Image";
			this.tabPageImage.UseVisualStyleBackColor = true;
			// 
			// numericUpDownLowerSize
			// 
			this.numericUpDownLowerSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::CrawlerDesktop.Properties.Settings.Default, "LowerSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.numericUpDownLowerSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownLowerSize.Location = new System.Drawing.Point(150, 7);
			this.numericUpDownLowerSize.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.numericUpDownLowerSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownLowerSize.Name = "numericUpDownLowerSize";
			this.numericUpDownLowerSize.Size = new System.Drawing.Size(100, 25);
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
			this.numericUpDownUpperSize.Location = new System.Drawing.Point(427, 7);
			this.numericUpDownUpperSize.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.numericUpDownUpperSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericUpDownUpperSize.Name = "numericUpDownUpperSize";
			this.numericUpDownUpperSize.Size = new System.Drawing.Size(100, 25);
			this.numericUpDownUpperSize.TabIndex = 7;
			this.numericUpDownUpperSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownUpperSize.Value = global::CrawlerDesktop.Properties.Settings.Default.UpperSize;
			// 
			// tabPageXml
			// 
			this.tabPageXml.Controls.Add(this.textBoxXmlBear);
			this.tabPageXml.Controls.Add(this.label1);
			this.tabPageXml.Location = new System.Drawing.Point(4, 28);
			this.tabPageXml.Name = "tabPageXml";
			this.tabPageXml.Size = new System.Drawing.Size(606, 180);
			this.tabPageXml.TabIndex = 2;
			this.tabPageXml.Text = "Xml";
			this.tabPageXml.UseVisualStyleBackColor = true;
			// 
			// textBoxXmlBear
			// 
			this.textBoxXmlBear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxXmlBear.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "XmlBearReg", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxXmlBear.Location = new System.Drawing.Point(106, 7);
			this.textBoxXmlBear.Multiline = true;
			this.textBoxXmlBear.Name = "textBoxXmlBear";
			this.textBoxXmlBear.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxXmlBear.Size = new System.Drawing.Size(487, 90);
			this.textBoxXmlBear.TabIndex = 14;
			this.textBoxXmlBear.Text = global::CrawlerDesktop.Properties.Settings.Default.XmlBearReg;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 18);
			this.label1.TabIndex = 13;
			this.label1.Text = "Bear:";
			// 
			// buttonJump
			// 
			this.buttonJump.Location = new System.Drawing.Point(5, 4);
			this.buttonJump.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.buttonJump.Name = "buttonJump";
			this.buttonJump.Size = new System.Drawing.Size(100, 34);
			this.buttonJump.TabIndex = 3;
			this.buttonJump.Text = "Jump";
			this.buttonJump.UseVisualStyleBackColor = true;
			this.buttonJump.Click += new System.EventHandler(this.buttonJump_Click);
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::CrawlerDesktop.Properties.Settings.Default, "CrawlerRootUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxUrl.Location = new System.Drawing.Point(115, 9);
			this.textBoxUrl.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(301, 25);
			this.textBoxUrl.TabIndex = 0;
			this.textBoxUrl.Text = global::CrawlerDesktop.Properties.Settings.Default.CrawlerRootUrl;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1132, 614);
			this.Controls.Add(this.splitContainerMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "FormMain";
			this.Text = "Crawler";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel2.ResumeLayout(false);
			this.splitContainerMain.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.panelOut.ResumeLayout(false);
			this.panelOut.PerformLayout();
			this.tabControlNode.ResumeLayout(false);
			this.tabPageMain.ResumeLayout(false);
			this.tabPageMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitRank)).EndInit();
			this.tabPageImage.ResumeLayout(false);
			this.tabPageImage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperSize)).EndInit();
			this.tabPageXml.ResumeLayout(false);
			this.tabPageXml.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxUrl;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.WebBrowser webBrowserMain;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.TextBox textBoxImageDirectory;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownUpperSize;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownLimitRank;
		private System.Windows.Forms.ProgressBar progressBarPages;
		private System.Windows.Forms.TextBox textBoxPages;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxImages;
		private System.Windows.Forms.ProgressBar progressBarImages;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownLowerSize;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.Button buttonJump;
		private System.Windows.Forms.Panel panelOut;
		private System.Windows.Forms.TabControl tabControlNode;
		private System.Windows.Forms.TabPage tabPageMain;
		private System.Windows.Forms.TextBox textBoxMainWhite;
		private System.Windows.Forms.Label labelMainWhite;
		private System.Windows.Forms.TabPage tabPageImage;
		private System.Windows.Forms.TabPage tabPageXml;
		private System.Windows.Forms.TextBox textBoxXmlBear;
		private System.Windows.Forms.Label label1;
	}
}

