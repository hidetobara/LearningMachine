using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Threading;
using System.Text.RegularExpressions;


namespace CrawlerDesktop2
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		// ブラウザ
		IWebDriver _Driver;
		// タイマー
		DispatcherTimer _DispatcherTimer;
		// クローラー
		WebCrawler3 _Crawler;

		public MainWindow()
		{
			InitializeComponent();

			LoadSettings();

			_Driver = new ChromeDriver();
			_Driver.Url = "https://www.yahoo.co.jp/";

			_Crawler = new WebCrawler3(_Driver);
			_Crawler.OnAddLog = AddLog;
			_Crawler.OnUpdatePageProgress = UpdatePageProgress;
			_Crawler.OnUpdateBearProgress = UpdateBearProgress;

			_DispatcherTimer = new DispatcherTimer();
			_DispatcherTimer.Interval = new TimeSpan(0, 0, 3);
			_DispatcherTimer.Tick += DispatcherTimer_Tick;
		}

		private void LoadSettings()
		{
			TextBoxSaveDir.Text = Properties.Settings.Default.SaveDir;
			TextBoxWhitePage.Text = Properties.Settings.Default.WhitePage;
			UpDownLimitRank.Value = Properties.Settings.Default.LimitRank;
			UpDownLowerSize.Value = Properties.Settings.Default.LowerSize;
			UpDownUpperSize.Value = Properties.Settings.Default.UpperSize;
			TextBoxWhiteXml.Text = Properties.Settings.Default.WhiteBear;
		}

		private void SaveSettings()
		{
			Properties.Settings.Default.SaveDir = TextBoxSaveDir.Text;
			Properties.Settings.Default.WhitePage = TextBoxWhitePage.Text;
			Properties.Settings.Default.LimitRank = (int)UpDownLimitRank.Value;
			Properties.Settings.Default.LowerSize = (int)UpDownLowerSize.Value;
			Properties.Settings.Default.UpperSize = (int)UpDownUpperSize.Value;
			Properties.Settings.Default.WhiteBear = TextBoxWhiteXml.Text;
			Properties.Settings.Default.Save();
		}
		
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (_Crawler.IsActive)
			{
				_Crawler.Close();
				ButtonRun.Content = "Run";
				return;
			}

			SaveSettings();
			TextBoxLog.Clear();

			if (!Directory.Exists(TextBoxSaveDir.Text)) Directory.CreateDirectory(TextBoxSaveDir.Text);

			var generators = new List<WebCrawler3.Generator>();
			if (TextBoxWhitePage.Text.Length > 0)
			{
				string[] cells = TextBoxWhitePage.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				List<Regex> regexs = new List<Regex>();
				foreach (var cell in cells) regexs.Add(new Regex(cell));
				generators.Add(new WebCrawler3.GeneratorPage() { WhiteUrls = regexs });
			}
			if ((int)UpDownLowerSize.Value != (int)UpDownUpperSize.Value)
			{
				var g = new WebCrawler3.GeneratorImage() { LowerSize = (int)UpDownLowerSize.Value, UpperSize = (int)UpDownUpperSize.Value };
				g.DownloadDir = TextBoxSaveDir.Text;
				generators.Add(g);
			}
			if (TextBoxWhiteXml.Text.Length > 0)
			{
				string[] cells = TextBoxWhiteXml.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				List<Regex> regexs = new List<Regex>();
				foreach (var cell in cells) regexs.Add(new Regex(cell));
				generators.Add(new WebCrawler3.GeneratorXml() { WhiteUrls = regexs, DownloadDir = TextBoxSaveDir.Text }.With(_Driver));
			}

			_Crawler.Open(_Driver.Url, (int)UpDownLimitRank.Value, generators);
			_DispatcherTimer.Start();
			ButtonRun.Content = "Running";
		}

		private void Main_Closed(object sender, EventArgs e)
		{
			if (_Driver != null) _Driver.Quit();
			_Driver = null;
			if (_Crawler != null) _Crawler.Close();
			_Crawler = null;
		}

		#region 非同期
		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			lock(_Logs)
			{
				foreach (string log in _Logs) TextBoxLog.Text += log + Environment.NewLine;
				_Logs.Clear();
			}
			TextBlockPages.Text = string.Format("{0:D}/{1:D}", _PageDone, _PageAll);
			if (_PageAll > 0) ProgressBarPages.Value = (double)_PageDone * 100 / (double)_PageAll;
			TextBlockBears.Text = string.Format("{0:D}/{1:D}", _BearDone, _BearAll);
			if (_BearAll > 0) ProgressBarBears.Value = (double)_BearDone * 100 / (double)_BearAll;

			if (this.IsActive)
			{
				TextBoxLog.CaretIndex = TextBoxLog.Text.Length;
				TextBoxLog.ScrollToEnd();
			}
			else
			{
				ButtonRun.Content = "Run";
				_DispatcherTimer.Stop();
			}
		}

		List<string> _Logs = new List<string>();
		private int _PageDone, _PageAll, _BearDone, _BearAll;

		public void AddLog(string s)
		{
			lock (_Logs) { _Logs.Add(s); }
		}
		public void UpdatePageProgress(int all, int done)
		{
			_PageAll = all;
			_PageDone = done;
		}
		public void UpdateBearProgress(int all, int done)
		{
			_BearAll = all;
			_BearDone = done;
		}
		#endregion
	}
}
