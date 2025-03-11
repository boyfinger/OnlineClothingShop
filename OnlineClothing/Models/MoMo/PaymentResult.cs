using System;
using System.Text.Json.Serialization;

namespace OnlineClothing.Models.MoMo
{
    public class PaymentResult
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("payUrl")]
        public string PayUrl { get; set; }

        [JsonPropertyName("shortLink")]
        public string ShortLink { get; set; }
    }
}
