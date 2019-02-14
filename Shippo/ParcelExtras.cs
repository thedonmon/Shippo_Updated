using Newtonsoft.Json;

namespace Shippo
{
    public class ParcelExtras
    {
        [JsonProperty(PropertyName = "COD")]
        public CollectionOnDelivery COD { get; set; }
        [JsonProperty(PropertyName = "insurance")]
        public InsuranceInformation Insurance { get; set; }
        /// <summary>
        /// Optional text to be printed on the shipping label. Up to 50 characters.
        /// </summary>
        public string reference_1 { get; set; }
        /// <summary>
        /// Optional text to be printed on the shipping label. Up to 50 characters.
        /// </summary>
        public string reference_2 { get; set; }
    }

    public class InsuranceInformation
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        /// <summary>
        /// Specify the carrier insurance to have Insurance provided by the carrier directly, not by Shippo's 3rd-party Shipsurance insurance.
        /// "FEDEX"
        /// "UPS"
        /// "ONTRAC"
        /// </summary>
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }

    public class CollectionOnDelivery
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Secured funds include money orders, certified cheques and others (see UPS and FedEx for details). If no payment_method inputted the value defaults to "ANY".)
        /// "SECURED_FUNDS"
        /// "CASH"
        /// "ANY"
        /// </summary>
        [JsonProperty(PropertyName = "payment_method")]
        public string PaymentMethod { get; set; }
    }
}