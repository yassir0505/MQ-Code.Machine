using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System.IO;

namespace WebApplication1.Pages.Home
{
    public class ApproveMachinesModel : PageModel
    {
        private readonly MQContext _context;

        public ApproveMachinesModel(MQContext context)
        {
            _context = context;
        }

        public List<Machine> PendingMachines { get; set; } = new();
        public List<Machine> MyCreatedMachines { get; set; } = new();

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetString("Role") != "man")
            {
                Response.Redirect("/Login");
                return;
            }

            PendingMachines = _context.Machines
                .Where(m => (m.Status == "Pending" || m.Status == "Approved") && !m.HideFromManagerView)
                .ToList();

            MyCreatedMachines = _context.Machines
                .Where(m => m.CreatedBy == username)
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(int machineId, string action)
        {
            var machine = _context.Machines.FirstOrDefault(m => m.Id == machineId);
            if (machine == null)
                return RedirectToPage();

            if (action == "approve")
            {
                machine.Status = "Approved";

                string qrContent = $"https://mq-machine.onrender.com/Machine/Details?id={machine.Id}";
                byte[] qrBytes;

                // ✅ Generate QR Code as byte[] PNG
                using (QRCodeGenerator qrGenerator = new())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
                using (PngByteQRCode qrCode = new(qrCodeData))
                {
                    qrBytes = qrCode.GetGraphic(20);
                }

                // ✅ Generate PDF in memory
                using (MemoryStream pdfStream = new())
                {
                    Document doc = new(PageSize.A4);
                    PdfWriter.GetInstance(doc, pdfStream);
                    doc.Open();

                    // ✅ Add logo if exists
                    string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                    if (System.IO.File.Exists(logoPath))
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(80f, 80f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(logo);
                    }

                    // ✅ Add title
                    var titleFont = FontFactory.GetFont("Arial", 18f, Font.BOLD, BaseColor.BLACK);
                    var title = new Paragraph("Machine QR Code\n\n", titleFont) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(title);

                    // ✅ Add Machine ID
                    var idFont = FontFactory.GetFont("Arial", 14f, Font.BOLD, BaseColor.BLACK);
                    var id = new Paragraph($"Machine ID: {machine.Id}\n\n", idFont) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(id);

                    // ✅ Add QR Code
                    var qrImg = iTextSharp.text.Image.GetInstance(qrBytes);
                    qrImg.ScaleToFit(300f, 300f);
                    qrImg.Alignment = Element.ALIGN_CENTER;
                    doc.Add(qrImg);

                    doc.Close();

                    byte[] pdfBytes = pdfStream.ToArray();
                    return File(pdfBytes, "application/pdf", $"machine_{machine.Id}.pdf");
                }
            }
            else if (action == "reject")
            {
                _context.Machines.Remove(machine);
            }
            else if (action == "hide")
            {
                machine.HideFromManagerView = true;
            }
            else if (action == "delete")
            {
                _context.Machines.Remove(machine);
                TempData["KeepTableOpen"] = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
