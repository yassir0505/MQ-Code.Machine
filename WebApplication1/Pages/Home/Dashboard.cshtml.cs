using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Home
{
    public class DashboardModel : PageModel
    {
        private readonly MQContext _context;

        public DashboardModel(MQContext context)
        {
            _context = context;
        }

        public int TotalMachines { get; set; }
        public int WorkingCount { get; set; }
        public int RepairingCount { get; set; }
        public int NotRepairableCount { get; set; }

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");

            var machines = _context.Machines
                .Where(m =>
                    m.CreatedBy == username &&
                    (m.Status == "Approved" || !m.HideFromManagerView)
                )
                .ToList();

            TotalMachines = machines.Count;
            WorkingCount = machines.Count(m => m.RepairStatus == RepairStatus.None);
            RepairingCount = machines.Count(m => m.RepairStatus == RepairStatus.BeingRepaired);
            NotRepairableCount = machines.Count(m => m.RepairStatus == RepairStatus.CannotBeRepaired);
        }
    }
}
