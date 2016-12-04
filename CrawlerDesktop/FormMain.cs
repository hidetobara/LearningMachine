﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrawlerDesktop
{
	public partial class FormMain : Form
	{
		WebCrawler _Crawler;

		public FormMain()
		{
			InitializeComponent();
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			if(_Crawler != null)
			{
				_Crawler.Close();
				_Crawler = null;
			}

			_Crawler = new WebCrawler(webBrowserMain, textBoxImageDirectory.Text);
			_Crawler.LimitSize = (int)numericUpDownLimitSize.Value;
			_Crawler.LimitRank = (int)numericUpDownLimitRank.Value;

			_Crawler.OnAddLog = AddLog;
			_Crawler.OnUpdatePageProgress = UpdatePageProgress;
			_Crawler.OnUpdateImageProgress = UpdateImageProgress;
			_Crawler.Open(textBoxUrl.Text);
			Properties.Settings.Default.Save();
		}

		private void AddLog(string message)
		{
			textBoxLog.AppendText(message + Environment.NewLine);
		}

		private void UpdatePageProgress(int all, int crawled)
		{
			progressBarPages.Maximum = all;
			progressBarPages.Value = crawled;
			textBoxPages.Text = crawled + "/" + all;
		}

		private void UpdateImageProgress(int all, int crawled)
		{
			progressBarImages.Maximum = all;
			progressBarImages.Value = crawled;
			textBoxImages.Text = crawled + "/" + all;
		}
	}
}