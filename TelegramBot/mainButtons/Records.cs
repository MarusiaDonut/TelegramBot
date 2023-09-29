using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.mainButtons
{
    internal class Records
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly NpgsqlConnection _connection;

        public Records(ITelegramBotClient botClient, Chat chat, NpgsqlConnection connection)
        {
            _botClient = botClient;
            _chat = chat;
            _connection = connection;
        }

        public async Task HandleRecords()
        {
            var inlineKeyboardRecords = new InlineKeyboardMarkup(new[]
             {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Чемпионат России", callbackData: "1"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Чемпионат Европы", callbackData: "2"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Чемпионат мира", callbackData: "3"),
                }
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "Выберите интересующий чемпионат:",
                      replyMarkup: inlineKeyboardRecords);
        }
    }
}
