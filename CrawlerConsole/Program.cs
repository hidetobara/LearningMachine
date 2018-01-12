using System;

namespace CrawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
			args = new string[] { "--twitter-search" };
#endif

			if (args.Length == 0 || args[0] == "--mercari")
			{
				CrawlerMercari crawler = new CrawlerMercari();
				crawler.Run();
			}
			if (args.Length == 0 || args[0] == "--twitter")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.RunHome();
			}
			if (args.Length == 0 || args[0] == "--twitter-search")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.SearchTags();
			}
			if (args.Length == 0 || args[0] == "--twitter-logs")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.LoadType1or2(@"./logs/");
			}
			if (args.Length == 0 || args[0] == "--twitter-csvs")
			{
				CrawlerTwitter twitter = new CrawlerTwitter();
				twitter.LoadCsvs(@"./csvs/");
			}
			if (args.Length == 0 || args[0] == "--rain")
			{
				CrawlerRain rain = new CrawlerRain();
				rain.Run(args.Length > 1 ? args[1] : "./");
			}
		}
    }
}
