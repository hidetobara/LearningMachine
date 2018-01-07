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
		List<Generator> _Generators;

		Dictionary<string, Node> _Nodes = new Dictionary<string, Node>();
		void AddNode(Node n) { if (n == null) return; lock (_Nodes) { if (!_Nodes.ContainsKey(n.Url)) _Nodes[n.Url] = n; } }
		Node GetNodeNotYet(NodeType type)
		{
			lock (_Nodes) { foreach (var p in _Nodes) if (p.Value.Type == type && p.Value.Status == DownloadStatus.NotYet) return p.Value; }
			return null;
		}
		private int CountNodesCrawled(NodeType type) { return _Nodes.Count(p => { return p.Value.Type == type && p.Value.Status == DownloadStatus.Done; }); }
		private int CountNodes(NodeType type) { return _Nodes.Count(p => { return p.Value.Type == type; }); }

		Node _CurrentNode;

		public Action<string> OnAddLog;
		public Action<int, int> OnUpdatePageProgress;
		public Action<int, int> OnUpdateImageProgress;
		public Action<string> OnLoadPage;
		public Action OnStop;
		private bool _IsActive;

		public WebCrawler2(WebBrowser browser)
		{
			_IsActive = true;
			_Browser = browser;
			_Browser.DocumentCompleted += DocumentCompleted;
		}


		public void Open(string url, int life, List<Generator> generators)
		{
			_Generators = generators;
			_CurrentNode = new Node() { Type = NodeType.Page, Url = url, Life = life };
			_Nodes[url] = _CurrentNode;
			_Browser.Url = new Uri(url);
			StartDownloading(NodeType.Xml);	// 要調整
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
				foreach (var root in _Generators)
				{
					foreach (HtmlElement element in _Browser.Document.GetElementsByTagName(root.Tag))
					{
						string url = element.GetAttribute(root.Attribute);
						if (string.IsNullOrEmpty(url)) continue;
						AddNode(root.Next(_CurrentNode, url));
					}
				}
			}
			catch (Exception ex)
			{
				OnAddLog(ex.Message + "@" + _CurrentNode.Url);
			}
			finally
			{
				OnLoadPage(_CurrentNode.Url);
				_CurrentNode.Status = DownloadStatus.Done;
			}

			bool isJumping = false;
			while(true)
			{
				var node = GetNodeNotYet(NodeType.Page);
				if (node == null) break;

				try
				{
					_Browser.Url = new Uri(node.Url);
				}
				catch (Exception ex)
				{
					OnAddLog("[Error] URL=" + node.Url + " message=" + ex.Message + "@" + ex.StackTrace);
					if (_Browser.IsBusy) _Browser.Stop();
					node.Status = DownloadStatus.Error;
					System.Threading.Thread.Sleep(1);
					continue;
				}
				_CurrentNode = node;
				isJumping = true;
				break;
			}
			OnUpdatePageProgress(CountNodes(NodeType.Page), CountNodesCrawled(NodeType.Page));
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

		private async void StartDownloading(NodeType type)
		{
			Generator generator = null;
			foreach (var g in _Generators) if (g.BearingType == type) generator = g;
			if (generator == null) return;

			while (_IsActive)
			{
				Node node = null;
				try
				{
					node = GetNodeNotYet(NodeType.Xml);
					OnUpdateImageProgress(CountNodes(NodeType.Xml), CountNodesCrawled(NodeType.Xml));

					if (node == null || string.IsNullOrEmpty(node.Url))
					{
						await Task.Delay(new TimeSpan(0, 0, 10));
						continue;
					}
					generator.Bear(node);
					node.Status = DownloadStatus.Done;
					OnAddLog("[Image] beared=" + node.Url);
					await Task.Delay(new TimeSpan(0, 0, 3));
				}
				catch (Exception ex)
				{
					if (node != null) node.Status = DownloadStatus.Error;
					OnAddLog("[Error] " +  ex.Message + "@" + ex.StackTrace);
				}
			}
		}

		public enum DownloadStatus { NotYet, Doing, Done, Skip, Error }
		public enum NodeType { None, Page, Image, Xml }

		public class Generator
		{
			static System.Security.Cryptography.SHA256 _Sha256 = System.Security.Cryptography.SHA256Managed.Create();

			public virtual NodeType BearingType { get; }
			public virtual string Tag { get; }
			public virtual string Attribute { get; }

			public List<Regex> WhiteUrls = new List<Regex>();
			public List<Regex> BlackUrls = new List<Regex>();

			public virtual Node Next(Node node, string newUrl) { return null; }
			public async virtual void Bear(Node node) { await Task.Delay(new TimeSpan(0, 0, 1)); }

			protected bool Check(string url)
			{
				foreach (var r in WhiteUrls)
				{
					var match = r.Match(url);
					if (!match.Success) return false;
				}
				foreach (var r in BlackUrls)
				{
					var match = r.Match(url);
					if (match.Success) return false;
				}
				return true;
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

		public class GeneratorPage : Generator
		{
			public override NodeType BearingType { get { return NodeType.Page; } }
			public override string Tag { get { return "a"; } }
			public override string Attribute { get { return "href"; } }

			public GeneratorPage()
			{
				string[] extensions = new string[] { "pdf", "mp3", "ogg", "wma", "wav", "mp4", "avi", "wmv", "wov", "flv", "doc", "docx", "xls", "xlsx" };
				foreach (var e in extensions) BlackUrls.Add(new Regex(@"\." + e + "$"));
			}

			public override Node Next(Node current, string newUrl)
			{
				if (current.Type != NodeType.Page) return null;
				if (current.Life <= 0) return null;
				if (!Check(newUrl)) return null;

				return new Node() { Type = NodeType.Page, Url = newUrl, Life = current.Life - 1 };
			}
		}

		public class GeneratorImage : Generator
		{
			public override NodeType BearingType { get { return NodeType.Image; } }
			public override string Tag { get { return "img"; } }
			public override string Attribute { get { return "src"; } }
			public string DownloadDir;
			public int UpperSize, LowerSize;

			public GeneratorImage()
			{
				string[] extensions = new string[] { "png", "jpeg", "jpg" };
				foreach (var e in extensions) WhiteUrls.Add(new Regex(@"\." + e + "$"));
			}

			public override Node Next(Node current, string newUrl)
			{
				if (current.Type != NodeType.Page) return null;
				if (!Check(newUrl)) return null;

				return new Node() { Type = NodeType.Image, Url = newUrl, Life = 0 };
			}

			public async override void Bear(Node node)
			{
				if (node.Type != NodeType.Image) return;

				WebClient client = new WebClient();
				node.Bytes = await client.DownloadDataTaskAsync(node.Url);

				int kbytes = node.Bytes.Length / 1024;
				if (kbytes < LowerSize || UpperSize < kbytes)
				{
					node.Status = DownloadStatus.Skip;
					return;
				}
				string filename = Hash(node.Url) + "_" + Path.GetFileName(node.Url);
				string path = Path.Combine(DownloadDir, filename);
				File.WriteAllBytes(path, node.Bytes);
			}
		}

		public class GeneratorXml : Generator
		{
			public override NodeType BearingType { get { return NodeType.Xml; } }
			public override string Tag { get { return "a"; } }
			public override string Attribute { get { return "href"; } }
			public string DownloadDir;

			public GeneratorXml()
			{
			}

			public override Node Next(Node current, string newUrl)
			{
				if (current.Type != NodeType.Page) return null;
				if (!Check(newUrl)) return null;

				return new Node() { Type = NodeType.Xml, Url = newUrl, Life = 0 };
			}

			public async override void Bear(Node node)
			{
				if (node.Type != NodeType.Xml) return;

				WebClient client = new WebClient();
				node.Bytes = await client.DownloadDataTaskAsync(node.Url);

				string[] cells = node.Url.Split('?');
				string filename = Hash(node.Url) + "_" + Path.GetFileNameWithoutExtension(cells[0]) + ".xml";
				string path = Path.Combine(DownloadDir, filename);
				File.WriteAllBytes(path, node.Bytes);
			}
		}

		public class Node
		{
			public NodeType Type;
			public int Life;
			public string Url;
			public DownloadStatus Status;
			public byte[] Bytes;
		}
	}
}
