using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Maintenance
    {
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public string Description { get; set; } = "";

        public DateTime Date { get; set; }

        public string? PerformedBy { get; set; }

        // ✅ Link to Machine
        public Machine Machine { get; set; } = null!;
    }
}
