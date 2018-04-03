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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class ProductController : Controller
    {
       // MarketBotDbContext db;

        Product Product { get; set; }

        private readonly IHostingEnvironment _appEnvironment;

        Telegram.Bot.TelegramBotClient TelegramBotClient { get; set; }

        BotInfo BotInfo { get; set; }

        ProductFunction ProductFunc { get; set; }

        public ProductController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {

            var products = ProductFunction.GetAllProductList();

            return View(products);


        }

        /// <summary>
        /// Добавить новый
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Creator()
        {


            var conf = Bot.GeneralFunction.GetBotInfo();

            var catlist = CategoryFunction.GetListCategory();

            Product product = new Product();
            product.Id = 0;
            product.Name = String.Empty;
            product.CategoryId = 0;
            product.UnitId = 1;
            product.TelegraphUrl = String.Empty;
            product.Text = String.Empty;
            product.PhotoUrl = String.Empty;
            product.CurrentPrice=new ProductPrice { CurrencyId = conf.Configuration.CurrencyId, Value = 0 };
            product.Stock.Add(new Stock { Balance = 100, ProductId = 0 });

            if(catlist.Count>0)
                ViewBag.Category = new SelectList(catlist, "Id", "Name", catlist.FirstOrDefault().Id);

            else
                ViewBag.Category = new SelectList(catlist, "Id", "Name", 0);


            ViewBag.Currency = CurrencyFunction.CurrencyList();
            ViewBag.Unit = new SelectList(UnitsFunction.UnitsList(), "Id", "Name", product.UnitId);
            return View("Editor", product);
        }

        /// <summary>
        /// Добавить доп фото
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Photos(int id)
        {
            ViewBag.ProductId = id;
            return View(ProductFunction.GetAdditionalPhoto(id));
        }

        /// <summary>
        /// удалить фото
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]

        public IActionResult DeletePhoto(int Id)
        {

            ProductFunc = new ProductFunction();

            ProductFunc.RemoveAdditionalPhoto(Id);

            return Ok();

        }

        /// <summary>
        /// загрузить фотографию
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoto(int ProductId, IList<IFormFile> image)
        {
            ProductFunc = new ProductFunction();

            foreach (var img in image)
            {
                var photo=ProductFunc.InsertAdditionallPhoto(ProductId, img.OpenReadStream(), img.Name);

                await this.SendPhotoAndSaveFileId(photo.AttachmentFs);
            }

            ProductFunc.Dispose();

            return new RedirectResult("Photos\\" + ProductId);
                     
        }



        /// <summary>
        /// Редакатирование товара
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Editor (int id)
        {
            ProductFunc = new ProductFunction();
            
            if (id > 0)
            {
                var product = ProductFunc.GetProduct(id);

                if (product.MainPhotoNavigation!= null) // вытаскиваем главную фотографию и готовим ее к отображению на странице
                {
                    string imageBase64Data = Convert.ToBase64String(product.MainPhotoNavigation.Fs);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    ViewBag.ImageData = imageDataURL;
                }

                ViewBag.Category = new SelectList(CategoryFunction.GetListCategory(), "Id", "Name",product.CategoryId);

                ViewBag.Currency = CurrencyFunction.CurrencyList();

                ViewBag.Unit = new SelectList(UnitsFunction.UnitsList(), "Id", "Name",product.UnitId);

                ProductFunc.Dispose();

                if (product != null)
                    return View(product);

                else
                    return NoContent();
            }



            else
                return null;
        }


        /// <summary>
        /// сохранить изменения
        /// </summary>
        /// <param name="SaveProduct"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save (Product SaveProduct, IFormFile image = null, int balance=0)
        {

            bool NameIsProhibited = false;

            BotInfo = Bot.GeneralFunction.GetBotInfo();

            ProductFunc = new ProductFunction();


            if(SaveProduct!=null)
                NameIsProhibited = ProductFunc.NameIsProhibited(SaveProduct.Name);

            if(NameIsProhibited)
                return Json("Данное имя запрещено");

            if (SaveProduct != null && SaveProduct.CurrentPrice.Value <= 0)
                return Json("Стоимость должна быть больше 0");

            if(SaveProduct!=null && SaveProduct.CategoryId<1)
                return Json("Выбертие категорию");

            if(SaveProduct != null && SaveProduct.Text!=null && SaveProduct.Text.Length>100)
                return Json("Ошибка. Максимальная длина описания 100 символов");

            if (!NameIsProhibited && SaveProduct != null && SaveProduct.Id > 0) // обновление уже сущ. товара
            {
                await UpdateProduct(SaveProduct, image);
                ProductFunc.Dispose();
                return new RedirectResult("Editor\\" + SaveProduct.Id);
            }

            ///добавление нового товара
            if (!NameIsProhibited && SaveProduct != null && SaveProduct.Name != null 
                && SaveProduct.Id == 0 && SaveProduct.CurrentPrice.Value>0 && SaveProduct.CategoryId>0)
            {
                int id=await CreateProduct(SaveProduct, image,balance);
                return new RedirectResult("Editor\\" + id);
            }


            else
                return Json("Ошибка");
        }


        private async Task<int> UpdateProduct(Product SaveProduct, IFormFile image = null)
        {

            AttachmentFs attachmentFs = null;


            if (image != null) // загружаем фотографию в базу данных
                attachmentFs = ProductFunc.InsertMainPhoto(SaveProduct.Id, image.OpenReadStream(), image.FileName);


            if (attachmentFs != null) // обновляем id фотографии  у продукта
            {
                SaveProduct.MainPhoto = attachmentFs.Id;

                //отправляем эту фотографию в телеграм и вытаскивваем FileId. Далее этот FileID присваеваем этой картинке
                await SendPhotoAndSaveFileId(attachmentFs);
            }


            ProductFunc.UpdateProduct(SaveProduct);

            return SaveProduct.Id;


        }

        private async Task<int> CreateProduct(Product SaveProduct, IFormFile image = null, int balance=0)
        {
            AttachmentFs attachmentFs = null;

            SaveProduct = ProductFunc.InsertProduct(SaveProduct.Name,
                                        Convert.ToInt32(SaveProduct.CategoryId),
                                        Convert.ToInt32(SaveProduct.UnitId),
                                        SaveProduct.CurrentPrice.Value,
                                        Convert.ToInt32(SaveProduct.CurrentPrice.CurrencyId),
                                        SaveProduct.Enable,
                                        SaveProduct.Text,
                                        SaveProduct.TelegraphUrl);

            ///добавляем остатки
            if (SaveProduct.Stock != null && balance > 0)
                ProductFunc.InsertStock(SaveProduct.Id, balance, balance, "добавление нового товара");


            if (image != null && SaveProduct!=null) // загружаем фотографию в базу данных
                attachmentFs = ProductFunc.InsertMainPhoto(SaveProduct.Id, image.OpenReadStream(), image.FileName);

            if (attachmentFs != null) // обновляем id фотографии  у продукта
            {
                ProductFunc.UpdateMainPhoto(SaveProduct.Id, attachmentFs.Id);

                //отправляем эту фотографию в телеграм и вытаскивваем FileId. Далее этот FileID присваеваем этой картинке
                await SendPhotoAndSaveFileId(attachmentFs);
            }

            return SaveProduct.Id;

        }

        private async Task<string> SendPhotoAndSaveFileId(AttachmentFs attachmentFs)
        {
            if (BotInfo == null)
                BotInfo = Bot.GeneralFunction.GetBotInfo();


            try
            {
                string token = BotInfo.Token;

                TelegramBotClient = new Telegram.Bot.TelegramBotClient(token);

                System.IO.Stream stream = new MemoryStream(attachmentFs.Fs);

                Telegram.Bot.Types.FileToSend fileToSend = new Telegram.Bot.Types.FileToSend
                {
                    Content = stream,
                    Filename = "Photo.jpg"
                };

                var Message = await TelegramBotClient.SendPhotoAsync(BotInfo.OwnerChatId, fileToSend);
                string Fileid = Message.Photo[Message.Photo.Length - 1].FileId;

                AttachmentTelegramFunction.AddAttachmentTelegram(attachmentFs.Id, BotInfo.Id, Fileid);

                return Fileid;
            }

            catch
            {
                return null;
            }

            


        }
    }
}