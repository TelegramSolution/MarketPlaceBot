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

            try
            {
                Product product = new Product
                {
                    Name = Name,
                    DateAdd = DateTime.Now,
                    Enable = Enable,
                };

                db.Product.Add(product);
                db.SaveChanges();
                return product;
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

        public Product GetProduct(string Name)
        {
            try
            {
                return db.Product.Where(p => p.Name == Name)
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

        public void Dispose()
        {
            db.Dispose();
        }

    }
}
