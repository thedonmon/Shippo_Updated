namespace Shippo
{
    public class ShipmentExtras
    {
        /// <summary>
        /// Request standard or adult signature confirmation. You can alternatively request Certified Mail (USPS only) or Indirect signature (FedEx only) or Carrier Confirmation (Deutsche Post only)
        /// Reference SignatureConfirmationEnum
        /// </summary>
        public string signature_confirmation { get; set; }
        /// <summary>
        /// Request “true” to give carrier permission to leave the parcel in a safe place if no one answers the door
        /// When set to “false” (Canada Post only), if no one is available to receive the item, the parcel won’t be left in a safe place
        /// </summary>
        public bool authority_to_leave { get; set; }
        public bool saturday_delivery { get; set; }
        /// <summary>
        /// Bypasses address validation (USPS, UPS, & LaserShip only).
        /// </summary>
        public bool bypass_address_validation { get; set; }
        /// <summary>
        /// Returns retail rates instead of account-based rates (UPS and FedEx only).
        /// </summary>
        public bool request_retail_rates { get; set; }
        /// <summary>
        /// Specify customer branch (Lasership only)
        /// </summary>
        public bool customer_branch { get; set; }
        /// <summary>
        /// Add premium service to a shipment (DHL Germany international shipments only).
        /// </summary>
        public bool premium { get; set; }
        /// <summary>
        /// Required for DHL Germany Paket Sameday. Designates a desired timeframe for delivery. Format is HHMMHHMM
        /// "10001200"
        /// "12001400"
        /// "14001600"
        /// "16001800"
        /// "18002000"
        /// "19002100"
        /// </summary>
        public string preferred_delivery_timeframe { get; set; }
        /// <summary>
        /// Specify Lasership Attributes (Lasership only). Multiple options accepted.
        /// "TwoPersonDelivery"
        /// "Explosive"
        /// "Alcohol"
        /// "Hazmat"
        /// "ControlledSubstance"
        /// "Refrigerated"
        /// "DryIce"
        /// "Perishable"
        /// </summary>
        public string lasership_attrs { get; set; }
        /// <summary>
        /// Declared value (Lasership only). Defaults to "50.00".
        /// </summary>
        public string lasership_declared_value { get; set; }
        /// <summary>
        /// Specify container type (Lasership only). If no container_type inputted, the value defaults to "Box".
        /// "Box"
        /// "Tube"
        /// "Pak"
        /// "Envelope"
        /// "CustomPackaging"
        /// </summary>
        public string container_type { get; set; }
        /// <summary>
        /// Specify collection on delivery (DHL Germany, FedEx, GLS, OnTrac, and UPS only).
        /// </summary>
        public CollectionOnDelivery COD { get; set; }
        /// <summary>
        /// Specify billing details (UPS, FedEx, and DHL Germany only).
        /// </summary>
        public BillingInformation billing { get; set; }
        /// <summary>
        /// Indicates that a shipment contains Alcohol (Fedex and UPS only).
        /// </summary>
        public AlcoholInformation alcohol { get; set; }
        /// <summary>
        /// Specifies that the package contains Dry Ice (FedEx and UPS only).
        /// </summary>
        public DryIceInformation dry_ice { get; set; }
        /// <summary>
        /// Specify amount, content, provider, and currency of shipment insurance (UPS, FedEx and Ontrac only).
        /// </summary>
        public InsuranceInformation insurance { get; set; }
        /// <summary>
        /// Indicates sort level (USPS Parcel Select only).
        /// <p>"NDC"
        ///"FiveDigit"
        ///"MixedNDC"
        ///"Nonpresorted"
        ///"Presorted"
        ///"SCF"
        ///"SinglePiece"
        ///"ThreeDigit"</p>
        /// </summary>
        public string usps_sort_type { get; set; }
        /// <summary>
        /// Indicates postal facility where mail is entered (USPS Parcel Select only).
        /// "DNDC"
        ///"DDU"
        ///"DSCF"
        ///"ONDC"
        ///"Other"
        /// </summary>
        public string usps_entry_facility { get; set; }
        /// <summary>
        /// Dangerous Goods Code (DHL eCommerce only) https://api.dhlglobalmail.com/docs/v1/appendix.html
        /// </summary>
        public string dangerous_goods_code { get; set; }
        public bool is_return { get; set; }
        /// <summary>
        /// Optional text to be printed on the shipping label. Up to 50 characters.
        /// </summary>
        public string reference_1 { get; set; }
        /// <summary>
        /// Optional text to be printed on the shipping label. Up to 50 characters.
        /// </summary>
        public string reference_2 { get; set; }
    }

    public class DryIceInformation
    {
        public bool contains_dry_ice { get; set; }
        /// <summary>
        /// Mandatory. Units must be in Kilograms. Cannot be greater than package weight.
        /// </summary>
        public decimal weight { get; set; }

    }

    public class AlcoholInformation
    {
        /// <summary>
        /// Mandatory for Fedex and UPS. Specifies that the package contains Alcohol.
        /// </summary>
        public bool contains_alcohol { get; set; }
        /// <summary>
        /// Mandatory for Fedex only. License type of the recipient of the Alcohol Package. Options: "licensee", "consumer"
        /// </summary>
        public string recipient_type { get; set; }
    }

    public class BillingInformation
    {
        /// <summary>
        /// Party to be billed. (Leave blank for DHL Germany.)
        /// "SENDER"
        /// "RECIPIENT"
        /// "THIRD_PARTY"
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Account number to be billed. (For DHL Germany, leave this field blank.)
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 	ZIP code of account number to be billed (required for UPS if there is a zip on the billing account).
        /// </summary>
        public string zip { get; set; }
        /// <summary>
        /// Country iso2 code of account number to be billed (required for UPS third party billing only).
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 2 digit code used to override your default participation code associated with your DHL Germany account.
        /// </summary>
        public string participation_code { get; set; }
    }
}