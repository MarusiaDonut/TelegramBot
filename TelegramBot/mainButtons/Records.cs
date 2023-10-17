using Npgsql;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Dapper;
using TelegramBot.Models;

namespace TelegramBot.mainButtons
{
    internal class Records
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;

        public Records(ITelegramBotClient botClient, Chat chat)
        {
            _botClient = botClient;
            _chat = chat;
        }

        public async Task HandleRecords()
        {
            var inlineKeyboardRecords = new InlineKeyboardMarkup(new[]
              {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "50м в/c", callbackData: "1"),
                    InlineKeyboardButton.WithCallbackData(text: "100м в/c", callbackData: "2"),
                    InlineKeyboardButton.WithCallbackData(text: "200м в/c", callbackData: "3"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "400м в/c", callbackData: "4"),
                    InlineKeyboardButton.WithCallbackData(text: "800м в/c", callbackData: "5"),
                    InlineKeyboardButton.WithCallbackData(text: "1500м в/c", callbackData: "6"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "50м н/c", callbackData: "7"),
                    InlineKeyboardButton.WithCallbackData(text: "100м н/c", callbackData: "8"),
                    InlineKeyboardButton.WithCallbackData(text: "200м н/c", callbackData: "9"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "50м брасс", callbackData: "10"),
                    InlineKeyboardButton.WithCallbackData(text: "100м брасс", callbackData: "11"),
                    InlineKeyboardButton.WithCallbackData(text: "200м брасс", callbackData: "12"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "50м батт", callbackData: "13"),
                    InlineKeyboardButton.WithCallbackData(text: "100м батт", callbackData: "14"),
                    InlineKeyboardButton.WithCallbackData(text: "200м батт", callbackData: "15"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "200м кп", callbackData: "16"),
                    InlineKeyboardButton.WithCallbackData(text: "400м кп", callbackData: "17"),
                },
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "в/с - вольный стиль, н/c - на спине, кп - комплексное плавание. \nВыберите дистанцию:",
                      replyMarkup: inlineKeyboardRecords);
        }

        internal async Task OnAnswer(Update update)
        {
            var records = "";
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    records = GetRecords(50, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "2":
                    records = GetRecords(100, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "3":
                    records = GetRecords(200, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "4":
                    records = GetRecords(400, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "5":
                    records = GetRecords(800, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "6":
                    records = GetRecords(1500, $"в/с");
                    await SendTextAsync(records);
                    break;
                case "7":
                    records = GetRecords(50, $"н/с");
                    await SendTextAsync(records);
                    break;
                case "8":
                    records = GetRecords(100, $"н/с");
                    await SendTextAsync(records);
                    break;
                case "9":
                    records = GetRecords(200, $"н/с");
                    await SendTextAsync(records);
                    break;
                case "10":
                    records = GetRecords(50, $"брасс");
                    await SendTextAsync(records);
                    break;
                case "11":
                    records = GetRecords(100, $"брасс");
                    await SendTextAsync(records);
                    break;
                case "12":
                    records = GetRecords(200, $"брасс");
                    await SendTextAsync(records);
                    break;
                case "13":
                    records = GetRecords(50, $"батт");
                    await SendTextAsync(records);
                    break;
                case "14":
                    records = GetRecords(100, $"батт");
                    await SendTextAsync(records);
                    break;
                case "15":
                    records = GetRecords(200, $"батт");
                    await SendTextAsync(records);
                    break;
                case "16":
                    records = GetRecords(200, $"кп");
                    await SendTextAsync(records);
                    break;
                case "17":
                    records = GetRecords(400, $"кп");
                    await SendTextAsync(records);
                    break;
            }
        }

        private async Task SendTextAsync(string records)
        {
            await _botClient.SendTextMessageAsync(
                    chatId: _chat.Id,
                    text: records);
        }

        private string GetRecords(int distance, string style)
        {
            var allRecords = "";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT championship, male, time, place, name FROM records WHERE distance = {distance} and style = '{style}'";
                var records = conn.Query<Record>(sql, new { Distance = distance, Style = style }).ToList();
                records.ForEach(record => allRecords += $"❗ {record.Championship}, Пол: {record.Male}, Имя: {record.Name}, Время: {record.Time}, Место: {record.Place}. \n");
                return allRecords;
            }
        }
    }
}
