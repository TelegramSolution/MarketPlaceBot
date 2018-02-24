using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string City { get; set; }
        public string Vk { get; set; }
        public string Instagram { get; set; }
        public string Chanel { get; set; }
        public string Telephone { get; set; }
        public string Chat { get; set; }
        public bool? Enable { get; set; }
    }
}
