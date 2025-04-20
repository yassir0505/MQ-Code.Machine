using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class TeamHistoryModel : PageModel
    {
        private readonly MQContext _context;

        public TeamHistoryModel(MQContext context)
        {
            _context = context;
        }

        public List<Maintenance> MaintenanceList { get; set; } = new();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "tl")
                return RedirectToPage("/Login");

            var company = _context.Users
                .Where(u => u.Username == username)
                .Select(u => u.CompanyName)
                .FirstOrDefault();

            var teamUsernames = _context.Users
                .Where(u => u.CompanyName == company)
                .Select(u => u.Username)
                .ToList();

            MaintenanceList = _context.Maintenances
                .Where(m => teamUsernames.Contains(m.PerformedBy))
                .OrderByDescending(m => m.Date)
                .ToList();

            return Page();
        }
    }
}
