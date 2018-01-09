using System;

namespace CrawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
			args = new string[] { "--twitter-logs" };

			if (args.Length == 0 || args[0] == "--mercari")
			{
				CrawlerMercari crawler = new CrawlerMercari();
				crawler.Run();
			}
			if (args.Length == 0 || args[0] == "--twitter")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.Run();
			}
			if(args.Length == 0 || args[0] == "--twitter-logs")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.LoadType1or2(@"./logs/");
			}
		}
    }
}
