using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace OnlineClothing.Models.MoMo
{
    public class CollectionLinkRequest
    {
        public string partnerCode { get; set; }
        public string? storeId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderId { get; set; }
        public string orderInfo { get; set; }
        public string redirectUrl { get; set; }
        public string ipnUrl { get; set; }
        public string requestType { get; set; }
        public string extraData { get; set; }
        public List<MoMoItem>? Items { get; set; } = new List<MoMoItem>();
        public MoMoUserInfo? UserInfo { get; set; } = new MoMoUserInfo();
        public bool? autoCapture { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }

    }
}
