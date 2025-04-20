using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly MQContext _context;

        public LoginModel(MQContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // Hash the entered password
            string hashedPassword = HashPassword(Input.Password);

            // Check user in DB
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == Input.Username && u.Password == hashedPassword);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }
            if (user.Role == "tl" && string.IsNullOrEmpty(user.TeamName))
            {
                user.TeamName = user.Username + "-team";
                _context.SaveChanges();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("TeamName", user.TeamName ?? ""); 

            // Redirect based on role
            return RedirectToPage("/Home/Home");

        }

        private string GetRedirectPage(string role)
        {
            return role switch
            {
                "tech" => "Tech",
                "tl" => "TeamLeader",
                "man" => "Manager",
                _ => "Index"
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
