using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class AttachmentType
    {
        public AttachmentType()
        {
            AttachmentFs = new HashSet<AttachmentFs>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AttachmentFs> AttachmentFs { get; set; }
    }
}
