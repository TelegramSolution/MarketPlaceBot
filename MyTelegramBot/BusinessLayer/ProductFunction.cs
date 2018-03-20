using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System.IO;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.BusinessLayer
{
    public class ProductFunction
    {
        MarketBotDbContext db { get; set; }
        public ProductFunction()
        {
            db = new MarketBotDbContext();
        }

        public Product InsertProduct(string Name, bool Enable)
        {
            MarketBotDbContext db = new MarketBotDbContext();
            Product product = null;

            var category = db.Category.FirstOrDefault(); // сразу присваеваем какое нибудь значение цены, категори, ед. изм
            var price = db.ProductPrice.Where(p => p.Enabled).FirstOrDefault();
            var unit = db.Units.FirstOrDefault();

            try
            {
                if (category != null && price != null)
                {
                    product = new Product
                    {
                        Name = Name,
                        DateAdd = DateTime.Now,
                        Enable = Enable,
                        UnitId = unit.Id,
                        CategoryId = category.Id,
                        CurrentPriceId = price.Id
                    };
                    db.Product.Add(product);
                    db.SaveChanges();
                    return product;
                }
                if (category != null && price == null)
                {
                    product = new Product
                    {
                        Name = Name,
                        DateAdd = DateTime.Now,
                        Enable = Enable,
                        UnitId = unit.Id,
                        CategoryId = category.Id
                    };
                    db.Product.Add(product);
                    db.SaveChanges();
                    return product;
                }
                if (category != null && price == null)
                {
                    product = new Product
                    {
                        Name = Name,
                        DateAdd = DateTime.Now,
                        Enable = Enable,
                        UnitId = unit.Id,
                        CurrentPriceId = price.Id
                    };
                    db.Product.Add(product);
                    db.SaveChanges();
                    return product;
                }
                else
                {
                    product = new Product
                    {
                        Name = Name,
                        DateAdd = DateTime.Now,
                        Enable = Enable,
                        UnitId = unit.Id,
                    };
                    db.Product.Add(product);
                    db.SaveChanges();
                    return product;
                }

            }

            catch
            {
                return null;
            }
        }


        public Product UpdateName(int ProductId, string Name)
        {
            try
            {
                var product = GetProduct(ProductId);
                if (product != null)
                {
                    product.Name = Name;
                    db.Update<Product>(product);
                    db.SaveChanges();
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public Product UpdateText(int ProductId, string Text)
        {
            try
            {
                var product = GetProduct(ProductId);
                if (product != null)
                {
                    product.Text = Text;
                    db.Update<Product>(product);
                    db.SaveChanges();
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public Product UpdateEnable(int ProductId,bool Enable)
        {
            try
            {
                var product = GetProduct(ProductId);
                if (product != null)
                {
                    product.Enable = Enable;
                    db.Update<Product>(product);
                    db.SaveChanges();
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public Product UpdateUrl(int ProductId, string Url)
        {
            try
            {
                var product = GetProduct(ProductId);
                if (product != null)
                {
                    product.TelegraphUrl = Url;
                    db.Update<Product>(product);
                    db.SaveChanges();
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public Product UpdatepMainPhoto(int ProductId,int AttachFsId)
        {
            try
            {
                var product = GetProduct(ProductId);

                if (product != null)
                {
                    product.MainPhoto = AttachFsId;
                    db.Update<Product>(product);
                    db.SaveChanges();
                }

                return product;
            }

            catch
            {
                return null;
            }
        }

        public Product GetProduct(int ProductId)
        {
            try
            {
                return db.Product.Where(p => p.Id == ProductId)
                    .Include(p => p.Category)
                    .Include(p => p.CurrentPrice)
                    .Include(p => p.Unit)
                    .Include(p => p.Stock)
                    .Include(p => p.Category).FirstOrDefault();
            }

            catch(Exception e)
            {
                return null;
            }
        }

        public static Product GetProductById(int ProductId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Product.Where(p => p.Id == ProductId)
                    .Include(p => p.Category)
                    .Include(p => p.CurrentPrice)
                    .Include(p => p.Unit)
                    .Include(p => p.Stock)
                    .Include(p => p.Category).FirstOrDefault();
            }

            catch (Exception e)
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static List<Product> GetAllProductList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Product.Where(p => p.CurrentPriceId > 0).Include(p => p.CurrentPrice)
                    .Include(p => p.Category).Include(p => p.Unit).ToList();
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public Product GetProduct(string Name)
        {
            try
            {
                return db.Product.Where(p => p.Name == Name)
                    .Include(p => p.Category)
                    .Include(p => p.CurrentPrice)
                    .Include(p => p.Unit)
                    .Include(p => p.Stock)
                    .Include(p=>p.ProductPhoto)
                    .Include(p => p.Category).FirstOrDefault();
            }

            catch (Exception e)
            {
                return null;
            }

        }

        /// <summary>
        /// Функция проверяет является ли данное имя запрещенным.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool NameIsProhibited(string Name)
        {
            StreamReader stream = null;
                

            try
            {
                stream = new StreamReader("Files\\ProhibitedNames.txt");
                string Read= stream.ReadToEnd();

                List<string> Item = Read.Split(',').ToList(); 
                //найдено запрещенное имя
                if (Item.Where(i => i == Name).FirstOrDefault() != null)
                    return true;

                else
                    return false;
            }

            catch
            {
                return false;
            }

            finally
            {
                stream.Dispose();
            }
        }

        public Product UpdatePrice(int ProductId, double Value, int CurrencyId)
        {
            try
            {
                var Price = InsertProductPrice(ProductId, Value, CurrencyId);

                var Product = GetProduct(ProductId);

                if (Product != null && Product.CurrentPrice != null)
                {
                    Product.CurrentPrice.Enabled = false;
                    db.SaveChanges();
                }

                if (Price != null && Product != null)
                {
                    Product.CurrentPriceId = Price.Id;

                    db.Update<Product>(Product);

                    db.SaveChanges();

                    Product.CurrentPrice = Price;

                    return Product;
                }



                else
                    return null;
            }

            catch
            {
                return null;
            }
        }

        public Product UpdateStock(int ProductId, int Value, string Comment="")
        {
            try
            {
                var Product = GetProduct(ProductId);

                if (Value>0 && Product != null && Product.Stock.LastOrDefault()!=null && Product.Stock.LastOrDefault().Balance+Value>0)
                {
                    Product.Stock.Add(InsertStock(ProductId,
                                                  Convert.ToInt32(Product.Stock.LastOrDefault().Balance + Value), 
                                                  Value));
                    return Product;
                }

                if (Value>0 && Product != null  && Product.Stock.Count==0)
                {
                    Product.Stock.Add(InsertStock(ProductId,
                                                  Value,
                                                  Value));
                    return Product;
                }

                //Если после изменения новое значение остатка будет меньше нуля, делаем остаток равным Нулю. Остаток не может быть отрицательным
                if (Value < 0 && Product != null && Product.Stock.LastOrDefault() != null && Product.Stock.LastOrDefault().Balance+Value<0)
                {
                    Product.Stock.Add(InsertStock(ProductId,
                                                  0,
                                                  Convert.ToInt32(Product.Stock.LastOrDefault().Balance + Value)));
                    return Product;
                }


                if (Value<0 && Product != null && Product.Stock.LastOrDefault() != null)
                {
                    Product.Stock.Add(InsertStock(ProductId,
                                                  Convert.ToInt32(Product.Stock.LastOrDefault().Balance+Value),
                                                  Value));
                    return Product;
                }

                else
                    return null;
            }

            catch
            {
                return null;
            }
        }

        public Stock InsertStock(int ProductId, int NewBalance, int Quantity, string Comment="")
        {
            try
            {
                Stock stock = new Stock
                {
                    Balance = NewBalance,
                    DateAdd = DateTime.Now,
                    ProductId = ProductId,
                    Quantity = Quantity,
                    Text = Comment
                };

                db.Stock.Add(stock);
                db.SaveChanges();
                return stock;
            }

            catch
            {
                return null;
            }
        }

        public Stock Balance(int ProductId)
        {
            try
            {
                return db.Stock.Where(s => s.ProductId == ProductId).LastOrDefault();
            }

            catch
            {
                return null;
            }
        }

        public Product UpdateCategory(int ProductId,int CategoryId)
        {
            try
            {
                var product = GetProduct(ProductId);

                var category = db.Category.Find(CategoryId);

                if (product!=null)
                {
                    product.CategoryId = category.Id;
                    db.Update<Product>(product);
                    db.SaveChanges();
                    product.Category = category;
                    
                }

                return product;
            }

            catch
            {
                return null;
            }
        }

        public Product UpdateUnit(int ProductId, int UnitId)
        {
            try
            {
                var product = GetProduct(ProductId);

                var unit = db.Units.Find(UnitId);

                if (product != null && unit!=null)
                {
                    product.UnitId = UnitId;
                    db.Update<Product>(product);
                    db.SaveChanges();
                    product.Unit = unit;
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public Product UpdateUnit(int ProductId, string UnitName)
        {
            try
            {
                var product = GetProduct(ProductId);

                var unit = db.Units.Where(u => u.Name == UnitName).FirstOrDefault();

                if (product != null && unit != null)
                {
                    product.UnitId = unit.Id;
                    db.Update<Product>(product);
                    db.SaveChanges();
                    product.Unit = unit;
                }

                return product;

            }

            catch
            {
                return null;
            }
        }

        public ProductPrice InsertProductPrice(int ProductId, double Value, int CurrencyId)
        {
            try
            {
                ProductPrice productPrice = new ProductPrice
                {
                    ProductId = ProductId,
                    DateAdd = DateTime.Now,
                    Enabled = true,
                    Value = Value,
                    CurrencyId=CurrencyId
                };

                db.ProductPrice.Add(productPrice);
                db.SaveChanges();
                return productPrice;
            }

            catch
            {
                return null;
            }
        }

        public ProductPhoto InsertAdditionallPhoto(int ProductId,Telegram.Bot.Types.File Photo, string Caption=null)
        {
            byte[] PhotoByte = null;

            AttachmentFs attachmentFs = null;

            ProductPhoto productPhoto = null;
            try
            {
                if (ProductId > 0 && Photo != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Photo.FileStream.CopyTo(ms);
                        PhotoByte = ms.ToArray();
                    }

                    if (PhotoByte != null)
                        attachmentFs = InsertAttachmentFs(PhotoByte, Caption: Caption);

                    if (attachmentFs != null)
                    {
                        productPhoto = new ProductPhoto
                        {
                            AttachmentFsId = attachmentFs.Id,
                            ProductId = ProductId
                        };

                        db.ProductPhoto.Add(productPhoto);

                        db.SaveChanges();

                    }

                    return productPhoto;
                }

                return null;
            }

            catch
            {
                return null;
            }
           
        }

        public int RemoveAdditionalPhoto(int ProductId, int AttachFsId)
        {
            try
            {
               var photo=db.ProductPhoto.Where(p => p.ProductId == ProductId && p.AttachmentFsId == AttachFsId).FirstOrDefault();

                if (photo != null)
                {
                    db.ProductPhoto.Remove(photo);

                    return db.SaveChanges();
                }

                else
                    return 0; 

            }

            catch
            {
                return -1;
            }
        }

        private AttachmentFs InsertAttachmentFs(byte[] PhotoByte, int AttachmentTypeId= ConstantVariable.MediaTypeVariable.Photo,string Caption="", string Name = "Photo.jpg")
        {
            try
            {
                AttachmentFs attachmentFs = new AttachmentFs
                {
                    AttachmentTypeId = AttachmentTypeId,
                    GuId = Guid.NewGuid(),
                    Caption = Caption,
                    Fs = PhotoByte,
                    Name = Name
                };

                db.AttachmentFs.Add(attachmentFs);
                db.SaveChanges();
                return attachmentFs;
            }

            catch
            {
                return null;
            }
        }


        public static List<Model.AdditionalPhoto> GetAdditionalPhoto(int ProductId, int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            List<Model.AdditionalPhoto> Result = new List<Model.AdditionalPhoto>();

            try
            {
                var product = db.Product.Where(p => p.Id == ProductId).Include(p=>p.ProductPhoto).FirstOrDefault();

                if (product != null)
                {
                    foreach(ProductPhoto photo in product.ProductPhoto)
                    {
                        photo.AttachmentFs = db.AttachmentFs.Find(photo.AttachmentFsId);

                        var tg_attach = db.AttachmentTelegram.Where(a => a.AttachmentFsId == photo.AttachmentFsId && a.BotInfoId == BotId).FirstOrDefault();

                        if (tg_attach != null && photo.AttachmentFs.AttachmentTypeId==ConstantVariable.MediaTypeVariable.Photo)
                        {
                            Model.AdditionalPhoto additional = new Model.AdditionalPhoto();
                            additional.ProductId = product.Id;
                            additional.Caption = photo.AttachmentFs.Caption;
                            additional.FileId = tg_attach.FileId;
                            additional.TelegramAttachId = tg_attach.Id;
                            additional.AttachFsId = photo.AttachmentFsId;
                            Result.Add(additional);
                        }
                    }
                    
                }

                return Result;
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}
