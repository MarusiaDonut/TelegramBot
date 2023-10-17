using Npgsql;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Dapper;

namespace TelegramBot.mainButtons
{
    internal class TablesRank
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;

        private Models.Table _table;

        public TablesRank(ITelegramBotClient botClient, Chat chat)
        {
            _botClient = botClient;
            _chat = chat;
        }

        public async Task HandleTablesRank()
        {
            var inlineKeyboardTablesRank = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Для женщин", callbackData: "женщины"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Для мужчин", callbackData: "мужчины"),
                }
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "Выберите пол:",
                      replyMarkup: inlineKeyboardTablesRank);
        }

        internal async Task OnAnswer(Update update)
        {
            switch (update.CallbackQuery.Data)
            {
                case "женщины":
                    _table.Male = update.CallbackQuery.Data;
                    await Table(_table.Male, "Таблица разрядов для женщин");
                    break;
                case "мужчины":
                    _table.Male = update.CallbackQuery.Data;
                    await Table(_table.Male, "Таблица разрядов для мужчин");
                    break;
            }
        }

        private async Task Table(string male, string caption)
        {
            var tablePath = GetTableById(male);
            using (var fileStream = new FileStream(tablePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await _botClient.SendPhotoAsync(
                    chatId: _chat.Id,
                    photo: new InputFileStream(fileStream),
                    caption: caption
                );
                fileStream.Close();
            }
        }

        private string GetTableById(string male)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT path_table FROM rank WHERE male = '{male}'";
                var pathPhoto = conn.QueryFirstOrDefault<Models.Table>(sql, new { Male = male });
                return pathPhoto.Path_table;
            }
        }
    }
}
