using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CrawlerDesktop
{
	public partial class FormMain : Form
	{
		WebCrawler _Crawler;

		public FormMain()
		{
			InitializeComponent();
			UpdateChart(null);
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			if(_Crawler != null)
			{
				_Crawler.Close();
				_Crawler = null;
			}

			_Crawler = new WebCrawler(webBrowserMain, textBoxImageDirectory.Text);
			_Crawler.LimitRank = (int)numericUpDownLimitRank.Value;
			_Crawler.IsFixedHost = checkBoxHostFixed.Checked;
			_Crawler.LowerSize = (int)numericUpDownLowerSize.Value;
			_Crawler.UpperSize = (int)numericUpDownUpperSize.Value;

			_Crawler.OnAddLog = AddLog;
			_Crawler.OnUpdatePageProgress = UpdatePageProgress;
			_Crawler.OnUpdateImageProgress = UpdateImageProgress;
			_Crawler.OnUpdateChart = UpdateChart;
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

		private void UpdateChart(List<int> list)
		{
			chartResult.Series.Clear();
			chartResult.Legends.Clear();
			if (list == null || list.Count == 0) return;

			Series series = new Series();
			series.ChartType = SeriesChartType.Column;
			Dictionary<int, int> table = new Dictionary<int, int>();
			foreach(int value in list)
			{
				int key = (value / 10) * 10;
				if (!table.ContainsKey(key)) table[key] = 1; else table[key] += 1;
			}
			foreach (var pair in table) series.Points.AddXY(pair.Key, pair.Value);
			series.Name = "Size(KB)";

			Legend legend = new Legend();
			legend.DockedToChartArea = "ChartArea1";
			legend.Alignment = StringAlignment.Near;

			chartResult.Series.Add(series);
			chartResult.Legends.Add(legend);
		}
	}
}
