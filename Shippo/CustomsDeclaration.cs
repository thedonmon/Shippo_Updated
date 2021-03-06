﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shippo
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CustomsDeclaration : ShippoId {
        [JsonProperty(PropertyName = "object_created")]
        public DateTime? ObjectCreated { get; set; }

        [JsonProperty(PropertyName = "object_updated")]
        public DateTime? ObjectUpdated { get; set; }

        [JsonProperty(PropertyName = "object_owner")]
        public string ObjectOwner { get; set; }

        [JsonProperty(PropertyName = "object_state")]
        public string ObjectState { get; set; }

        [JsonProperty(PropertyName = "exporter_reference")]
        public string ExporterReference { get; set; }

        [JsonProperty(PropertyName = "importer_reference")]
        public string ImporterReference { get; set; }
        /// <summary>
        /// 'DOCUMENTS'
        ///'GIFT'
        ///'SAMPLE'
        ///'MERCHANDISE'
        ///'HUMANITARIAN_DONATION'
        ///'RETURN_MERCHANDISE'
        ///'OTHER'
        /// </summary>
        [JsonProperty(PropertyName = "contents_type")]
        public string ContentsType { get; set; }

        [JsonProperty(PropertyName = "contents_explanation")]
        public string ContentsExplanation { get; set; }

        [JsonProperty(PropertyName = "invoice")]
        public string Invoice { get; set; }

        [JsonProperty(PropertyName = "license")]
        public string License { get; set; }

        [JsonProperty(PropertyName = "certificate")]
        public string Certificate { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
        /// <summary>
        /// EEL / PFC type of the shipment. For most shipments from the US to CA, 'NOEEI_30_36' is applicable; for most other shipments from the US, 'NOEEI_30_37_a' is applicable.
        /// 'NOEEI_30_37_a'
        ///'NOEEI_30_37_h'
        ///'NOEEI_30_37_f'
        ///'NOEEI_30_36'
        ///'AES_ITN'
        /// </summary>
        [JsonProperty(PropertyName = "eel_pfc")]
        public string EelPfc { get; set; }
        /// <summary>
        /// AES / ITN reference of the shipment.
        /// </summary>
        [JsonProperty(PropertyName = "aes_itn")]
        public string AesItn { get; set; }
        /// <summary>
        /// 'ABANDON'
        ///'RETURN'
        /// </summary>
        [JsonProperty(PropertyName = "non_delivery_option")]
        public string NonDeliveryOption { get; set; }

        [JsonProperty(PropertyName = "certify")]
        public bool Certify { get; set; }

        [JsonProperty(PropertyName = "certify_signer")]
        public string CertifySigner { get; set; }

        [JsonProperty (PropertyName = "address_importer")]
        public Address AddressImporter { get; set; }

        [JsonProperty(PropertyName = "disclaimer")]
        public string Discliamer { get; set; }
        /// <summary>
        /// The incoterm reference of the shipment. FCA available for DHL Express and FedEx only.
        /// 'DDP', 
        /// 'DDU', 
        /// 'FCA'
        /// </summary>
        [JsonProperty(PropertyName = "incoterm")]
        public string Incoterm { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<string> Items { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; set; }
    }
}

