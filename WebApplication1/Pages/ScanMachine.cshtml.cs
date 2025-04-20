using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class ScanMachineModel : PageModel
    {
        private readonly MQContext _context;

        public ScanMachineModel(MQContext context)
        {
            _context = context;
        }

        public Machine? Machine { get; set; }
        public List<Maintenance> MaintenanceList { get; set; } = new();
        public List<User> TechnicianInfo { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
                return RedirectToPage("/Login");

            Machine = _context.Machines.FirstOrDefault(m => m.Id == id);
            if (Machine == null)
                return RedirectToPage("/Home/Home");

            MaintenanceList = _context.Maintenances
                .Where(m => m.MachineId == id)
                .OrderByDescending(m => m.Date)
                .ToList();

            var usernames = MaintenanceList.Select(m => m.PerformedBy).Distinct().ToList();
            TechnicianInfo = _context.Users
                .Where(u => usernames.Contains(u.Username))
                .ToList();

            return Page();
        }
    }
}
