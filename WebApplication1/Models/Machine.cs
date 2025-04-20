namespace WebApplication1.Models
{
    public enum RepairStatus
    {
        None,
        BeingRepaired,
        CannotBeRepaired,
        ScanError
    }

    public class Machine
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public int Year { get; set; }
        public string Type { get; set; } = "";
        public string Company { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public string RequestedBy { get; set; } = ""; // ✅ مهم جداً
        public string Status { get; set; } = "Pending"; // Pending or Approved
        public RepairStatus RepairStatus { get; set; } = RepairStatus.None;
        public string? RepairReport { get; set; }
        public bool HideFromManagerView { get; set; } = false;
        

    }
}
