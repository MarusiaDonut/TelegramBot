using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Npgsql;
using TelegramBot.mainButtons;
using Dapper;
using TelegramBot.Models;

namespace TelegramBot.telegram
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient _botClient;

        private static StyleOfSwimming? _styleOfSwimming;
        private static TablesRank? _tablesRank;
        private static WorkoutRecording? _workoutRecording;
        private static Records? _records;
        private static LocationPool? _locationPool;

        private Models.User _user;

        private string _workout = "";
        public TelegramBotService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var message = update.Message;
                switch (update.Type)
                {
                    case UpdateType.Message:

                        _user = GetUserById(message);
                        if (_user.Id != message.From.Id)
                        {
                            InsertUser(message);
                        }

                        if (message.Text != null)
                        {
                            if (message.Text.StartsWith("/"))
                            {
                                await HandleCommandsSleshAsync(message.Chat.Id, message.Text, message, cancellationToken);
                            }
                            else
                            {
                                await HandleCommandsAsync(message.Text, message, cancellationToken);
                            }
                        }

                        if (message.Location != null)
                        {
                            string latitude = message.Location.Latitude.ToString().Replace(",", ".");
                            string longitude = message.Location.Longitude.ToString().Replace(",", ".");
                            _locationPool = new LocationPool(_botClient, message.Chat);
                            var namePool = _locationPool.nearestPool(latitude, longitude);
                            await _locationPool.RemoveRequestContactButton(namePool);
                        }
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
                        if (_records != null)
                        {
                            await _records.OnAnswer(update);
                            _records = null;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        private async Task HandleCommandsSleshAsync(long chatId, string command, Message message, CancellationToken cancellationToken)
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
                    new KeyboardButton("‍Найти ближайщий бассейн 📍"),
                },
                new[]
                {
                    new KeyboardButton("Дневник тренировок 📖")
                }
            });
            keyboard.ResizeKeyboard = true;

            switch (command)
            {
                case "/start":
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Добро пожаловать в чат-бот \"Все о плавании\", {message.From.FirstName}!\n" +
                        $"Вам также доступны команды:\n" +
                        $"/start - начало работы с ботом\n" +
                        $"/info - дополнительная информация\n",
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken
                    );
                    break;
                case "/info":
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Более подробную информацию о плавании можно найти на официальном сайте Всероссийская Федерация Плавания - https://russwimming.ru/",
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }

        private async Task HandleCommandsAsync(string command, Message message, CancellationToken cancellationToken)
        {
            switch (command)
            {
                case "Рекорды в мире плавания 🏆":
                    _records = new Records(_botClient, message.Chat);
                    await _records.HandleRecords();
                    break;

                case "Таблица разрядов‍ 📄":
                    _tablesRank = new TablesRank(_botClient, message.Chat);
                    await _tablesRank.HandleTablesRank();
                    break;

                case "Стили плавания 🏊":
                    _styleOfSwimming = new StyleOfSwimming(_botClient, message.Chat);
                    await _styleOfSwimming.HandleStylesOfSwimming();
                    break;

                case "‍Найти ближайщий бассейн 📍":
                    _locationPool = new LocationPool(_botClient, message.Chat);
                    await _locationPool.HandleLocationPool();
                    break;

                case "Дневник тренировок 📖":
                    _workout = "Дневник тренировок";
                    _workoutRecording = new WorkoutRecording(_botClient, message.Chat, message);
                    await _workoutRecording.HandleWorkoutRecording();
                    break;
                default:
                    if (message.Text != null && _workout == "Дневник тренировок")
                    {
                        _workoutRecording = new WorkoutRecording(_botClient, message.Chat, message);
                        var idState = GetStateUser(message);
                        if (idState == 1)
                        {
                            _workoutRecording.SendTextWorkout(message.Text.ToLower());
                            await _botClient.SendTextMessageAsync(chatId: message.Chat, text: "Вы ввели тренировку."); 
                        }
                        if (idState == 2)
                        {
                            await _workoutRecording.GetTextWorkout(message.Text.ToLower());
                        }
                        _workoutRecording = null;
                    }
                    break;
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private Models.User GetUserById(Message message)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT id FROM users WHERE id = {message.From.Id}";
                var idUser = conn.QueryFirstOrDefault<Models.User>(sql, new { id = message.From.Id });
                return idUser;
            }
        }

        private void InsertUser(Message message)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"INSERT INTO users (id, name) VALUES('{message.From.Id}', '{message.From.FirstName}')";
                conn.Execute(sql, new { id = message.From.Id, name = message.From.FirstName });
            }
        }

        private int GetStateUser(Message message)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"SELECT state FROM users WHERE id = {message.From.Id}";
                var idState = conn.QueryFirstOrDefault<Models.User>(sql, new { Id = message.From.Id });
                return idState.State;
            }
        }
    }
}
