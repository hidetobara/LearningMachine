using System;
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
			_Crawler = new WebCrawler(webBrowserMain, textBoxImageDirectory.Text);
			_Crawler.LimitSize = (int)numericUpDownLimitSize.Value;
			_Crawler.LimitRank = (int)numericUpDownLimitRank.Value;

			_Crawler.OnAddLog += AddLog;
			_Crawler.OnProgress += UpdateProgress;
			_Crawler.Start(textBoxUrl.Text);
			_Crawler.StartDownloading();
			Properties.Settings.Default.Save();
		}

		private void AddLog(string message)
		{
			textBoxLog.AppendText(message + Environment.NewLine);
		}

		private void UpdateProgress(int pageCount, int crawled)
		{
			progressBarPages.Maximum = pageCount;
			progressBarPages.Value = crawled;
			textBoxPages.Text = crawled + "/" + pageCount;
		}
	}
}
