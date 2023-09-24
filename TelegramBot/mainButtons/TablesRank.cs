using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.mainButtons
{

    internal class TablesRank
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly NpgsqlConnection _connection;

        public TablesRank(ITelegramBotClient botClient, Chat chat, NpgsqlConnection connection)
        {
            _botClient = botClient;
            _chat = chat;
            _connection = connection;
        }

        public async Task HandleTablesRank()
        {
            var inlineKeyboardTablesRank = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Для женщин", callbackData: "1"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Для мужчин", callbackData: "2"),
                }
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "Выберите пол:",
                      replyMarkup: inlineKeyboardTablesRank);
        }

        internal async Task OnAnswer(Update update)
        {
            _connection.Open();
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT path_table FROM rank WHERE id_rank = {update.CallbackQuery.Data}", _connection);
                    var pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendPhotoAsync(
                            chatId: _chat.Id,
                            photo: new InputFileStream(fileStream),
                            caption: "Таблица разрядов для женщин"
                        );
                    }
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");


                    break;
                case "2":
                    npgSqlCommand = new NpgsqlCommand($"SELECT path_table FROM rank WHERE id_rank = {update.CallbackQuery.Data}", _connection);
                    pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendPhotoAsync(
                            chatId: _chat.Id,
                            photo: new InputFileStream(fileStream),
                            caption: "Таблица разрядов для мужчин"
                        );
                    }
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                    break;
            }
            _connection.Close();
        }
    }
}
