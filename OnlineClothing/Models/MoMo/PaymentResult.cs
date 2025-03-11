﻿using System;

namespace OnlineClothing.Models.MoMo
{
    public class PaymentResult
    {
        public string PartnerCode { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string PayUrl { get; set; }
        public string ShortLink { get; set; }
    }
}