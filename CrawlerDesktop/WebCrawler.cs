using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;


namespace CrawlerDesktop
{
	class WebCrawler
	{
		WebBrowser _Browser;
		string _ImageDirectory;
		WebObject _CurrentWeb;

		public int LimitSize = 100;
		public int LimitRank = 3;

		Dictionary<string, WebObject> _Webs = new Dictionary<string, WebObject>();
		Dictionary<string, ImageObject> _Images = new Dictionary<string, ImageObject>();
		private void PushImage(string url) { lock (_Images) { _Images[url] = new ImageObject() { Url = url, ParentWeb = _CurrentWeb }; } }
		private string PopImage()
		{
			lock (_Images)
			{
				foreach (ImageObject o in _Images.Values) if (o.Status == DownloadStatus.None) { o.Status = DownloadStatus.Doing; return o.Url; }
				return null;
			}
		}
		private void SetImageStatus(string url, DownloadStatus s) { lock (_Images) { _Images[url].Status = s; } }

		public event Action<string> OnAddLog;

		public WebCrawler(WebBrowser browser, string dir)
		{
			_Browser = browser;
			//_Browser.ScriptErrorsSuppressed = true;
			_Browser.DocumentCompleted += DocumentCompleted;
			_ImageDirectory = dir;
		}

		private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			foreach(HtmlElement element in _Browser.Document.GetElementsByTagName("a"))
			{
				string url = element.GetAttribute("href");
				if (string.IsNullOrEmpty(url)) continue;
				if (IsIgnoring(url)) continue;
				if (IsImage(url))
				{
					PushImage(url);
					continue;
				}
				if (!IsHttp(url)) continue;
				if (_Webs.ContainsKey(url)) continue;
				_Webs[url] = new WebObject() { Rank = _CurrentWeb.Rank + 1, Url = url };
			}
			foreach(HtmlElement element in _Browser.Document.GetElementsByTagName("img"))
			{
				string src = element.GetAttribute("src");
				if (string.IsNullOrEmpty(src)) continue;
				if (!IsImage(src)) continue;
				if (_Images.ContainsKey(src)) continue;
				PushImage(src);
			}
			_CurrentWeb.IsCrawled = true;

			foreach(var w in _Webs.Values)
			{
				if (w.IsCrawled) continue;
				if (w.Rank > LimitRank) continue;

				try
				{
					_Browser.Url = new Uri(w.Url);
				}
				catch(Exception ex)
				{
					OnAddLog("[Error] URL=" + w.Url + " message=" + ex.Message + "@" + ex.StackTrace);
					if (_Browser.IsBusy) _Browser.Stop();
					System.Threading.Thread.Sleep(1);
					continue;
				}
				_CurrentWeb = w;
				break;
			}
			OnAddLog("[Info] crawled=" + CountCrawled() + "/" + _Webs.Count + " image=" + _Images.Count + " URL=" + _CurrentWeb.Url);
			GC.Collect();
		}

		private int CountCrawled() { return _Webs.Count(p => { return p.Value.IsCrawled; }); }

		public void Start(string url)
		{
			_CurrentWeb = new WebObject() { Rank = 1, Url = url };
			_Webs[url] = _CurrentWeb;
			_Browser.Url = new Uri(url);
		}

		private bool IsHttp(string url)
		{
			if (url.StartsWith("http:") || url.StartsWith("https:")) return true;
			return false;
		}

		private bool IsImage(string src)
		{
			string ext = Path.GetExtension(src).ToLower();
			if (ext == ".png" || ext == ".jpg" || ext == ".jpeg") return true;
			return false;
		}

		private bool IsIgnoring(string url)
		{
			string ext = Path.GetExtension(url).ToLower();
			if (ext == ".pdf") return true;	// pdfは不要
			if (url.Contains("#")) return true;	// ページ内ジャンプは不要
			return false;
		}

		public async void StartDownloading()
		{
			while(true)
			{
				string url = null;
				try
				{
					url = PopImage();
					if (string.IsNullOrEmpty(url))
					{
						await Task.Delay(new TimeSpan(0, 0, 1));
						continue;
					}
					WebClient client = new WebClient();
					string filename = Path.GetFileName(url);
					string path = Path.Combine(_ImageDirectory, filename);
					await client.DownloadFileTaskAsync(url, path);
					FileInfo info = new FileInfo(path);
					if(info.Length / 1024 < LimitSize)
					{
						File.Delete(path);
						SetImageStatus(url, DownloadStatus.Skip);
						continue;
					}
					SetImageStatus(url, DownloadStatus.Done);
					OnAddLog("[Image] downloaded=" + path);
				}
				catch(Exception ex)
				{
					if (!string.IsNullOrEmpty(url)) SetImageStatus(url, DownloadStatus.Error);
					OnAddLog("[Error]" + ex.Message + "@" + ex.StackTrace);
				}
			}
		}
	}

	class WebObject
	{
		public int Rank;
		public string Url;
		public bool IsCrawled;
	}

	enum DownloadStatus { None, Doing, Done, Skip, Error }
	class ImageObject
	{
		public string Url;
		public WebObject ParentWeb;
		public DownloadStatus Status;
	}
}
