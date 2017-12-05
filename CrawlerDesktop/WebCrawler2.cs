using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace CrawlerDesktop
{
	/*
	 * 進化したクローラー
	 */
	public class WebCrawler2
	{
		WebBrowser _Browser;
		string _SaveDirectory;

		List<PageNode> _Roots = new List<PageNode>();
		Dictionary<string, PageNode> _Nodes = new Dictionary<string, PageNode>();
		void AddNode(PageNode n) { lock (_Nodes) { if (!_Nodes.ContainsKey(n.Url)) _Nodes[n.Url] = n; } }
		Dictionary<string, PageNode> _Bears = new Dictionary<string, PageNode>();
		void AddBear(PageNode n) { lock (_Bears) { if (!_Bears.ContainsKey(n.Url)) _Bears[n.Url] = n; } }
		PageNode GetNotYetBear()
		{
			lock (_Bears) { foreach (var p in _Bears) if (p.Value.Crawling == DownloadStatus.NotYet) return p.Value; }
			return null;
		}
		PageNode _CurrentNode;

		public Action<string> OnAddLog;
		public Action<int, int> OnUpdatePageProgress;
		public Action<int, int> OnUpdateImageProgress;
		public Action OnStop;
		private bool _IsActive;

		public WebCrawler2(WebBrowser browser, string dir)
		{
			_IsActive = true;
			_Browser = browser;
			_Browser.DocumentCompleted += DocumentCompleted;
			_SaveDirectory = dir;
		}

		public void AddRoot(PageNode root) { _Roots.Add(root); }

		public void Open(string url)
		{
			_CurrentNode = new PageNode() { Url = url };
			_Nodes[url] = _CurrentNode;
			_Browser.Url = new Uri(url);
			StartDownloading();
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
				foreach (var root in _Roots)
				{
					foreach (HtmlElement element in _Browser.Document.GetElementsByTagName(root.Tag))
					{
						string url = element.GetAttribute(root.Attribute);
						if (string.IsNullOrEmpty(url)) continue;
						if (root.CheckCrawl(url)) AddNode(root.Clone(url));
						if (root.CheckBear(url)) AddBear(root.Clone(url));
					}
				}
			}
			catch (Exception ex)
			{
				OnAddLog(ex.Message + "@" + _CurrentNode.Url);
			}
			finally
			{
				_CurrentNode.Crawling = DownloadStatus.Done;
			}

			bool isJumping = false;
			foreach (var node in _Nodes.Values)
			{
				if (node.Crawling != DownloadStatus.NotYet) continue;

				try
				{
					_Browser.Url = new Uri(node.Url);
				}
				catch (Exception ex)
				{
					OnAddLog("[Error] URL=" + node.Url + " message=" + ex.Message + "@" + ex.StackTrace);
					if (_Browser.IsBusy) _Browser.Stop();
					node.Crawling = DownloadStatus.Error;
					System.Threading.Thread.Sleep(1);
					continue;
				}
				_CurrentNode = node;
				isJumping = true;
				break;
			}
			OnUpdatePageProgress(CountPagesGoingToCrawl(), CountPagesCrawled());
			GC.Collect();

			if (isJumping)
			{
				OnAddLog("[Info] Life=" + _CurrentNode.Life + " URL=" + _CurrentNode.Url);
			}
			else
			{
				OnStop();
			}
		}

		private async void StartDownloading()
		{
			while (_IsActive)
			{
				PageNode node = null;
				try
				{
					node = GetNotYetBear();
					OnUpdateImageProgress(CountBears(), CountBearsCrawled());

					if (node == null || string.IsNullOrEmpty(node.Url))
					{
						await Task.Delay(new TimeSpan(0, 0, 1));
						continue;
					}
					WebClient client = new WebClient();
					node.Bytes = await client.DownloadDataTaskAsync(node.Url);
					node.Bear(_SaveDirectory);
					OnAddLog("[Image] downloaded=" + node.Url);
				}
				catch (Exception ex)
				{
					if (node != null) node.Crawling = DownloadStatus.Error;
					OnAddLog("[Error] " +  ex.Message + "@" + ex.StackTrace);
				}
			}
		}

		private int CountPagesCrawled() { return _Nodes.Count(p => { return p.Value.Crawling == DownloadStatus.Done; }); }
		private int CountPagesGoingToCrawl() { return _Nodes.Count(p => { return p.Value.Crawling == DownloadStatus.NotYet; }); }
		private int CountBearsCrawled() { return _Bears.Count(p => { return p.Value.Crawling == DownloadStatus.Done; }); }
		private int CountBears() { return _Bears.Count; }

		public enum DownloadStatus { NotYet, Doing, Done, Skip, Error }

		public class PageNode
		{
			static System.Security.Cryptography.SHA256 _Sha256 = System.Security.Cryptography.SHA256Managed.Create();

			public virtual string Tag { get { return "a"; } }
			public virtual string Attribute { get { return "href"; } }

			public string Url;
			public byte[] Bytes;
			public int Life;
			public DownloadStatus Crawling;
			public List<Regex> WhiteUrls = new List<Regex>();
			public List<Regex> BlackUrls = new List<Regex>();
			public List<Regex> BearUrls = new List<Regex>();

			public PageNode()
			{
				string[] extensions = new string[] { "pdf", "mp3", "ogg", "wma", "wav", "mp4", "avi", "wmv", "wov", "flv", "doc", "docx", "xls", "xlsx" };
				foreach (var e in extensions) BlackUrls.Add(new Regex(@"\." + e + "$"));
			}

			public virtual PageNode Clone(string url) { return new PageNode() { Url = url, Life = this.Life - 1 }; }

			public virtual bool CheckCrawl(string url)
			{
				if (Life <= 0) return false;

				foreach(var r in WhiteUrls)
				{
					var match = r.Match(url);
					if (!match.Success) return false;
				}
				foreach(var r in BlackUrls)
				{
					var match = r.Match(url);
					if (match.Success) return false;
				}
				return true;
			}

			public virtual bool CheckBear(string url)
			{
				foreach (var r in BearUrls)
				{
					var match = r.Match(url);
					if (match.Success) return true;
				}
				return false;
			}

			public virtual bool Bear(string dir)
			{
				Crawling = DownloadStatus.Done;
				return false;
			}

			protected static string Hash(string s)
			{
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
				byte[] hashed = _Sha256.ComputeHash(bytes);
				StringBuilder output = new StringBuilder();
				for (int i = 0; i < 8; i++) output.Append(hashed[i].ToString("x2"));
				return output.ToString();
			}
		}

		public class ImageNode : PageNode
		{
			public override string Tag { get { return "img"; } }
			public override string Attribute { get { return "src"; } }

			public int UpperSize, LowerSize;

			public ImageNode()
			{
				string[] extensions = new string[] { "png", "jpeg", "jpg" };
				foreach (var e in extensions) BearUrls.Add(new Regex(@"\." + e + "$"));
			}

			public override PageNode Clone(string url)
			{
				return new ImageNode() { Url = url, Life = 0, LowerSize = this.LowerSize, UpperSize = this.UpperSize };
			}

			public override bool Bear(string dir)
			{
				int kbytes = Bytes.Length / 1024;
				if (kbytes < LowerSize || UpperSize < kbytes)
				{
					Crawling = DownloadStatus.Skip;
					return false;
				}
				string filename = Hash(Url) + "_" + Path.GetFileName(Url);
				string path = Path.Combine(dir, filename);
				File.WriteAllBytes(path, Bytes);
				Crawling = DownloadStatus.Done;
				return true;
			}
		}

		public class XmlNode : PageNode
		{
			public override string Tag { get { return "a"; } }
			public override string Attribute { get { return "href"; } }

			public XmlNode()
			{

			}

			public override PageNode Clone(string url) { return new XmlNode() { Url = url, Life = 0 }; }

			public override bool Bear(string dir)
			{
				string[] cells = Url.Split('?');
				string filename = Hash(Url) + "_" + Path.GetFileNameWithoutExtension(cells[0]) + ".xml";
				string path = Path.Combine(dir, filename);
				File.WriteAllBytes(path, Bytes);
				Crawling = DownloadStatus.Done;
				return true;
			}
		}
	}
}
