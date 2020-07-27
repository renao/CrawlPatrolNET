using System;
namespace CrawlPatrolNET.DB
{
    public class Subscriber
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }

        public Subscriber()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
