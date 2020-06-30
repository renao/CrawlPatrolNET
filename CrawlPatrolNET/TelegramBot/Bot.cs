using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using CrawlPatrolNET.Crawler;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Linq;

namespace CrawlPatrolNET.TelegramBot
{
    public class Bot
    {

        private List<long> ChatIds = new List<long>() { 85700835L };
        private EpisodeStore EpisodeStore;
        private ITelegramBotClient BotClient;

        public Bot()
        {
            this.BotClient = new TelegramBotClient("1268728321:AAG_55XXdSBlqW9tO0AST3LrhKHZ6Euc1xI");
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
            var message = $"<strong>Neues verfügbar!</strong>";
            foreach (var chatId in this.ChatIds)
            {
                this.BotClient.SendTextMessageAsync(
                    chatId,
                    message,
                    Telegram.Bot.Types.Enums.ParseMode.Html)
                    .Wait();
                foreach (var episode in episodes)
                {
                    var episodeCaption = $"{episode.Title} {System.Environment.NewLine} {System.Environment.NewLine} {episode.Description} {System.Environment.NewLine} {System.Environment.NewLine}  {episode.URL}";
                    this.BotClient.SendPhotoAsync(chatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(episode.Image), episodeCaption).Wait();
                
                }
            }
        }

        private void OnMessage(object _sender, MessageEventArgs e)
        {
            if (e.Message.Text == "/start")
            {
                this.SaveChatId(e.Message.Chat.Id);
            }
        }

        private void SaveChatId(long chatId)
        {
            this.ChatIds.Add(chatId);
        }
    }
}
