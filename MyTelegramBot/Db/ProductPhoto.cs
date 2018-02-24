using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class ProductPhoto
    {
        public int ProductId { get; set; }
        public int AttachmentFsId { get; set; }

        public bool MainPhoto { get; set; }

        public AttachmentFs AttachmentFs { get; set; }
        public Product Product { get; set; }
    }
}
