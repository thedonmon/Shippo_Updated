using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shippo
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Transaction : ShippoId {
        [JsonProperty(PropertyName = "object_state")]
        public string ObjectState { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "object_status")]
        public string ObjectStatus { get; set; }

        [JsonProperty(PropertyName = "object_created")]
        public DateTime? ObjectCreated { get; set; }

        [JsonProperty(PropertyName = "object_updated")]
        public DateTime? ObjectUpdated { get; set; }

        [JsonProperty(PropertyName = "object_owner")]
        public string ObjectOwner { get; set; }

        [JsonProperty(PropertyName = "test")]
        public bool Test { get; set; }

        [JsonProperty(PropertyName = "async")]
        public bool Async { get; set; }

        [JsonProperty(PropertyName = "rate")]
        public string Rate { get; set; }

        [JsonProperty(PropertyName = "tracking_number")]
        public string TrackingNumber { get; set; }
        /// <summary>
        /// Specify the label file format for this label. If you don't specify this value, the API will default to your default file format that you can set on the settings page
        /// "PNG"
        ///"PNG_2.3x7.5"
        ///"PDF"
        ///"PDF_2.3x7.5"
        ///"PDF_4x6"
        ///"PDF_4x8"
        ///"PDF_A4"
        ///"PDF_A6"
        ///"ZPLII"
        /// </summary>
        [JsonProperty(PropertyName = "label_file_type")]
        public string LabelFileType { get; set; }


        //[JsonProperty(PropertyName = "tracking_status")]
        //public TrackingStatus TrackingStatus { get; set; }

        [JsonProperty(PropertyName = "tracking_status")]
        public ShippoEnums.TrackingStatus TrackingStatus { get; set; }

        [JsonProperty(PropertyName = "tracking_url_provider")]
        public object TrackingUrlProvider { get; set; }

        [JsonProperty(PropertyName = "label_url")]
        public string LabelURL { get; set; }
        /// <summary>
        /// A string of up to 100 characters that can be filled with any additional information you want to attach to the object.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public string MetaData { get; set; }

        [JsonProperty(PropertyName = "commercial_invoice_url")]
    	public string CommercialInvoiceUrl { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public IEnumerable<ShippoMessage> Messages { get; set; }

        [JsonProperty(PropertyName = "eta")]
        public DateTime? ETA { get; set; }


    }
}

