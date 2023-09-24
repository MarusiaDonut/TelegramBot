using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Microsoft.Data.Sqlite;
using Npgsql;
using Npgsql.Internal;
using System.Xml.Linq;
using System;
using TelegramBot.mainButtons;

namespace TelegramBot.telegram
{
    public class TelegramBotService
    {

        private readonly ITelegramBotClient _botClient;
        private readonly NpgsqlConnection _connection;

        private static StyleOfSwimming? _styleOfSwimming;
        private static TablesRank? _tablesRank;
        private static WorkoutRecording? _workoutRecording;

        public TelegramBotService(ITelegramBotClient botClient, NpgsqlConnection connection)
        {
            _botClient = botClient;
            _connection = connection;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var message = update.Message;


                switch (update.Type)
                {

                    case UpdateType.Message:
                        _connection.Open();
                        Console.WriteLine("Соединение с БД открыто");

                        NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT id FROM users WHERE id = {message.From.Id}", _connection);
                        var idUser = npgSqlCommand.ExecuteScalar();

                        if (idUser == null)
                        {
                            npgSqlCommand = new NpgsqlCommand($"INSERT INTO users (id, name) VALUES('{message.From.Id}', '{message.From.FirstName}')", _connection);
                            npgSqlCommand.ExecuteNonQuery();
                        }

                        if (message.Text.StartsWith("/"))
                        {
                            await HandleCommandsSlesh(message.Chat.Id, message.Text, message, cancellationToken);
                        }
                        else
                        {
                            await HandleCommands(message.Chat.Id, message.Text, message, cancellationToken);
                        }
                        _connection.Close();
                        break;

                    case UpdateType.CallbackQuery:

                        if (_styleOfSwimming != null)
                        {
                            await _styleOfSwimming.OnAnswer(update);
                            _styleOfSwimming = null;
                        }
                        if (_tablesRank != null)
                        {
                            await _tablesRank.OnAnswer(update);
                            _tablesRank = null;
                        }
                        if (_workoutRecording != null)
                        {
                            await _workoutRecording.OnAnswer(update);
                            _workoutRecording = null;
                        }
                        else
                        {
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        private async Task HandleCommandsSlesh(long chatId, string command, Message message, CancellationToken cancellationToken)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Рекорды в мире плавания 🏆"),
                    new KeyboardButton("Таблица разрядов‍ 📄"),

                },
                new[]
                {
                    new KeyboardButton("Стили плавания 🏊"),
                    new KeyboardButton("‍Полезные статьи о плавании ❗"),
                },
                new[]
                {
                    new KeyboardButton("Дневник тренировок 📖")
                }
            });
            switch (command)
            {
                case "/start":
                case "Вернуться к":
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Добро пожаловать в чат-бот \"Все о плавании\", {message.From.FirstName}!\n" +
                        $"Вам также доступны команды:\n" +
                        $"/start - начало работы с ботом\n" +
                        $"/help - список доступных команд\n",
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken
                    );
                    break;
                case "/help":
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Доступные команды:\n" +
                              "/start - начало работы с ботом\n" +
                              "/help - список доступных команд\n",
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }

        private async Task HandleCommands(long chatId, string command, Message message, CancellationToken cancellationToken)
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

            switch (command)
            {
                case "Рекорды в мире плавания 🏆":
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Выберите интересующий чемпионат:",
                        replyMarkup: inlineKeyboardRecords,
                        cancellationToken: cancellationToken);
                    break;
                case "Таблица разрядов‍ 📄":
                    _tablesRank = new TablesRank(_botClient, message.Chat, _connection);
                    await _tablesRank.HandleTablesRank();
                    break;
                case "Стили плавания 🏊":
                    _styleOfSwimming = new StyleOfSwimming(_botClient, message.Chat, _connection);
                    await _styleOfSwimming.HandleStylesOfSwimming();
                    break;
                case "Полезные статьи о плавании ❗":
                    await _botClient.SendLocationAsync(
                        chatId: chatId,
                        latitude: message.Location.Latitude,
                        longitude: message.Location.Longitude,
                        cancellationToken: cancellationToken
                    );
                    break;
                case "Дневник тренировок 📖":
                    _workoutRecording = new WorkoutRecording(_botClient, message.Chat, _connection, message);
                    await _workoutRecording.HandleWorkoutRecording();
                    break;
                default:
                    if (message.Text != null)
                    {
                        _workoutRecording = new WorkoutRecording(_botClient, message.Chat, _connection, message);
                        string text = message.Text.ToLower();
                        await _workoutRecording.GetText(text);
                    }
                    break;
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}
