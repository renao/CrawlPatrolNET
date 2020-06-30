using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CrawlPatrolNET.Crawler
{
    public class PawPatrolCrawler
    {
        private const string VideosListUrl = "http://www.nickjr.de/paw-patrol/videos/";

        private readonly EpisodeStore EpisodeStore;

        public PawPatrolCrawler(EpisodeStore store)
        {
            this.EpisodeStore = store;
        }

        public void Crawl()
        {
            var currentEpisodes = this.FullEpisodes();
            this.EpisodeStore.UpdateCurrentEpisodes(currentEpisodes);
        }

        public List<Episode> FullEpisodes()
        {
            var web = new HtmlWeb();
            var doc = web.Load(VideosListUrl);

            var episodeNodes = this.FullEpisodeNodes(doc);
            var episodes
                = episodeNodes?
                .Select(this.ReadEpisodeInfo)
                .ToList();
            return episodes;
        }

        private List<HtmlNode> FullEpisodeNodes(HtmlDocument html)
        {
            var docNode = html.DocumentNode;
            var videoItems = docNode.SelectNodes("//div[contains(concat(' ', normalize-space(@class), ' '), 'stream-block-item')]");
            var episodeItems = videoItems
                .Where(v => v.SelectNodes("./a/div/div/div[@class=\"stream-label stream-tile-flag tile-episode-flag\"]") != null)
                .ToList();

            return episodeItems;
        }

        private Episode ReadEpisodeInfo(HtmlNode node)
        {
            var titleNode = node.SelectSingleNode("./div/b[@class=\"tooltip-title\"]");
            var descriptionNode = node.SelectSingleNode("./div/p[@class=\"tooltip-description\"]");
            var imageUrlNode = node.SelectSingleNode("./a/div/picture/img[@class=\"main-content-image\"]");
            var routeNode = node.SelectSingleNode("./a[@class=\"route\"]");
            return new Episode
            {
                Title = titleNode?.InnerText,
                Description = descriptionNode?.InnerText,
                Image = imageUrlNode?.GetAttributeValue("srcset", ""),
                URL = $"http://nickjr.de{routeNode.GetAttributeValue("href", "")}"
            };
        }
    }
}
