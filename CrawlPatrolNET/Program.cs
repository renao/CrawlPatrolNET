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
            if (args.Length < 1)
            {
                Console.WriteLine("Fehler - Bitte geben Sie Ihren Telegram-Bot AccessToken an");
                return;
            }

            PrepareCrawler();
            PrepareBot(args[0]);

            StartBot();
            StartTimer();

            Crawler.Crawl();

            Console.ReadKey();
        }

        private static void PrepareCrawler()
        {
            EpisodeStore = new EpisodeStore();
            Crawler = new PawPatrolCrawler(EpisodeStore);
        }

        private static void PrepareBot(string accessToken)
        {
            Bot = new Bot(accessToken);
        }

        private static void StartTimer()
        {
            Timer = new Timer(TimerInterval);
            Timer.Elapsed += (_s, _e) => Crawler?.Crawl();
            Timer.AutoReset = true;
            Timer.Start();

            Crawler?.Crawl();
        }

        private static void StartBot()
        {
            Bot.ListenTo(EpisodeStore);
        }
    }
}
