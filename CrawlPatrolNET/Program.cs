using System;
using System.Timers;
using CrawlPatrolNET.Crawler;
using CrawlPatrolNET.TelegramBot;

namespace CrawlPatrolNET
{
    class Program
    {
        private static PawPatrolCrawler Crawler;
        private static Bot Bot;

        private static EpisodeStore EpisodeStore;
        private static Timer Timer;
        private static double TimerInterval = 30 * 60 * 1_000;

        static void Main(string[] args)
        {
            PrepareCrawler();
            PrepareBot();

            StartBot();

            Timer = new Timer(TimerInterval);
            Timer.Elapsed += OnCrawl;
            Timer.AutoReset = true;
            Timer.Start();

            Crawler.Crawl();

            Console.ReadKey();
        }

        private static void OnCrawl(object _sender, ElapsedEventArgs _e)
        {
            Crawler?.Crawl();
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
