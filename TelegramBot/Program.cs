using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SqlTypes;
using Npgsql;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using TelegramBot.telegram;
using System.Collections.Generic;
using TelegramBot.Models;

namespace TelegramBot
{
    class Program
    {
        private static void Main()
        {
            var conn = new NpgsqlConnection(Config.SqlConnectionString);
            var bot = new TelegramBotClient(Config.TelegramBotToken);
            TelegramBotService botService = new(bot);
            
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
               botService.HandleUpdateAsync,
               botService.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
