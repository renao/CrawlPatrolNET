using System;
using CrawlPatrolNET.Crawler;
using CrawlPatrolNET.TelegramBot;

namespace CrawlPatrolNET
{
    class Program
    {
        private static PawPatrolCrawler Crawler;
        private static Bot Bot;

        private static EpisodeStore EpisodeStore;

        static void Main(string[] args)
        {
            PrepareCrawler();
            PrepareBot();

            StartBot();
            StartCrawler();

            Console.ReadKey();
        }

        private static void PrepareCrawler()
        {
            EpisodeStore = new EpisodeStore();
            Crawler = new PawPatrolCrawler(EpisodeStore);
        }

        private static void PrepareBot()
        {
            Bot = new Bot();
        }

        private static void StartCrawler()
        {
            Crawler.Crawl();
        }

        private static void StartBot()
        {
            Bot.ListenTo(EpisodeStore);
        }
    }
}
