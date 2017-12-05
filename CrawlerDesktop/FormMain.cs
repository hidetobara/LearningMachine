using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace CrawlerDesktop
{
	public partial class FormMain : Form
	{
		WebCrawler2 _Crawler;

		public FormMain()
		{
			InitializeComponent();
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			try
			{
				if (_Crawler != null)
				{
					_Crawler.Close();
					_Crawler = null;
					buttonStart.Text = "Start";
					return;
				}

				buttonJump.Enabled = false;

				_Crawler = new WebCrawler2(webBrowserMain, textBoxImageDirectory.Text);
				var main = new WebCrawler2.PageNode() { Life = (int)numericUpDownLimitRank.Value };
				if (textBoxMainWhite.Text.Length > 0)
				{
					var lines = textBoxMainWhite.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var line in lines) main.WhiteUrls.Add(new Regex(line));
				}
				_Crawler.AddRoot(main);
				if (numericUpDownUpperSize.Value > 0)
				{
					_Crawler.AddRoot(new WebCrawler2.ImageNode() { LowerSize = (int)numericUpDownLowerSize.Value, UpperSize = (int)numericUpDownUpperSize.Value });
				}
				if (textBoxXmlBear.Text.Length > 0)
				{
					var xml = new WebCrawler2.XmlNode();
					var lines = textBoxXmlBear.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var line in lines) xml.BearUrls.Add(new Regex(line));
					_Crawler.AddRoot(xml);
				}

				_Crawler.OnAddLog = AddLog;
				_Crawler.OnUpdatePageProgress = UpdatePageProgress;
				_Crawler.OnUpdateImageProgress = UpdateImageProgress;
				_Crawler.OnStop = StopCrawl;
				_Crawler.Open(textBoxUrl.Text);
				Properties.Settings.Default.Save();
				buttonStart.Text = "Stop";
			}
			catch(Exception ex)
			{
				AddLog(ex.Message + "@" + ex.StackTrace);
				if (_Crawler != null) { _Crawler.Close(); _Crawler = null; buttonStart.Text = "Start"; }
			}
		}

		private void AddLog(string message)
		{
			textBoxLog.AppendText(message + Environment.NewLine);
		}

		private void UpdatePageProgress(int all, int crawled)
		{
			if (crawled > all) crawled = all;
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

		private void buttonJump_Click(object sender, EventArgs e)
		{
			webBrowserMain.Navigate(textBoxUrl.Text);
		}

		private void StopCrawl()
		{
			buttonJump.Enabled = true;
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_Crawler != null) _Crawler.Close();
		}
	}
}
