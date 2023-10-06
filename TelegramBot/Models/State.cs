using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public struct State
    {
        public int Id_user { get; set; }
        public int state { get; set; }
        public string Name { get; set; }
    }
}
