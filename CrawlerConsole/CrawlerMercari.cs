using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Net;
using HtmlAgilityPack;

namespace CrawlerConsole
{
    class CrawlerMercari
    {
		const string URL_PURCHASED_50000 = @"https://www.mercari.com/jp/search/?sort_order=&keyword=&price_min=50000&price_max=&status_trading_sold_out=1";
		const string URL_SWITCH = @"https://www.mercari.com/jp/search/?sort_order=&keyword=任天堂+スイッチ&price_min=25000&price_max=&status_trading_sold_out=1";
		const string USER_AGENT = @"Mozilla/5.0 (Linux; U; Android 2.2.1; en-us; Nexus One Build/FRG83) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
		const string NUMBERS = @"[0-9]+";

		MySqlConnection _Connection;

		public CrawlerMercari()
		{
			string enviroment = "server=localhost; userid=baraoto; password=390831; database=mercari; CharSet=utf8";
			MySqlConnection conn = new MySqlConnection(enviroment);
			conn.Open();
			_Connection = conn;
		}

		public void Run()
		{
			RunPurchased50000();
			RunSwitch();
		}

		private void RunPurchased50000()
		{
			try
			{
				string html = DownloadFromMercari(URL_PURCHASED_50000);
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(html);
				foreach (var item in PickingUpItems(document))
				{
					Save("over_50000", item);
				}
				Console.WriteLine("Have run Purchase 50000.");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + ex.StackTrace);
			}
		}

		private void RunSwitch()
		{
			try
			{
				string html = DownloadFromMercari(URL_SWITCH);
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(html);
				foreach (var item in PickingUpItems(document))
				{
					Save("switch", item);
				}
				Console.WriteLine("Have Run Switch.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + ex.StackTrace);
			}
		}

		private string DownloadFromMercari(string url)
		{
			WebClient client = new WebClient();
			client.Headers["User-Agent"] = USER_AGENT;
			client.Headers["Content-Language"] = "ja-JP";
			client.Headers["Accept-Charset"] = "utf-8";
			client.Headers["Accept-Encoding"] = "gzip";
			var data = client.DownloadData(url);
			StreamReader reader = new StreamReader(new GZipStream(new MemoryStream(data), CompressionMode.Decompress));
			return reader.ReadToEnd();
		}

		private IEnumerable<Item> PickingUpItems(HtmlDocument document)
		{
			Regex regex = new Regex(NUMBERS);

			foreach (var node in document.DocumentNode.Descendants("section").Where(n => n.Attributes["class"].Value == "items-box"))
			{
				var a = node.Descendants("a").First();
				var cells = a.Attributes["href"].Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
				var title = node.Descendants().Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("items-box-name")).First();
				var price = node.Descendants("div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("items-box-price")).First();
				var match = regex.Match(price.InnerText.Replace(",", ""));
				if (!match.Success) continue;
				yield return new Item() { mid = cells.Last(), title = title.InnerText, price = match.Groups[0].Value };
			}
		}

		private void Save(string table, Item item)
		{
			Save(table, item.mid, item.title, item.price);
		}
		private void Save(string table, string mid, string title, string price)
		{
			MySqlCommand command;
			string sql;

			sql = string.Format("SELECT `id` FROM `{0}` WHERE `purchase_id` = '{1}'", table, mid);
			command = new MySqlCommand(sql, _Connection);
			var o = command.ExecuteScalar();
			if (o != null) return;

			string strDate = DateTime.Now.ToString("yyyy-MM-dd HH:00:00");
			sql = string.Format("INSERT INTO `{0}` (`at_purchased`, `purchase_id`, `price`, `title`) VALUES ('{1}', '{2}', {3}, '{4}')", table, strDate, mid, price, title);
			command = new MySqlCommand(sql, _Connection);
			command.ExecuteNonQuery();
		}

		class Item
		{
			public string mid;
			public string title;
			public string price;
		}
	}
}
