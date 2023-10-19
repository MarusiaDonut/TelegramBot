namespace TelegramBot.Models
{
    public struct Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
