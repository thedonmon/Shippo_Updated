using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shippo
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Shipment : ShippoId {

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "async")]
        public bool Async { get; set; }

        [JsonProperty(PropertyName = "object_created")]
        public DateTime? ObjectCreated { get; set; }

        [JsonProperty(PropertyName = "object_updated")]
        public DateTime? ObjectUpdated { get; set; }

        [JsonProperty(PropertyName = "object_owner")]
        public string ObjectOwner { get; set; }

        [JsonProperty(PropertyName = "object_status")]
        public string ObjectStatus { get; set; }

        [JsonProperty(PropertyName = "address_from")]
        public virtual Address AddressFrom { get; set; }

        [JsonProperty(PropertyName = "address_to")]
        public virtual Address AddressTo { get; set; }

        [JsonProperty(PropertyName = "parcels")]
        public IEnumerable<Parcel> Parcels { get; set; }
        [Obsolete]
        [JsonProperty(PropertyName = "parcel")]
        public string Parcel { get; set; }

        [JsonProperty(PropertyName = "shipment_date")]
        public DateTime? ShipmentDate { get; set; }

        [JsonProperty(PropertyName = "address_return")]
        public Address AddressReturn { get; set; }

        [JsonProperty(PropertyName = "customs_declaration")]
        public string CustomsDeclaration { get; set; }

        [JsonProperty(PropertyName = "carrier_accounts")]
        public List<string> CarrierAccounts;

        [JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public ShipmentExtras Extra { get; set; }

        [JsonProperty(PropertyName = "rates")]
        public IEnumerable<Rate> Rates { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public IEnumerable<ShippoMessage> Messages { get; set; }

        [JsonProperty(PropertyName = "test")]
        public bool? Test;

        [JsonProperty(PropertyName = "insurance_amount")]
        public decimal Insurance_Amount { get; set; }


        public static Shipment createForBatch(Address addressFrom,
                                              Address addressTo, Parcel[] parcels, ShipmentExtras extra = null)
        {
            Shipment s = new Shipment();
            s.AddressFrom = addressFrom;
            s.AddressTo = addressTo;
            s.Parcels = parcels;
            s.Extra = extra;
            return s;
        }
    }
}

