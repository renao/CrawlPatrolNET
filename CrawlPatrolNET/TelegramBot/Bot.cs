using System;
using System.Collections.Generic;
using CrawlPatrolNET.Crawler;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using CrawlPatrolNET.DataStore;

namespace CrawlPatrolNET.TelegramBot
{
    public class Bot
    {

        // private List<long> SubscriberChatIds = new List<long>() { };
        private EpisodeStore EpisodeStore;
        private ITelegramBotClient BotClient;
        private BotSubscribersStore SubscribersStore;


        public Bot(string accessToken)
        {
            this.SubscribersStore = new BotSubscribersStore();
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
            foreach (var chatId in this.SubscribersStore.Subscribers)
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

            if (string.IsNullOrEmpty(episode?.Image))
            {
                this.BotClient
                    .SendTextMessageAsync(chatId, episodeCaption)
                    .Wait();
            }
            else
            {
                this.BotClient
                    .SendPhotoAsync(
                        chatId,
                        new Telegram.Bot.Types.InputFiles.InputOnlineFile(episode.Image),
                        episodeCaption)
                    .Wait();
            }
        }

        private void OnMessage(object _sender, MessageEventArgs e)
        {
            switch (e.Message.Text)
            {
                case "/start":
                    this.Subscribe(e.Message.Chat.Id);
                    this.SendStartAnswer(e.Message);
                    break;
                case "/stop":
                    this.Unsubscribe(e.Message.Chat.Id);
                    this.SendStopAnswer(e.Message);
                    break;
                case "/current":
                    this.SendCurrentEpisodes(e.Message.Chat.Id);
                    break;
            }
        }

        private void SendStartAnswer(Message message)
        {
            this.BotClient
                .SendTextMessageAsync(message.Chat.Id,
                $"Willkommen {message.Chat.FirstName}!" +
                $"{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Wir benachrichtigen dich, sobald es neue frei-verfügbare Episoden gibt." +
                $"{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Um die aktuell verfügbaren Folgen anzuzeigen gebe /current ein.");
        }

        private void SendStopAnswer(Message message)
        {
            this.BotClient
                .SendTextMessageAsync(message.Chat.Id,
                $"Na gut!" +
                $"{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Wir lassen dich mit automatischen Antworten ab sofort in Ruhe.");
        }

        private void Subscribe(long chatId)
        {
            this.SubscribersStore.Add(chatId);
        }

        private void Unsubscribe(long chatId)
        {
            this.SubscribersStore.Remove(chatId);
        }
    }
}
