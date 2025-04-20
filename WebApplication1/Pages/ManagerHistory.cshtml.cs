using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class ManagerHistoryModel : PageModel
    {
        private readonly MQContext _context;

        public ManagerHistoryModel(MQContext context)
        {
            _context = context;
        }

        public List<Maintenance> MaintenanceHistory { get; set; } = new();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "man")
                return RedirectToPage("/Login");

            var approvedMachines = _context.Machines
                .Where(m => m.Status == "Approved" && m.CreatedBy == username)
                .Select(m => m.Id)
                .ToList();

            MaintenanceHistory = _context.Maintenances
                .Where(m => approvedMachines.Contains(m.MachineId))
                .OrderByDescending(m => m.Date)
                .ToList();

            return Page();
        }
    }
}
