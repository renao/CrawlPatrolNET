using System;
using CrawlPatrolNET.Crawler;

namespace CrawlPatrolNET
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var crawler = new PawPatrolCrawler();

            var episodes = crawler.FullEpisodes();

            Console.WriteLine("C'est finit");
        }
    }
}
