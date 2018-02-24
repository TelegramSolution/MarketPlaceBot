using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderTemp
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? AddressId { get; set; }
        public int? FollowerId { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? BotInfoId { get; set; }
        public int? PickupPointId { get; set; }

        public Address Address { get; set; }
        public BotInfo BotInfo { get; set; }
        public Follower Follower { get; set; }
        public PaymentType PaymentType { get; set; }
        public PickupPoint PickupPoint { get; set; }
    }

}
