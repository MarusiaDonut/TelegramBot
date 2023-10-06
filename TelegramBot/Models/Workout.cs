using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public struct Workout
    {
        public int Id_user { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
