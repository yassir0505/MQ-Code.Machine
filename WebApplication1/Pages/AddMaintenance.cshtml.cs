using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class AddMaintenanceModel : PageModel
    {
        private readonly MQContext _context;

        public AddMaintenanceModel(MQContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int MachineId { get; set; }

        [BindProperty]
        public string? RepairReport { get; set; }

        public string? Message { get; set; }
        public Machine? Machine { get; set; }

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !(role == "tech" || role == "tl" || role == "man"))
                return RedirectToPage("/Login");

            return Page();
        }

        public IActionResult OnPost(string action)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !(role == "tech" || role == "tl" || role == "man"))
                return RedirectToPage("/Login");

            Machine = _context.Machines.FirstOrDefault(m => m.Id == MachineId && m.Status == "Approved");
            if (Machine == null)
            {
                Message = "❌ Machine not found or not approved.";
                return Page();
            }

            Machine.RepairReport = RepairReport;

            switch (action)
            {
                case "BeingRepaired":
                    Machine.RepairStatus = RepairStatus.BeingRepaired;
                    break;

                case "CannotBeRepaired":
                    Machine.RepairStatus = RepairStatus.CannotBeRepaired;
                    break;

                case "ScanError":
                    Machine.RepairStatus = RepairStatus.ScanError;
                    break;
            }

            _context.SaveChanges();
            Message = $"✅ Status updated to '{Machine.RepairStatus}'";

            return Page();
        }
    }
}
