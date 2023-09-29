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
    internal class WorkoutRecording
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly NpgsqlConnection _connection;
        private readonly Message _message;

        public WorkoutRecording(ITelegramBotClient botClient, Chat chat, NpgsqlConnection connection, Message message)
        {
            _botClient = botClient;
            _chat = chat;
            _connection = connection;
            _message = message;
        }

        public async Task HandleWorkoutRecording()
        {
            var inlineKeyboardWorkoutRecording = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Записать тренировку", callbackData: "1"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Посмотреть запись тренировки", callbackData: "2"),
                }
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "Выберите действие:",
                      replyMarkup: inlineKeyboardWorkoutRecording);
        }

        internal async Task OnAnswer(Update update)
        {
            _connection.Open();
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            "Введите тренировку одним сообщением.");
                    break;
                case "2":
                    await _botClient.SendTextMessageAsync(_chat.Id,
                           "Введите дату тренировки в формате ДД месяц");
                    break;
            }
            _connection.Close();
        }

        internal async Task GetTextWorkout(string message)
        {
            //await Console.Out.WriteLineAsync(message);
            var date = DateTime.Now;
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"INSERT INTO workout (id_user, text, date) VALUES('{_message.From.Id}', '{message}', '{date:M}')", _connection);
            npgSqlCommand.ExecuteNonQuery();
        }

        internal async Task GetDateWorkout(string date)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT text FROM workout WHERE id_user = {_message.From.Id} and date = {date}", _connection);
            var text = npgSqlCommand.ExecuteScalar();
            await _botClient.SendTextMessageAsync(_chat.Id,
                            (string)text);
            //await Console.Out.WriteLineAsync((string?)text);
        }
        

    }
}

