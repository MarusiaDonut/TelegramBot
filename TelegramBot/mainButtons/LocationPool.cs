using Npgsql;
using System;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Dapper;

namespace TelegramBot.mainButtons
{
    internal class LocationPool
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;

        public LocationPool(ITelegramBotClient botClient, Chat chat)
        {
            _botClient = botClient;
            _chat = chat;
        }

        public async Task HandleLocationPool()
        {
            var list = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>
                {
                    KeyboardButton.WithRequestLocation("Отправить геолокацию")
                }
            };
            var keyboard = new ReplyKeyboardMarkup(list);
            keyboard.ResizeKeyboard = true;
            await _botClient.SendTextMessageAsync(_chat.Id, "Отправьте геолокацию:", replyMarkup: keyboard);
        }

        internal string nearestPool(string latitude, string longitude)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"select name, adress, site, phone from location_pool where ST_Distance(ST_Transform(location, 26986), " +
                                $"ST_Transform(ST_SetSRID(ST_MakePoint({latitude}, {longitude}), 4326), 26986)) > 3500;";
                var location = conn.QueryFirstOrDefault<Models.Location>(sql, new { latitude, longitude });
               return  location.Name;
            }
        }

        public async Task RemoveRequestContactButton(string namePool)
        {
            await _botClient.SendTextMessageAsync(_chat.Id, $"Самый ближайщий к вам бассейн - {namePool}", replyMarkup: new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Рекорды в мире плавания 🏆"),
                    new KeyboardButton("Таблица разрядов‍ 📄"),

                },
                new[]
                {
                    new KeyboardButton("Стили плавания 🏊"),
                    new KeyboardButton("‍Найти ближайщий бассейн ❗"),
                },
                new[]
                {
                    new KeyboardButton("Дневник тренировок 📖")
                }
            })).ConfigureAwait(false);
        }

    }
}
