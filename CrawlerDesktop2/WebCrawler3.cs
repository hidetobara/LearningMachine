using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace CrawlerDesktop2
{
	/*
	 * 進化したクローラー
	 */
	public class WebCrawler3
	{
		const int SLEEP_TIME = 1500;

		IWebDriver _Driver;
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

		private Thread _ThreadPage, _ThreadBear;
		Node _CurrentNode;

		public Action<string> OnAddLog;
		public Action<int, int> OnUpdatePageProgress;
		public Action<int, int> OnUpdateBearProgress;
		//public Action OnStop;
		public bool IsActive { private set; get; }

		public WebCrawler3(IWebDriver driver)
		{
			_Driver = driver;
		}

		public void Open(string url, int limit, List<Generator> generators)
		{
			_Nodes.Clear();

			_Generators = generators;
			var node = new Node() { Type = NodeType.Page, Url = url, Life = limit };
			AddNode(node);
			_ThreadPage = new Thread(DownloadingPage);
			_ThreadPage.Start();
			_ThreadBear = new Thread(DownloadingBear);
			_ThreadBear.Start(new Node() { Type = NodeType.Xml });
			IsActive = true;
		}

		public void Close()
		{
			IsActive = false;
			if (_ThreadPage != null && _ThreadPage.IsAlive) _ThreadPage.Abort();
			_ThreadPage = null;
			if (_ThreadBear != null && _ThreadBear.IsAlive) _ThreadBear.Abort();
			_ThreadBear = null;
		}

		private void DownloadingPage()
		{
			while(IsActive)
			{
				try
				{
					_CurrentNode = GetNodeNotYet(NodeType.Page);
					if (_CurrentNode == null) break;

					Thread.Sleep(SLEEP_TIME);
					lock (_Driver)
					{
						_Driver.Url = _CurrentNode.Url; // URL代入
						foreach (var root in _Generators)
						{
							foreach (var element in _Driver.FindElements(By.TagName("a")))
							{
								var url = element.GetAttribute(root.Attribute);
								if (string.IsNullOrEmpty(url)) continue;
								AddNode(root.Next(_CurrentNode, url));
							}
						}
					}
					_CurrentNode.Status = DownloadStatus.Done;
					OnAddLog("[page] life=" + _CurrentNode.Life + " url=" + _CurrentNode.Url);
					OnUpdatePageProgress(CountNodes(NodeType.Page), CountNodesCrawled(NodeType.Page));
					GC.Collect();
				}
				catch (Exception ex)
				{
					OnAddLog(ex.Message + "@" + ex.StackTrace);
					if (_CurrentNode != null) _CurrentNode.Status = DownloadStatus.Error;
				}
			}
		}

		private void DownloadingBear(Object o)
		{
			Node target = o as Node;
			if (o == null) return;
			Generator generator = null;
			foreach (var g in _Generators) if (g.BearingType == target.Type) generator = g;
			if (generator == null) return;

			Node node = null;
			while (IsActive)
			{
				try
				{
					int page = CountNodesCrawled(NodeType.Page);
					int all = CountNodes(target.Type);
					int crawled = CountNodesCrawled(target.Type);
					OnUpdateBearProgress(all, crawled);
					if (page > 0 && all == crawled)
					{
						IsActive = false;
						break;
					}

					node = GetNodeNotYet(target.Type);
					if (node == null || string.IsNullOrEmpty(node.Url))
					{
						Thread.Sleep(SLEEP_TIME);
						continue;
					}
					generator.Bear(node);
					node.Status = DownloadStatus.Done;
					OnAddLog("[bear] url=" + node.Url);
					Thread.Sleep(SLEEP_TIME);
				}
				catch (Exception ex)
				{
					OnAddLog(ex.Message + "@" + ex.StackTrace);
					if (node != null) node.Status = DownloadStatus.Error;
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
			private IWebDriver _Driver;

			public GeneratorXml()
			{
			}
			public GeneratorXml With(IWebDriver driver) { _Driver = driver; return this; }

			public override Node Next(Node current, string newUrl)
			{
				if (current.Type != NodeType.Page) return null;
				if (!Check(newUrl)) return null;

				return new Node() { Type = NodeType.Xml, Url = newUrl, Life = 0 };
			}

			public async override void Bear(Node node)
			{
				if (node.Type != NodeType.Xml) return;

				if (_Driver == null)
				{
					WebClient client = new WebClient();
					node.Bytes = await client.DownloadDataTaskAsync(node.Url);
				}
				else
				{
					lock (_Driver)
					{
						_Driver.Url = node.Url;
						node.Content = _Driver.PageSource;
					}
				}

				string[] boxes = node.Url.Split('?');
				string[] cells = boxes[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				string filename = Path.GetFileNameWithoutExtension(cells[cells.Length - 1]) + (boxes.Length > 1 ? "_" + Hash(boxes[1]) : "") + ".xml";
				string path = Path.Combine(DownloadDir, filename);
				if (node.Bytes != null) File.WriteAllBytes(path, node.Bytes);
				else if (node.Content != null) File.WriteAllText(path, node.Content);
			}
		}

		public class Node
		{
			public NodeType Type;
			public int Life;
			public string Url;
			public DownloadStatus Status;
			public byte[] Bytes;
			public string Content;
		}
	}
}
