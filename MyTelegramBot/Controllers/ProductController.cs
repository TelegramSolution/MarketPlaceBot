using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting; // для IHostingEnvironment

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class ProductController : Controller
    {
        MarketBotDbContext db;

        Product Product { get; set; }

        private readonly IHostingEnvironment _appEnvironment;

        public ProductController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            db = new MarketBotDbContext();
            var products = db.Product.Include(p => p.Category).ToList();
           
            foreach(Product p in products)
                p.ProductPrice = db.ProductPrice.Where(pr => pr.ProductId == p.Id && pr.Enabled == true).Include(pr=>pr.Currency).OrderByDescending(pr => pr.Id).ToList();            

            return View(products);
        }

        [HttpGet]
        public IActionResult Creator()
        {

            db = new MarketBotDbContext();

            var conf = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).Include(b=>b.Configuration).FirstOrDefault();

            var catlist = db.Category.ToList();

            Product product = new Product();
            product.Id = 0;
            product.Name = String.Empty;
            product.CategoryId = 0;
            product.UnitId = 1;
            product.TelegraphUrl = String.Empty;
            product.Text = String.Empty;
            product.PhotoUrl = String.Empty;
            product.ProductPrice.Add(new ProductPrice { CurrencyId = conf.Configuration.CurrencyId, Value = 0 });

            if(catlist.Count>0)
            ViewBag.Category = new SelectList(catlist, "Id", "Name", db.Category.FirstOrDefault().Id);

            else
                ViewBag.Category = new SelectList(catlist, "Id", "Name", 0);


            ViewBag.Currency = db.Currency.ToList();
            ViewBag.Unit = new SelectList(db.Units.ToList(), "Id", "Name", product.UnitId);
            return View("Editor", product);
        }

        [HttpGet]
        public IActionResult Photos(int id)
        {
            db = new MarketBotDbContext();

            Product=db.Product.Where(p => p.Id == id).Include(p => p.ProductPhoto).FirstOrDefault();

            List<AttachmentFs> list = new List<AttachmentFs>();

            foreach(ProductPhoto pp in Product.ProductPhoto)
                if(!pp.MainPhoto)
                    list.Add(db.AttachmentFs.Find(pp.AttachmentFsId));

            ViewBag.ProductId = id;

            return View(list);
        }

        [HttpGet]

        public IActionResult DeletePhoto(int Id)
        {
            db = new MarketBotDbContext();

            if (Id > 0)
            {
                var pp = db.ProductPhoto.Where(p => p.AttachmentFsId == Id).FirstOrDefault();

                db.ProductPhoto.Remove(pp);

                db.SaveChanges();

                return Ok();
            }

            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPhoto(int ProductId, IList<IFormFile> image)
        {
            if(ProductId>0 && image!=null && image.Count > 0)
            {

                foreach (IFormFile file in image)
                    AddAttachment(file, ProductId, false);


                RedirectResult redirectResult = new RedirectResult("Photos\\" + ProductId);

                return redirectResult;
            }

            else
            {
                RedirectResult redirectResult = new RedirectResult("Photos\\" + ProductId);

                return redirectResult;
            }
        }


        [HttpGet]
        public IActionResult ImportFaq()
        {
            return View();
        }

        [HttpGet]

        public FileStreamResult Template()
        {
            string path = Path.Combine(_appEnvironment.ContentRootPath, "Files/Шаблон.csv");
            FileStream fs = new FileStream(path, FileMode.Open);
            string file_type = "text/plain";
            string file_name = "Шаблон.csv";
            return File(fs, file_type, file_name);
      
        }

        [HttpGet]
        public IActionResult Example()
        {
            string path = Path.Combine(_appEnvironment.ContentRootPath, "Files/Пример.csv");
            FileStream fs = new FileStream(path, FileMode.Open);
            string file_type = "text/plain";
            string file_name = "Пример.csv";
            return File(fs, file_type, file_name);
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile import)
        {
            if (import != null)
            {
                ImportCSV importCSV = new ImportCSV();
                await importCSV.ImportToDb(import.OpenReadStream());
                return RedirectToAction("Index");
            }

            else
                return RedirectToAction("ImportFaq");
        }

        [HttpPost]
        public IActionResult SaveCaption ([FromBody] AttachmentFs attachmentFs)
        {
            db = new MarketBotDbContext();

            if (attachmentFs != null && attachmentFs.Id > 0)
            {
                var att = db.AttachmentFs.Find(attachmentFs.Id);
                att.Caption = attachmentFs.Caption;
                db.SaveChanges();
                return Json("Сохранено");
            }


            else
               return Json("Ошибка");

            
        }

        [HttpGet]
        public IActionResult Editor (int id)
        {

            if (id > 0)
            {
                db = new MarketBotDbContext();
                var product = db.Product.Where(p => p.Id == id).Include(p => p.Unit).Include(p=>p.Category).FirstOrDefault();

                product.ProductPhoto.Add(db.ProductPhoto.Where(photo => photo.ProductId == product.Id && photo.MainPhoto).
                    OrderByDescending(photo=>photo.AttachmentFsId).Include(photo=>photo.AttachmentFs).FirstOrDefault());

                product.ProductPrice.Add(db.ProductPrice.Where(price => price.ProductId == product.Id && price.Enabled == true).OrderByDescending(price => price.Id).FirstOrDefault());
                if (product.ProductPhoto.FirstOrDefault() != null)
                {
                    string imageBase64Data = Convert.ToBase64String(product.ProductPhoto.FirstOrDefault().AttachmentFs.Fs);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    ViewBag.ImageData = imageDataURL;
                }

                ViewBag.Category = new SelectList(db.Category.Where(c=>c.Enable).ToList(), "Id", "Name",product.CategoryId);
                ViewBag.Currency = db.Currency.ToList();
                ViewBag.Unit = new SelectList(db.Units.ToList(), "Id", "Name",product.UnitId);

                if (product != null)
                    return View(product);

                else
                    return NoContent();
            }



            else
                return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save (Product SaveProduct, IFormFile image = null)
        {
            db = new MarketBotDbContext();

            bool Check = true;

            var conf = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).Include(b => b.Configuration).FirstOrDefault();

            SaveProduct.ProductPrice.FirstOrDefault().CurrencyId = conf.Configuration.CurrencyId;

            if (SaveProduct!=null)
                Check = CheckName(SaveProduct.Name);

            if (SaveProduct != null && SaveProduct.Id > 0)
            {
                Product = db.Product.Where(p => p.Id == SaveProduct.Id).FirstOrDefault(); // находим товар в бд
                Product.ProductPrice = db.ProductPrice.Where(pr => pr.ProductId == Product.Id && pr.Enabled == true).
                    Include(pr => pr.Currency).OrderByDescending(pr => pr.Id).ToList();
            }

            if (Product!=null && Product.Name != SaveProduct.Name && Check == false || Product==null && Check==false)
                return Json("Товар с таким названием уже существует");

            //Редактируется уже сущестуюий товар. Перед этим проверятся изменилось ли имя, если изменилось,
            //то мы проверям не занято ли оно
            if (SaveProduct != null && SaveProduct.Id>0 && SaveProduct.Name!=null && Product != null && Product.Name==SaveProduct.Name ||
                SaveProduct != null && SaveProduct.Id > 0 && SaveProduct.Name != null && Product != null 
                && Product.Name != SaveProduct.Name && Check)
            {
                Product.Name = SaveProduct.Name;
                Product.CategoryId = SaveProduct.CategoryId;
                Product.TelegraphUrl = SaveProduct.TelegraphUrl;
                Product.Enable = SaveProduct.Enable;
                Product.PhotoUrl = SaveProduct.PhotoUrl;
                Product.Text = SaveProduct.Text;
                Product.UnitId = SaveProduct.UnitId;

                // Проверям изменил ли пользователь цену .  Если изменил то добавляем новую запись в БД
                if (SaveProduct.ProductPrice.FirstOrDefault().Value != Product.ProductPrice.FirstOrDefault().Value 
                    && SaveProduct.ProductPrice.FirstOrDefault().Value >0)
                {
                    SaveProduct.ProductPrice.FirstOrDefault().ProductId = Product.ProductPrice.FirstOrDefault().ProductId;
                    ProductPriceInsert(SaveProduct.ProductPrice.FirstOrDefault());
                    DisablePrice(Product.ProductPrice.FirstOrDefault());
                }

                if (image != null && SaveProduct!=null && SaveProduct.Id>0) // обновляем фотографию
                    AddAttachment(image, SaveProduct.Id);

                db.SaveChanges();
                return new RedirectResult("Editor\\" + SaveProduct.Id);
            }

            ///добавление нового товара
            if (SaveProduct != null && SaveProduct.Name != null && SaveProduct.Id == 0 && CheckName(SaveProduct.Name))
            {
                SaveProduct=ProductInsert(SaveProduct);

                if (SaveProduct.Id > 0 && image != null)
                    AddAttachment(image, SaveProduct.Id);

                if(SaveProduct.Id>0)
                    return RedirectToAction("Index");

                else
                    return Json("Ошибка при добавлении товара");
            }


            else
                return Json("Ошибка");
        }


        /// <summary>
        /// Добавить новый товар в базу данных
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product ProductInsert (Product product)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (product!=null && product.ProductPrice.FirstOrDefault() != null)
            {
                product.ProductPrice.FirstOrDefault().Enabled = true;
                product.ProductPrice.FirstOrDefault().DateAdd = DateTime.Now;
            }

            if(product!=null && product.Stock.FirstOrDefault() != null)
            {
                product.Stock.FirstOrDefault().DateAdd = DateTime.Now;
                product.Stock.FirstOrDefault().Quantity =Convert.ToInt32(product.Stock.FirstOrDefault().Balance);
                product.Stock.FirstOrDefault().Text = "Добавление нового товара";
            }

            if (product != null )
            {
                product.DateAdd = DateTime.Now;
                db.Product.Add(product);
                db.SaveChanges();
            }

            return product;
        }

        /// <summary>
        /// Добавить новую цену на товар
        /// </summary>
        /// <param name="NewPrice"></param>
        /// <param name="OldPrice"></param>
        /// <returns></returns>
        private ProductPrice ProductPriceInsert(ProductPrice NewPrice)
        {
            if (NewPrice.Value > 0)
            {
                NewPrice.DateAdd = DateTime.Now;
                NewPrice.Enabled = true;
                NewPrice.Volume = 1;
                db.ProductPrice.Add(NewPrice);
                db.SaveChanges();
            }

            return NewPrice;
        }

        private void AddAttachment(IFormFile file , int ProductId, bool MainMenu=true)
        {
            if (db == null)
                db = new MarketBotDbContext();

            System.IO.MemoryStream s=new System.IO.MemoryStream();

            file.CopyTo(s);
           
            AttachmentFs fs = new AttachmentFs
            {
                Fs = s.ToArray(),
                GuId = Guid.NewGuid(),
                Name = file.FileName,
                AttachmentTypeId = 1
            };

            db.AttachmentFs.Add(fs);

            db.SaveChanges();

            ProductPhoto productPhoto = new ProductPhoto
            {
                AttachmentFsId = fs.Id,
                ProductId = ProductId,
                MainPhoto= MainMenu
            };

            db.ProductPhoto.Add(productPhoto);

            db.SaveChanges();

        }

        /// <summary>
        /// Диактивировать цену на товар
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private int DisablePrice (ProductPrice price)
        {
            if (price != null && price.Id > 0)
            {
                price.Enabled = false;
                db.Entry(price).State = EntityState.Modified;
                return db.SaveChanges();
            }

            else
                return -1;
        }

        /// <summary>
        /// Проверяем занято ли имяю Если занято то возращает FALSE
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
       public bool CheckName (string name)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (db.Product.Where(p => p.Name == name).FirstOrDefault() != null)
                return false;

            else
                return true;
        }
    }
}