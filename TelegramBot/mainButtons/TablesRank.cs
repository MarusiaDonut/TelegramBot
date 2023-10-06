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
using Dapper;
using System.Numerics;
using TelegramBot.Models;

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
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    _table.Id = update.CallbackQuery.Data;
                    await Table(_table.Id, "Таблица разрядов для женщин");
                    break;
                case "2":
                    _table.Id = update.CallbackQuery.Data;
                    await Table(_table.Id, "Таблица разрядов для мужчин");
                    break;
            }
        }

        private async Task Table(string idState, string caption)
        {
            var tablePath = GetTableById(idState);
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

        private string GetTableById(string id)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT path_table FROM rank WHERE id = {id}";
                var pathPhoto = conn.QueryFirstOrDefault<Models.Table>(sql, new { id });
                return pathPhoto.Path_table;
            }
        }
    }
}
