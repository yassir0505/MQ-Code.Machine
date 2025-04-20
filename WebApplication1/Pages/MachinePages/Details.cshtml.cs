using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.MachinePages
{
    public class DetailsModel : PageModel
    {
        private readonly MQContext _context;

        public DetailsModel(MQContext context)
        {
            _context = context;
        }

        public Machine? Machine { get; set; }

        public void OnGet(int id)
        {
            Machine = _context.Machines.FirstOrDefault(m => m.Id == id);
        }

        public IActionResult OnPostStartRepair(int machineId)
        {
            var machine = _context.Machines.FirstOrDefault(m => m.Id == machineId);
            if (machine != null)
            {
                machine.RepairStatus = RepairStatus.BeingRepaired;
                _context.SaveChanges();
            }
            return RedirectToPage(new { id = machineId });
        }

        public IActionResult OnPostFinishRepair(int machineId, string result, string report)
        {
            var machine = _context.Machines.FirstOrDefault(m => m.Id == machineId);
            if (machine != null)
            {
                machine.RepairStatus = result == "repaired" ? RepairStatus.None : RepairStatus.CannotBeRepaired;
                machine.RepairReport = report;
                _context.SaveChanges();
            }
            return RedirectToPage(new { id = machineId });
        }

        public IActionResult OnPostCancelRepair(int machineId)
        {
            var machine = _context.Machines.FirstOrDefault(m => m.Id == machineId);
            if (machine != null && machine.RepairStatus == RepairStatus.BeingRepaired)
            {
                machine.RepairStatus = RepairStatus.ScanError;
                _context.SaveChanges();
            }
            return RedirectToPage(new { id = machineId });
        }
    }
}
