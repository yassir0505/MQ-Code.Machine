using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Home
{
    public class TeamLeaderModel : PageModel
    {
        private readonly MQContext _context;

        public TeamLeaderModel(MQContext context)
        {
            _context = context;
        }

        public string FullName { get; set; } = "";

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToPage("/Login");

            FullName = $"{user.FirstName} {user.LastName}";
            return Page();
        }
    }
}

