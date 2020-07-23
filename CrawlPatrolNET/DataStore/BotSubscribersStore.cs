using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlPatrolNET.DataStore
{
    public class BotSubscribersStore
    {

        private const string SubscribersFileName = "subscribers.store";
        public List<long> Subscribers { get; set; }

        public BotSubscribersStore()
        {
            PrepareSubscribers();
        }

        private void PrepareSubscribers()
        {
            if (File.Exists(SubscribersFileName))
            {
                var lines = File.ReadAllLines(SubscribersFileName);
                this.Subscribers = lines.Select(l => long.Parse(l)).ToList();
            }
            else
            {
                File.Create(SubscribersFileName);
                this.Subscribers = new List<long>();
            }
        }

        public void Add(long subscriberId)
        {
            if (!this.Subscribers.Contains(subscriberId))
            {
                this.Subscribers.Add(subscriberId);
                this.RewriteSubscribersFile();
            }
        }

        public void Remove(long subscriberId)
        {
            if (this.Subscribers.Contains(subscriberId))
            {
                this.Subscribers.Remove(subscriberId);
                this.RewriteSubscribersFile();
            }
        }

        private void RewriteSubscribersFile()
        {
            File.WriteAllLines(
                    SubscribersFileName,
                    this.Subscribers.Select(l => l.ToString()).ToList());
        }
    }
}
