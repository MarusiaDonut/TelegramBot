using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Npgsql;
using Dapper;

namespace TelegramBot.mainButtons
{
    internal class StyleOfSwimming
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;

        public StyleOfSwimming(ITelegramBotClient botClient, Chat chat)
        {
            _botClient = botClient;
            _chat = chat;
        }
        public async Task HandleStylesOfSwimming()
        {
            var inlineKeyboardStyleOfSwimming = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Вольный стиль", callbackData: "Вольный"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "На спине", callbackData: "На спине"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Брасс", callbackData: "Брасс"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Баттерфляй", callbackData: "Баттерфляй")
                }

            });

            await _botClient.SendTextMessageAsync(
                       chatId: _chat.Id,
                       text: "Выберите стиль плавания:",
                       replyMarkup: inlineKeyboardStyleOfSwimming);
        }

        internal async Task OnAnswer(Update update)
        {
            switch (update.CallbackQuery.Data)
            {
                case "Вольный":
                    await Style(update.CallbackQuery.Data, "Техника плавания вольным стилем");
                    break;
                case "На спине":
                    await Style(update.CallbackQuery.Data, "Техника плавания на спине");
                    break;
                case "Брасс":
                    await Style(update.CallbackQuery.Data, "Техника плавания брассом");
                    break;
                case "Баттерфляй":
                    await Style(update.CallbackQuery.Data, "Техника плавания баттерфляем");
                    break;
            }
        }

        private async Task Style(string nameStyle, string caption)
        {
            var info = GetInfoById(nameStyle);

            await _botClient.SendTextMessageAsync(_chat.Id,
                            info);
            var pathVideo = GetPathVideoById(nameStyle);
            using (var fileStream = new FileStream(pathVideo, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await _botClient.SendVideoAsync(
                    chatId: _chat.Id,
                    video: new InputFileStream(fileStream),
                    caption: caption
                );
                fileStream.Close();
            }
        }

        private string GetInfoById(string nameStyle)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT info FROM styles WHERE name = '{nameStyle}'";
                var info = conn.QueryFirstOrDefault<Models.Style>(sql, new { Name = nameStyle });
                return info.Info;
            }
        }

        private string GetPathVideoById(string nameStyle)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT path_video FROM styles WHERE name = '{nameStyle}'";
                var info = conn.QueryFirstOrDefault<Models.Style>(sql, new { Name = nameStyle });
                return info.Path_video;
            }
        }
    }
}
