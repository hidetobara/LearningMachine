using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace CrawlerConsole
{
    class CrawlerRain
    {
		const string SRC_URL = "http://tokyo-ame.jwa.or.jp/mesh/000/";

		public CrawlerRain()
		{

		}

		public void Run(string datadir)
		{
			WebClient clinet = new WebClient();
			var now = DateTime.Now;
			for (int minute = 5; minute < 15; minute += 5)
			{
				string url = "";
				try
				{
					DateTime target = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 5) * 5, 0) - new TimeSpan(0, minute, 0);
					string subdir = target.ToString("yyyyMM/dd");
					string filename = target.ToString("yyyyMMddHHmm") + ".gif";

					url = SRC_URL + filename;
					string dir = Path.Combine(datadir, subdir);
					if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
					string path = Path.Combine(dir, filename);
					if (File.Exists(path)) continue;
					clinet.DownloadFile(url, path);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message + "@" + ex.StackTrace + " url=" + url);
				}
			}
			Console.WriteLine("Have run rain");
		}
    }
}
