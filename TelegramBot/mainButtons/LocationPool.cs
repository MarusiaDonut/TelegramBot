using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

namespace TelegramBot.mainButtons
{
    internal class LocationPool
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly NpgsqlConnection _connection;
        private readonly Message _message;


        public LocationPool(ITelegramBotClient botClient, Chat chat, NpgsqlConnection connection, Message message)
        {
            _botClient = botClient;
            _chat = chat;
            _connection = connection;
            _message = message;
        }

        public async Task HandleLocationPool()
        {
            var list = new List<List<KeyboardButton>>();
            list.Add(new List<KeyboardButton>() { KeyboardButton.WithRequestLocation("Отправить геолокацию") });

            var keyboard = new ReplyKeyboardMarkup(list);
            keyboard.ResizeKeyboard = true;

            await _botClient.SendTextMessageAsync(_chat.Id, "Отправьте геолокацию:", replyMarkup: keyboard);

        }

    }
}
