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

namespace TelegramBot
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
            _connection.Open();
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT info FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    var info = npgSqlCommand.ExecuteScalar();
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            (string)info);
                    npgSqlCommand = new NpgsqlCommand($"SELECT path_photo FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    var pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendVideoAsync(
                            chatId: _chat.Id,
                            video: new InputFileStream(fileStream),
                            caption: "Техника плавания вольным стилем"
                        );
                    }

                    break;
                case "2":
                    npgSqlCommand = new NpgsqlCommand($"SELECT info FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    info = npgSqlCommand.ExecuteScalar();
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            (string)info);
                    npgSqlCommand = new NpgsqlCommand($"SELECT path_photo FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendVideoAsync(
                            chatId: _chat.Id,
                            video: new InputFileStream(fileStream),
                            caption: "Техника плавания на спине"
                        );
                    }

                    break;
                case "3":
                    npgSqlCommand = new NpgsqlCommand($"SELECT info FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    info = npgSqlCommand.ExecuteScalar();
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            (string)info);
                    npgSqlCommand = new NpgsqlCommand($"SELECT path_photo FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendVideoAsync(
                            chatId: _chat.Id,
                            video: new InputFileStream(fileStream),
                            caption: "Техника плавания брассом"
                        );
                    }

                    break;
                case "4":
                    npgSqlCommand = new NpgsqlCommand($"SELECT info FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    info = npgSqlCommand.ExecuteScalar();
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "");
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            (string)info);
                    npgSqlCommand = new NpgsqlCommand($"SELECT path_photo FROM styles WHERE id = {update.CallbackQuery.Data}", _connection);
                    pathPhoto = npgSqlCommand.ExecuteScalar();
                    using (var fileStream = new FileStream((string)pathPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botClient.SendVideoAsync(
                            chatId: _chat.Id,
                            video: new InputFileStream(fileStream),
                            caption: "Техника плавания баттерфляем"
                        );
                    }

                    break;
            }
            _connection.Close();
        }
    }
}
