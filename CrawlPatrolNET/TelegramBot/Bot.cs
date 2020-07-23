using System;
using System.Collections.Generic;
using CrawlPatrolNET.Crawler;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Linq;

namespace CrawlPatrolNET.TelegramBot
{
    public class Bot
    {

        private List<long> SubscriberChatIds = new List<long>() { };
        private EpisodeStore EpisodeStore;
        private ITelegramBotClient BotClient;

        public Bot(string accessToken)
        {
            this.BotClient = new TelegramBotClient(accessToken);
            BotClient.OnMessage += this.OnMessage;
            BotClient.StartReceiving();
        }

        public void ListenTo(EpisodeStore store)
        {
            this.EpisodeStore = store;
            this.EpisodeStore.NewEpisodes += OnNewEpisodes;
        }

        private void OnNewEpisodes(List<Episode> episodes)
        {
            var message = $"<strong>Neue Folge verfügbar!</strong>";
            foreach (var chatId in this.SubscriberChatIds)
            {
                this.BotClient.SendTextMessageAsync(
                    chatId,
                    message,
                    Telegram.Bot.Types.Enums.ParseMode.Html)
                    .Wait();
                foreach (var episode in episodes)
                {
                    this.SendEpisodeMessage(chatId, episode);
                }
            }
        }

        private void SendCurrentEpisodes(long chatId)
        {
            foreach (var episode in this.EpisodeStore.CurrentEpisodes)
            {
                this.SendEpisodeMessage(chatId, episode);
            }
        }

        private void SendEpisodeMessage(long chatId, Episode episode)
        {
            var episodeCaption = $"{episode.Title} {Environment.NewLine} {Environment.NewLine} {episode.Description} {Environment.NewLine} {Environment.NewLine}  {episode.URL}";
            this.BotClient
                .SendPhotoAsync(
                chatId,
                new Telegram.Bot.Types.InputFiles.InputOnlineFile(episode.Image),
                episodeCaption).Wait();

        }

        private void OnMessage(object _sender, MessageEventArgs e)
        {
            switch (e.Message.Text)
            {
                case "/start":
                    this.Subscribe(e.Message.Chat.Id);
                    break;
                case "/stop":
                    this.Unsubscribe(e.Message.Chat.Id);
                    break;
                case "/current2":
                    this.SendCurrentEpisodes(e.Message.Chat.Id);
                    break;
            }
        }

        private void Subscribe(long chatId)
        {
            if (!this.SubscriberChatIds.Contains(chatId))
            {
                this.SubscriberChatIds.Add(chatId);
            }
        }

        private void Unsubscribe(long chatId)
        {
            if (this.SubscriberChatIds.Contains(chatId))
            {
                this.SubscriberChatIds.RemoveAll(l => l == chatId);
            }
        }
    }
}
