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
            //_connection.Open();
            switch (update.CallbackQuery.Data)
            {
                case "1":
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            "Введите разминку одним сообщением.");

                    //await GetText(update);
                    await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Вы ввели разминку. Введите основное задание одним сообщением");
                    //await _botClient.SendTextMessageAsync(_chat.Id,
                    //        "Вы ввели разминку. Введите основное задание одним сообщением");
                    break;
                case "2":
                    break;
            }


            //_connection.Close();
        }

        internal async Task GetText(string message)
        {
            await Console.Out.WriteLineAsync(message);
        }

    }
}

