using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.IO;
using Npgsql;

namespace TelegramBot.mainButtons
{
    internal class StyleOfSwimming
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly NpgsqlConnection _connection;

        public StyleOfSwimming(ITelegramBotClient botClient, Chat chat, NpgsqlConnection connection)
        {
            _botClient = botClient;
            _chat = chat;
            _connection = connection;
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
            _connection.Close();
        }

        private async Task Style(string dataId, string caption)
        {
            _connection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT info FROM styles WHERE id_style = {dataId}", _connection);
            var info = npgSqlCommand.ExecuteScalar();

            await _botClient.SendTextMessageAsync(_chat.Id,
                           (string)info);
            npgSqlCommand = new NpgsqlCommand($"SELECT path_video FROM styles WHERE id_style = {dataId}", _connection);
            var pathPhoto = npgSqlCommand.ExecuteScalar();
            using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await _botClient.SendVideoAsync(
                    chatId: _chat.Id,
                    video: new InputFileStream(fileStream),
                    caption: caption
                );
                fileStream.Close();
            }
            _connection.Close();
        }
    }
}
