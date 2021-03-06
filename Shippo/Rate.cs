﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shippo
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Rate : ShippoId {
        [JsonProperty(PropertyName = "object_created")]
        public DateTime ObjectCreated { get; set; }

        [JsonProperty(PropertyName = "object_owner")]
        public string ObjectOwner { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<string> Attributes { get; set; }

        [JsonProperty(PropertyName = "amount_local")]
        public decimal AmountLocal { get; set; }

        [JsonProperty(PropertyName = "currency_local")]
        public string CurrencyLocal { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }

        [JsonProperty(PropertyName = "provider_image_75")]
        public string ProviderImage75 { get; set; }

        [JsonProperty(PropertyName = "provider_image_200")]
        public string ProviderImage200 { get; set; }

        [JsonProperty(PropertyName = "servicelevel")]
        public ServiceLevel Servicelevel => new ServiceLevel { Name = ServicelevelName, Terms = ServiceLevelTerms, Token = ServicelevelToken };

        [JsonProperty(PropertyName = "servicelevel_name")]
        public string ServicelevelName { get; set; }

        [JsonProperty(PropertyName = "servicelevel_token")]
        public string ServicelevelToken { get; set; }

        [JsonProperty(PropertyName = "servicelevel_terms")]
        public string ServiceLevelTerms { get; set; }

        [JsonProperty(PropertyName = "estimated_days")]
        public int EstimatedDays { get; set; }

        [JsonProperty(PropertyName = "duration_terms")]
        public string DurationTerms { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public List<ShippoMessage> Messages { get; set; }

        [JsonProperty(PropertyName = "zone")]
        public string Zone { get; set; }

        [JsonProperty(PropertyName = "shipment")]
        public string ShipmentId { get; set; }

        [JsonProperty(PropertyName = "insurance")]
        public bool Insurance { get; set; }

        [JsonProperty(PropertyName = "insurance_amount_local")]
        public float Insurance_Amount_Local { get; set; }

        [JsonProperty(PropertyName = "insurance_currency_local")]
        public string Insurance_Currency_Local { get; set; }

        [JsonProperty(PropertyName = "insurance_amount")]
        public float Insurance_Amount { get; set; }

        [JsonProperty(PropertyName = "insurance_currency")]
        public string Insruance_Currency { get; set; }

        [JsonProperty(PropertyName = "carrier_account")]
        public string Carrier_Account { get; set; }

    }

    public class ServiceLevel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "terms")]
        public string Terms { get; set; }
    }
}
