using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot;
using System.Data.SqlTypes;
using Npgsql;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace TelegramBot
{
    class Program
    {
        private static void Main()
        {
            String connectionString = "Server=localhost;Port=5432;UserName=postgres;Password=123;Database=postgres;";
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionString);

            var bot = new TelegramBotClient("6584101748:AAEcp4nYF0pPN1X3MgpkfnPDGjAmAjrmTH0");
            TelegramBotService botService = new(bot, npgSqlConnection);
            
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
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
