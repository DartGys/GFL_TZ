using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GFL_TZ.Models;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace GFL_TZ.Controllers
{
    public class CatalogiesController : Controller
    {
        private readonly GflDbContext _context;

        public CatalogiesController(GflDbContext context)
        {
            _context = context;
        }

        // GET: Catalogies
        public async Task<IActionResult> Index(int? index = 1)
        {
            var all = await _context.Catalogies.ToListAsync();
            var models = await _context.Catalogies.Where(x => x.ForeignKey == index).ToListAsync();
            var folder = await _context.Catalogies.FirstAsync(x => x.Id == index);

            ViewBag.Folder = folder.Name;

            return View(models);
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {

                var catalogies = _context.Catalogies.ToList();

                var worksheet = workbook.Worksheets.Add("Catalogies");

                int row = 2;
                int col = 1;

                foreach (var catalogy in catalogies)
                {
                    worksheet.Cell(row, col).Value = catalogy.Id;
                    worksheet.Cell(row, ++col).Value = catalogy.Name;
                    worksheet.Cell(row, ++col).Value = catalogy.ForeignKey;
                    col = 1;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"catalogies.xlsx"
                    };
                }
            }
        }

        public ActionResult Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var context = new GflDbContext()) // Замініть на ваш контекст бази даних
                    {
                        context.Catalogies.RemoveRange(context.Catalogies); // Видалити всі записи з таблиці
                        context.SaveChanges();
                    }

                    using (var stream = file.OpenReadStream())
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        var workbook = package.Workbook;
                        if (workbook != null)
                        {
                            var worksheet = workbook.Worksheets.FirstOrDefault(); // Ви можете вказати ім'я аркуша, якщо воно відмінне від першого
                            if (worksheet != null)
                            {
                                using (var context = new GflDbContext()) // Замініть на ваш контекст бази даних
                                {
                                    int startRow = 2; // Початковий рядок, де знаходяться дані
                                    int rowCount = worksheet.Dimension.Rows;

                                    for (int row = startRow; row <= rowCount; row++)
                                    {
                                        var catalogy = new Catalogy
                                        {
                                            Id = worksheet.Cells[row, 1].GetValue<int>(),
                                            Name = worksheet.Cells[row, 2].GetValue<string>(),
                                            ForeignKey = worksheet.Cells[row, 3].GetValue<int>()
                                        };

                                        context.Catalogies.Add(catalogy);
                                    }

                                    context.SaveChanges();
                                }
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Дані імпортовано успішно.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Помилка імпорту: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Оберіть файл для імпорту.";
            }

            return RedirectToAction("Index");
        }

    }
}

