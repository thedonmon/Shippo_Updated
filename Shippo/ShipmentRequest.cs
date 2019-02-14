using Newtonsoft.Json;
using Shippo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shippo
{
    /// <summary>
    /// Can use this class to pass object Ids instead of the objects when creating shipments
    /// </summary>
    public class ShipmentRequest : Shipment
    {
        [JsonProperty(PropertyName = "address_from")]
        public new string AddressFrom { get; set; }

        [JsonProperty(PropertyName = "address_to")]
        public new string AddressTo { get; set; }
        [JsonProperty(PropertyName = "parcels")]
        public new string[] Parcels { get; set; }
    }
}
