using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Home
{
    public class ManagerModel : PageModel
    {
        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "man")
                return RedirectToPage("/Login");

            return Page();
        }
    }
}
