using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Model
{
    public class AdditionalPhoto
    {
        public int ProductId { get; set; }

        public string Caption { get; set; }

        public string FileId { get; set; }

        public int TelegramAttachId { get; set; }

        public int AttachFsId { get; set; }
    }
}
