﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Data;
using WebApplication1.Models;
using QRCoder;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace WebApplication1.Pages
{
    public class AddMachineModel : PageModel
    {
        private readonly MQContext _context;

        public AddMachineModel(MQContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? Message { get; set; }

        public class InputModel
        {
            public string Name { get; set; } = "";
            public string SerialNumber { get; set; } = "";
            public string Type { get; set; } = "";
            public int Year { get; set; }
            public string Company { get; set; } = "";
        }

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !(role == "tech" || role == "tl" || role == "man"))
                return RedirectToPage("/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Input.Company))
            {
                ModelState.AddModelError("Input.Company", "Company is required.");
                return Page();
            }

            var machine = new Machine
            {
                Name = Input.Name,
                SerialNumber = Input.SerialNumber,
                Type = Input.Type,
                Year = Input.Year,
                Company = Input.Company,
                CreatedBy = username,
                RequestedBy = username,
                Status = (role == "man") ? "Approved" : "Pending"
            };

            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();

            if (role == "man")
            {
                string qrContent = $"https://mq-machine.onrender.com/Machine/Details?id={machine.Id}";
                byte[] qrBytes;

                // ✅ Use PngByteQRCode for compatibility with Linux containers (Render)
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    qrBytes = qrCode.GetGraphic(20);
                }

                using (MemoryStream pdfStream = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4);
                    PdfWriter.GetInstance(doc, pdfStream);
                    doc.Open();

                    string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                    if (System.IO.File.Exists(logoPath))
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(80f, 80f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(logo);
                    }

                    var titleFont = FontFactory.GetFont("Arial", 18f, Font.BOLD, BaseColor.BLACK);
                    var title = new Paragraph("Machine QR Code\n\n", titleFont) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(title);

                    var idFont = FontFactory.GetFont("Arial", 14f, Font.BOLD, BaseColor.BLACK);
                    var id = new Paragraph($"Machine ID: {machine.Id}\n\n", idFont) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(id);

                    var qrImg = iTextSharp.text.Image.GetInstance(qrBytes);
                    qrImg.ScaleToFit(300f, 300f);
                    qrImg.Alignment = Element.ALIGN_CENTER;
                    doc.Add(qrImg);

                    doc.Close();

                    byte[] pdfBytes = pdfStream.ToArray();
                    return File(pdfBytes, "application/pdf", $"machine_{machine.Id}.pdf");
                }
            }
            else
            {
                Message = "✅ Machine request submitted successfully!";
            }

            ModelState.Clear();
            return Page();
        }
    }
}
