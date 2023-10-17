using Npgsql;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Dapper;
using TelegramBot.Models;

namespace TelegramBot.mainButtons
{
    internal class WorkoutRecording
    {
        private ITelegramBotClient _botClient;
        private Chat _chat;
        private readonly Message _message;

        public WorkoutRecording(ITelegramBotClient botClient, Chat chat,  Message message)
        {
            _botClient = botClient;
            _chat = chat;
            _message = message;
        }

        public async Task HandleWorkoutRecording()
        {
            var inlineKeyboardWorkoutRecording = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Записать тренировку", callbackData: "send"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Посмотреть запись тренировки", callbackData: "check"),
                }
            });

            await _botClient.SendTextMessageAsync(
                      chatId: _chat.Id,
                      text: "Выберите действие:",
                      replyMarkup: inlineKeyboardWorkoutRecording);
        }

        internal async Task OnAnswer(Update update)
        {
            switch (update.CallbackQuery.Data)
            {
                case "send":
                    await _botClient.SendTextMessageAsync(_chat.Id,
                            "Введите тренировку.");
                    UpdateStateByIdUser(1);
                    break;
                case "check":
                    await _botClient.SendTextMessageAsync(_chat.Id,
                           "Введите дату тренировки в формате ДД.ММ.ГГГГ.");
                    UpdateStateByIdUser(2);
                    break;
            }
        }

        internal void SendTextWorkout(string message)
        {
            var dateTimeNow = DateTime.Now;
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"INSERT INTO workout (id_user, text, date) VALUES('{_message.From.Id}', '{message}', '{dateTimeNow}')";
                conn.Execute(sql, new { Id_User = _message.From.Id, Text = message, Date = dateTimeNow });
            }
        }

        internal async Task GetTextWorkout(string date)
        {
            var text = GetText(date);
            await _botClient.SendTextMessageAsync(_chat.Id,
                            text);
        }

        private string GetText(string date)
        {
            try
            {
                using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
                {
                    var alltext = "";
                    string sql = $"SELECT text FROM workout WHERE id_user = '{_message.From.Id}' and  date = '{date}'";
                    var texts = conn.Query<Workout>(sql, new { Id_User = _message.From.Id, Date = date }).ToList();
                    if (texts.Count != 0)
                    {
                        texts.ForEach(text => alltext += text.Text + "\n \n");
                        return alltext;
                    }
                    else
                    {
                        return $"Вы не записывали тренировку {date}";
                    }
                }
            }
            catch
            {
                return "Вы ввели дату в неверном формате. Повторите попытку ввода формата в виде ДД.ММ.ГГГГ";
            }
        }

        private void UpdateStateByIdUser(int state)
        {
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                string sql = $"Update users set state = '{state}', state_name = 'Запись тренировки' where id = '{_message.From.Id}'";
                conn.QueryFirstOrDefault<Models.User>(sql, new { Id = _message.From.Id, State = state });
            }
        }
    }
}

