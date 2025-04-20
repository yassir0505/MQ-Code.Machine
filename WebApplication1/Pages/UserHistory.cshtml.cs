using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class UserHistoryModel : PageModel
    {
        private readonly MQContext _context;

        public UserHistoryModel(MQContext context)
        {
            _context = context;
        }

        public List<Maintenance> MaintenanceList { get; set; } = new();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            MaintenanceList = _context.Maintenances
                .Where(m => m.PerformedBy == username)
                .OrderByDescending(m => m.Date)
                .ToList();

            return Page();
        }
    }
}
