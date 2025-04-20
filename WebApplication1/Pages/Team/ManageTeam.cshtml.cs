using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Team
{
    public class ManageTeamModel : PageModel
    {
        private readonly MQContext _context;

        public ManageTeamModel(MQContext context)
        {
            _context = context;
        }

        public List<User> TeamMembers { get; set; } = new();

        [BindProperty]
        public string NewUsername { get; set; } = "";
        public string TeamName { get; set; } = "";

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "tl")
                return RedirectToPage("/Login");

            TeamName = username + "-team";
            TeamMembers = _context.Users
                .Where(u => u.TeamName == TeamName && u.Role == "tech")
                .ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            var tlUsername = HttpContext.Session.GetString("Username");
            var tech = _context.Users.FirstOrDefault(u => u.Username == NewUsername && u.Role == "tech");

            if (tech != null)
            {
                tech.TeamName = tlUsername + "-team";
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRemove(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.TeamName = null;
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
