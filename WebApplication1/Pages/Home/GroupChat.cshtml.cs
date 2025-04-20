using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Home
{
    public class GroupChatModel : PageModel
    {
        private readonly MQContext _context;

        public GroupChatModel(MQContext context)
        {
            _context = context;
        }

        public List<ChatMessage> Messages { get; set; } = new();

        [BindProperty]
        public string NewMessage { get; set; } = "";

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || (role != "tech" && role != "tl"))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || string.IsNullOrEmpty(user.TeamName))
                return RedirectToPage("/Home/Home");

            // ترتيب من الأقدم إلى الأحدث (WhatsApp style)
            Messages = _context.ChatMessages
                .Where(m => m.TeamName == user.TeamName)
                .OrderBy(m => m.Timestamp)
                .ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || string.IsNullOrEmpty(user.TeamName))
            {
                TempData["Error"] = "⛔ You are not in a team.";
                return RedirectToPage("/Home/Home");
            }

            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                var msg = new ChatMessage
                {
                    SenderUsername = username,
                    TeamName = user.TeamName,
                    Content = NewMessage.Trim(),
                    Timestamp = DateTime.Now
                };

                _context.ChatMessages.Add(msg);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
