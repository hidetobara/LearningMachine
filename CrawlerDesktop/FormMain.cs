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

				var generators = new List<WebCrawler2.Generator>();
				_Crawler = new WebCrawler2(webBrowserMain);
				var main = new WebCrawler2.GeneratorPage();
				if (textBoxMainWhite.Text.Length > 0)
				{
					var lines = textBoxMainWhite.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var line in lines) main.WhiteUrls.Add(new Regex(line));
				}
				generators.Add(main);
				if (numericUpDownUpperSize.Value > 0)
				{
					generators.Add(new WebCrawler2.GeneratorImage() { LowerSize = (int)numericUpDownLowerSize.Value, UpperSize = (int)numericUpDownUpperSize.Value, DownloadDir = textBoxImageDirectory.Text });
				}
				if (textBoxXmlBear.Text.Length > 0)
				{
					var xml = new WebCrawler2.GeneratorXml() { DownloadDir = textBoxImageDirectory.Text };
					var lines = textBoxXmlBear.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var line in lines) xml.WhiteUrls.Add(new Regex(line));
					generators.Add(xml);
				}

				_Crawler.OnAddLog = AddLog;
				_Crawler.OnUpdatePageProgress = UpdatePageProgress;
				_Crawler.OnUpdateImageProgress = UpdateImageProgress;
				_Crawler.OnStop = StopCrawl;
				_Crawler.OnLoadPage = LoadPage;
				_Crawler.Open(textBoxUrl.Text, (int)numericUpDownLimitRank.Value, generators);
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

		private void LoadPage(string url)
		{
			textBoxUrl.Text = url;
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_Crawler != null) _Crawler.Close();
		}
	}
}
