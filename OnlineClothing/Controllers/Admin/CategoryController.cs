using Microsoft.AspNetCore.Mvc;
using OnlineClothing.Models;

namespace OnlineClothing.Controllers.Admin
{
    public class CategoryController : Controller
    {
        private readonly ClothingShopPrn222G2Context _context;
        public CategoryController()
        {
            _context = new ClothingShopPrn222G2Context();
        }

        public IActionResult Manage()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return PartialView("Create", category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return PartialView("Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return PartialView("Edit", category);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return PartialView("DeleteConfirm", category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }
    }
}
