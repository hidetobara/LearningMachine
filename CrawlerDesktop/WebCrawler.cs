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
		WebObject _RootWeb;
		WebObject _CurrentWeb;
		System.Security.Cryptography.SHA256 _Sha256;

		public int LimitRank = 3;
		public bool IsFixedHost = true;
		public int LowerSize = 100;
		public int UpperSize = 1000;

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
		private void SetImageStatus(string url, DownloadStatus s, int size = 0) { lock (_Images) { var o = _Images[url]; o.Status = s; o.Size = size; } }

		public Action<string> OnAddLog;
		public Action<int, int> OnUpdatePageProgress;
		public Action<int, int> OnUpdateImageProgress;
		public Action<List<int>> OnUpdateChart;
		public Action OnStop;
		private bool _IsActive;

		public WebCrawler(WebBrowser browser, string dir)
		{
			_IsActive = true;
			_Browser = browser;
			//_Browser.ScriptErrorsSuppressed = true;
			_Browser.DocumentCompleted += DocumentCompleted;
			_ImageDirectory = dir;
			_Sha256 = System.Security.Cryptography.SHA256Managed.Create();
		}

		public void Close()
		{
			_IsActive = false;
			_Browser.DocumentCompleted -= DocumentCompleted;
		}

		private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			try
			{
				foreach (HtmlElement element in _Browser.Document.GetElementsByTagName("a"))
				{
					string url = element.GetAttribute("href");
					if (string.IsNullOrEmpty(url)) continue;
					if (IsIgnoring(url)) continue;
					if (IsImageExtension(url))
					{
						PushImage(url);
						continue;
					}
					if (!IsHttp(url)) continue;
					if (_Webs.ContainsKey(url)) continue;
					_Webs[url] = new WebObject() { Rank = _CurrentWeb.Rank + 1, Url = url };
				}
				foreach (HtmlElement element in _Browser.Document.GetElementsByTagName("img"))
				{
					string src = element.GetAttribute("src");
					if (string.IsNullOrEmpty(src)) continue;
					//if (!IsImage(src)) continue;
					if (_Images.ContainsKey(src)) continue;
					PushImage(src);
				}
			}
			catch (Exception ex)
			{
				OnAddLog(ex.Message + "@" + _CurrentWeb.Url);
			}
			finally
			{
				_CurrentWeb.IsCrawled = true;
			}

			bool isJumping = false;
			foreach(var w in _Webs.Values)
			{
				if (w.IsCrawled) continue;
				if (w.Rank > LimitRank) continue;
				if (IsFixedHost && !w.IsFixedHost(_RootWeb)) continue;

				try
				{
					_Browser.Url = new Uri(w.Url);
				}
				catch(Exception ex)
				{
					OnAddLog("[Error] URL=" + w.Url + " message=" + ex.Message + "@" + ex.StackTrace);
					if (_Browser.IsBusy) _Browser.Stop();
					w.IsCrawled = true;
					System.Threading.Thread.Sleep(1);
					continue;
				}
				_CurrentWeb = w;
				isJumping = true;
				break;
			}
			OnUpdatePageProgress(CountPagesGoingToCrawl(), CountPagesCrawled());
			GC.Collect();

			if(isJumping)
			{
				OnAddLog("[Info] URL=" + _CurrentWeb.Url);
			}
			else
			{
				OnStop();
			}
		}

		private int CountPagesCrawled() { return _Webs.Count(p => { return p.Value.IsCrawled; }); }
		private int CountPagesGoingToCrawl() { return _Webs.Count(p => { return p.Value.Rank <= LimitRank && (IsFixedHost && p.Value.IsFixedHost(_RootWeb)); }); }
		private int CountImagesCrawled() { return _Images.Count(p => { return p.Value.IsCrawled; }); }
		private int CountImages() { return _Images.Count; }

		public void Open(string url)
		{
			_RootWeb = new WebObject() { Rank = 1, Url = url };
			_CurrentWeb = _RootWeb;
			_Webs[url] = _RootWeb;
			_Browser.Url = new Uri(url);
			StartDownloading();
		}

		private bool IsHttp(string url)
		{
			if (url.StartsWith("http:") || url.StartsWith("https:")) return true;
			return false;
		}

		private bool IsImageExtension(string src)
		{
			string ext = Path.GetExtension(src).ToLower();
			if (ext == ".png" || ext == ".jpg" || ext == ".jpeg") return true;
			return false;
		}

		private bool IsIgnoring(string url)
		{
			string ext = Path.GetExtension(url).ToLower();
			if (ext == ".pdf") return true; // pdfは不要
			if (ext == ".mp3" || ext == ".ogg" || ext == ".wma" || ext == ".wav") return true;
			if (ext == ".mp4" || ext == ".avi" || ext == ".wmv" || ext == ".mov") return true;
			if (ext == ".doc" || ext == ".docx") return true;
			if (ext == ".xls" || ext == ".xlsx") return true;
			if (url.Contains("#")) return true;	// ページ内ジャンプは不要
			return false;
		}

		private async void StartDownloading()
		{
			while(_IsActive)
			{
				string url = null;
				try
				{
					OnUpdateImageProgress(CountImages(), CountImagesCrawled());
					if (CountImagesCrawled() % 20 == 19) UpdatingChart();

					url = PopImage();
					if (string.IsNullOrEmpty(url))
					{
						await Task.Delay(new TimeSpan(0, 0, 1));
						continue;
					}
					WebClient client = new WebClient();
					byte[] bytes = await client.DownloadDataTaskAsync(url);
					int size = 0;
					if (bytes != null) size = bytes.Length / 1024;
					if (size < LowerSize || UpperSize < size)
					{
						SetImageStatus(url, DownloadStatus.Skip, size);
						continue;
					}
					string filename = Path.GetFileName(url);
					if (!IsImageExtension(filename))
					{
						filename = Hash(url);
						if (IsPng(bytes)) filename += ".png";
						else if (IsJpg(bytes)) filename += ".jpg";
						else throw new Exception("Unknown extension url=" + url);
					}
					string path = Path.Combine(_ImageDirectory, filename);
					File.WriteAllBytes(path, bytes);
					SetImageStatus(url, DownloadStatus.Done, size);
					if (_IsActive) OnAddLog("[Image] downloaded=" + url);
				}
				catch (Exception ex)
				{
					if (!string.IsNullOrEmpty(url)) SetImageStatus(url, DownloadStatus.Error);
					if (_IsActive) OnAddLog("[Error]" + ex.Message + "@" + ex.StackTrace);
				}
			}
		}

		private string Hash(string str)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
			byte[] hashed =_Sha256.ComputeHash(bytes);
			StringBuilder output = new StringBuilder();
			for (int i = 0; i < 8; i++)
				output.Append(hashed[i].ToString("x2"));
			return output.ToString();
		}

		private bool IsPng(byte[] bytes)
		{
			if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return true;
			return false;
		}

		private bool IsJpg(byte[] bytes)
		{
			if (bytes[0] == 0xFF && bytes[1] == 0xD8) return true;
			return false;
		}

		private void UpdatingChart()
		{
			List<int> sizes = new List<int>();
			lock (_Images)
			{
				foreach (var o in _Images.Values) if (o.IsCrawled) sizes.Add(o.Size);
			}
			OnUpdateChart(sizes);
		}
	}

	class WebObject
	{
		public int Rank;
		public string Url;
		public bool IsCrawled;

		public string HostName { get { Uri u = new Uri(Url); return u.DnsSafeHost; } }
		public bool IsFixedHost(WebObject root) { return this.Url.Contains(root.HostName); }
	}

	enum DownloadStatus { None, Doing, Done, Skip, Error }
	class ImageObject
	{
		public string Url;
		public int Size;
		public WebObject ParentWeb;
		public DownloadStatus Status;
		
		public bool IsCrawled { get { return Status == DownloadStatus.Done || Status == DownloadStatus.Skip || Status == DownloadStatus.Error; } }
	}
}
