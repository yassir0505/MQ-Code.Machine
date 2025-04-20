using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using WebApplication1.Data;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly MQContext _context;

        public RegisterModel(MQContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string FirstName { get; set; } = "";

            [Required]
            public string LastName { get; set; } = "";

            [Required]
            public string Role { get; set; } = "";

            public string Username { get; set; } = "";

            [Required]
            [MinLength(9)]
            public string Password { get; set; } = "";

            public string? Email { get; set; }

            public string? CompanyName { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Generate base username
            string baseUsername = $"{Input.FirstName.ToLower()}.{Input.LastName.ToLower()}.{Input.Role}";
            string username = baseUsername;
            int counter = 1;

            while (_context.Users.Any(u => u.Username == username))
            {
                username = baseUsername + counter;
                counter++;
            }

            // 2. Hash password
            string hashedPassword = HashPassword(Input.Password);

            // 3. Create user object
            var user = new User
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Role = Input.Role,
                Username = username,
                Password = hashedPassword,
                Email = Input.Email,
                CompanyName = Input.CompanyName
            };

            // 4. Save to DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 5. Redirect or success message
            return RedirectToPage("Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
