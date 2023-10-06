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
                    InlineKeyboardButton.WithCallbackData(text: "Вольный стиль", callbackData: "1"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "На спине", callbackData: "2"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Брасс", callbackData: "3"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Баттерфляй", callbackData: "4")
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
                case "1":
                    await Style(update.CallbackQuery.Data, "Техника плавания вольным стилем");
                    break;
                case "2":
                    await Style(update.CallbackQuery.Data, "Техника плавания на спине");
                    break;
                case "3":
                    await Style(update.CallbackQuery.Data, "Техника плавания брассом");
                    break;
                case "4":
                    await Style(update.CallbackQuery.Data, "Техника плавания баттерфляем");
                    break;
            }
        }

        private async Task Style(string dataId, string caption)
        {
            var info = GetInfoById(dataId);

            await _botClient.SendTextMessageAsync(_chat.Id,
                            info);
            var pathVideo = GetPathVideoById(dataId);
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

        private string GetInfoById(string id)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT info FROM styles WHERE id_style = {id}";
                var info = conn.QueryFirstOrDefault<Models.Style>(sql, new { id });
                return info.Info;
            }
        }

        private string GetPathVideoById(string id)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT path_video FROM styles WHERE id_style = {id}";
                var info = conn.QueryFirstOrDefault<Models.Style>(sql, new { id });
                return info.Path_video;
            }
        }
    }
}
