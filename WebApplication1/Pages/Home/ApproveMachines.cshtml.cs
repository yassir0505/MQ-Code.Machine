using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

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

                string qrContent = $"https://localhost:7260/Machine/Details?id={machine.Id}";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qr");
                Directory.CreateDirectory(folderPath);

                // Generate QR Code
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                using (MemoryStream stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    byte[] qrBytes = stream.ToArray();

                    string pdfPath = Path.Combine(folderPath, $"machine_{machine.Id}.pdf");
                    using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, fs);
                        doc.Open();

                        // Logo
                        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logo = iTextSharp.text.Image.GetInstance(logoPath);
                            logo.ScaleToFit(80f, 80f);
                            logo.Alignment = Element.ALIGN_CENTER;
                            doc.Add(logo);
                        }

                        // Title 
                        var titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 18f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        var title = new Paragraph("Machine QR Code\n\n", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        doc.Add(title);

                        // Machine ID
                        var idFont = iTextSharp.text.FontFactory.GetFont("Arial", 14f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        var id = new Paragraph($"Machine ID: {machine.Id}\n\n", idFont)
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        doc.Add(id);

                        // QR Code
                        var qrImg = iTextSharp.text.Image.GetInstance(qrBytes);
                        qrImg.ScaleToFit(300f, 300f);
                        qrImg.Alignment = Element.ALIGN_CENTER;
                        doc.Add(qrImg);

                        doc.Close();
                    }
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
