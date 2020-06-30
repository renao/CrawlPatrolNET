using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CrawlPatrolNET.Crawler
{
    public class EpisodeStore
    {
        public event Action<List<Episode>> NewEpisodes;

        private List<Episode> CurrentEpisodes = new List<Episode>();


        public void UpdateCurrentEpisodes(List<Episode> episodes)
        {
            var storedTitles
                = this.CurrentEpisodes
                .Select(e => e.Title)
                .ToList();
            var newEpisodes
                = episodes
                .Where(e => !storedTitles.Contains(e.Title))
                .ToList();

            this.CurrentEpisodes = episodes;

            if (newEpisodes.Count > 0)
            {
                this.NewEpisodes?.Invoke(newEpisodes);
            }

        }
    }
}
