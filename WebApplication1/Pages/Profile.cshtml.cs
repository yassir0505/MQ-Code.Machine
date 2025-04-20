using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly MQContext _context;

        public ProfileModel(MQContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? Message { get; set; }

        public class InputModel
        {
            public string Username { get; set; } = "";
            public string FullName { get; set; } = "";  // ✅ جديد
            public string Email { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string CompanyName { get; set; } = "";
        }



        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToPage("/Login");

            Input = new InputModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyName = user.CompanyName
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToPage("/Login");

            user.FullName = Input.FullName;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;
            user.CompanyName = Input.CompanyName;

            _context.SaveChanges();

            Message = "✅ Profile updated successfully!";
            return Page();
        }

        public IActionResult OnPostLeaveTeam()
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                user.TeamName = null;
                _context.SaveChanges();
                HttpContext.Session.SetString("TeamName", ""); 
                Message = "🚪 You have left the team.";
            }

            return RedirectToPage();
        }
    }
}
