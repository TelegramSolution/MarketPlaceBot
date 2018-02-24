using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyTelegramBot.Controllers
{
    public class CategoryController : Controller
    {
        MarketBotDbContext db;

        Category Category;
        public IActionResult Index()
        {
            db = new MarketBotDbContext();

            return View(db.Category.ToList());
        }

        [HttpGet]
        public IActionResult Creator()
        {
            db = new MarketBotDbContext();
            Category = new Category
            {
                Name = String.Empty,
                Enable = false
            };
            return View("Editor", Category);
        }

        public IActionResult Editor(int id)
        {
            db = new MarketBotDbContext();

            if (id > 0)
            {
                var category = db.Category.Find(id);

                if (category != null)
                    return View(category);

                else
                    return NoContent();
            }

            else
                return NotFound();
        }

        public IActionResult Save (Category SaveCategory)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if(SaveCategory!=null && SaveCategory.Id > 0)
            {
                Category = db.Category.Where(c => c.Id == SaveCategory.Id).FirstOrDefault();

                Category.Enable = SaveCategory.Enable;

                if (SaveCategory.Name != null && SaveCategory.Name != Category.Name && CheckName(SaveCategory.Name))
                    Category.Name = SaveCategory.Name;

                db.SaveChanges();

            }

            if(SaveCategory!=null && SaveCategory.Id == 0 && SaveCategory.Name!=null)
                InsertCategory(SaveCategory);
            
            return RedirectToAction("Index");
        }

        public Category InsertCategory(Category category)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (category != null && category.Name != null)
            {                  
                db.Category.Add(category);
                db.SaveChanges();
                return category;
            }

            else
                return null;
        }

        public bool CheckName(string name)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (db.Category.Where(c => c.Name == name).FirstOrDefault() != null)
                return false;

            else
                return true;
        }
    }
}