using System;

namespace WebApplication1.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderUsername { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
