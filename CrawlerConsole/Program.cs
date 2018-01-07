using System;

namespace CrawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
			CrawlerMercari crawler = new CrawlerMercari();
			crawler.Run();
        }
    }
}
