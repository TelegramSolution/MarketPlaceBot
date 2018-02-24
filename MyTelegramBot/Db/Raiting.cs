using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Raiting
    {
        public Raiting()
        {
            FeedBack = new HashSet<FeedBack>();
        }

        public int Id { get; set; }
        public short? Value { get; set; }

        public ICollection<FeedBack> FeedBack { get; set; }
    }
}
