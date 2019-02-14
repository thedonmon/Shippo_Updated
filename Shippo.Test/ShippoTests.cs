using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shippo;

namespace ShippoAPITests
{
    [TestClass]
    public class ShippoTests
    {
        private APIResource _apiResource;
        [TestInitialize]
        public void Initialize()
        {
            _apiResource = new APIResource("<API_TOKEN>");
            _apiResource.SetApiVersion("2018-02-08");
        }



        [TestMethod]
        public async Task CreteAddressIsValidTest()
        {

            var address = new Address
            {
                Name = "David Sixsmith",
                Street1 = "One Infinite Loop",
                City = "Cupertino",
                State = "CA",
                Zip = "95014",
                Country = "US",
                Email = "support@apple.com",
                IsResidential = false,
                Company = "Apple Inc.",
                Phone = "4086065775",
                Validate = true
            };
            var response = await _apiResource.CreateAddressAsync(address);

            Address parameters2 = CreateFromAddress();

            var address2 = await _apiResource.CreateAddressAsync(parameters2).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(address.ObjectId));

            Assert.IsTrue(!string.IsNullOrEmpty(address2.ObjectId));


        }


        [TestMethod]
        public async Task CreteShipmentIsValidTest()
        {
            var addressTo = CreateToAddress();
            var addressFrom = CreateFromAddress();
            var parcel = CreateParcel();

            //Replace this list with your carrier account ids you want to use to test!!
            List<string> carriers = new List<string>
            {
                "CarrierAccount1",
                "CarrierAccount2",
                "CarrierAccount3"
            };
            var customsDeclaration = await CreateInternationalTariffParamater().ConfigureAwait(false);

            var shipmentToCreate = new Shipment
            {
                AddressTo = addressTo,
                AddressFrom = addressFrom,
                Parcels = new Parcel[] { parcel},
                CarrierAccounts = carriers,
                CustomsDeclaration = customsDeclaration.ObjectId,
                Extra = new ShipmentExtras
                {
                    reference_1 = "Test"
                }

            };


            var shipment = await _apiResource.CreateShipmentAsync(shipmentToCreate).ConfigureAwait(false);


            Assert.IsTrue(!string.IsNullOrEmpty(shipment.ObjectId));

            Assert.IsTrue(shipment.Rates.Any());

            Assert.IsTrue(!string.IsNullOrEmpty(shipment.Status));

            var shipmentRetrieve = await _apiResource.RetrieveShipmentAsync(shipment.ObjectId).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(shipmentRetrieve.ObjectId));

        }


        [TestMethod]
        public void CreteParcelIsValidTest()
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("length", "6");
            parameters.Add("width", "4");
            parameters.Add("height", "4");
            parameters.Add("distance_unit", "in");
            parameters.Add("weight", 0 * 16 + 12);
            parameters.Add("mass_unit", "oz");

            var SPParcel = _apiResource.CreateParcel(parameters);
            Assert.IsTrue(!string.IsNullOrEmpty(SPParcel.ObjectId));
        }
        [TestMethod]
        public async Task CreteTransactionIsValidTest()
        {
            var addressTo = CreateToAddress();
            var addressFrom = CreateFromAddress();
            var parcel = CreateParcel();
            
            //Replace this list with your carrier account ids you want to use to test!!
            List<string> carriers = new List<string>
            {
                "CarrierAccount1",
                "CarrierAccount2",
                "CarrierAccount3"
            };

            var customsDeclaration = await CreateInternationalTariffParamater().ConfigureAwait(false);

            var shipmentToCreate = new ShipmentRequest
            {
                AddressTo = addressTo.ObjectId,
                AddressFrom = addressFrom.ObjectId,
                Parcels = new string[] { parcel.ObjectId },
                CarrierAccounts = carriers,
                CustomsDeclaration = customsDeclaration.ObjectId,
                Extra = new ShipmentExtras
                {
                    reference_1 = "Test"
                }

            };


            var shipment = await _apiResource.CreateShipmentAsync(shipmentToCreate).ConfigureAwait(false);
            Assert.IsTrue(!string.IsNullOrEmpty(shipment.ObjectId));
            var rateToBuy = shipment.Rates.OrderBy(o => o.Amount).Select(x => x.ObjectId).FirstOrDefault();

            var transaction = new Transaction
            {
                Rate = rateToBuy,
                LabelFileType = "PDF",
                MetaData = "This is a test shipment with new API",
            };
            var transactionCreated = await _apiResource.CreateTransactionAsync(transaction).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(transactionCreated.ObjectId));
            var refund = new Refund
            {
                Transaction = transactionCreated.ObjectId

            };
            var refunded = await _apiResource.CreateRefundAsync(refund).ConfigureAwait(false);

            Assert.IsTrue((refunded.Status == "SUCCESS") || (refunded.Status == "PENDING"));
        }

        [TestMethod]
        public async Task GetTrack()
        {
            var results = await _apiResource.RetrieveTrackingAsync("usps", "<trackingNumber>");
            
        }
        [TestMethod]
        public async Task GetTransaction()
        {

            var results = await _apiResource.RetrieveTransactionAsync("<transactionId>");

        }
        [TestMethod]
        public async Task GetParcel()
        {
            var parcel = new Parcel
            {
                Length = 10,
                Width = 8,
                Height = 2,
                DistanceUnit = "in",
                Weight = 3.0M,
                MassUnit = "oz",
                Extra = new ParcelExtras
                {
                    COD = new CollectionOnDelivery
                    {
                        Amount = "10.00",
                        Currency = "USD",
                        PaymentMethod = "ANY"
                    },
                    reference_1 = "test"
                }

            };
            var creeatedParcel = await _apiResource.CreateParcelAsync(parcel).ConfigureAwait(false);
            var results = await _apiResource.RetrieveParcelAsync(creeatedParcel.ObjectId).ConfigureAwait(false);

        }
        private async Task<CustomsDeclaration> CreateInternationalTariffParamater()
        {

            List<string> listOfCustomItemIds = new List<string>();

            var customsItem = new CustomsItem
            {
                Description = "HardCover Book",
                Quantity = 1,
                NetWeight = 12,
                MassUnit = "oz",
                ValueAmount = 1,
                TariffNumber = "490199",
                SkuCode = null,
                ValueCurrency = "USD",
                OriginCountry = "US"
            };
            var customItem = await _apiResource.CreateCustomsItemAsync(customsItem).ConfigureAwait(false);

            var customsDeclaration = new CustomsDeclaration
            {
                CertifySigner = "Google",
                Certify = true,
                Items = new List<string>
                {
                    customItem.ObjectId
                },
                NonDeliveryOption = "RETURN",
                ContentsType = "MERCHANDISE",
                ContentsExplanation = "Purchase",
                EelPfc = "NOEEI_30_37_a"
            };

            return await _apiResource.CreateCustomsDeclarationAsync(customsDeclaration).ConfigureAwait(false);


        }
        private Parcel CreateParcel()
        {

            Hashtable parameters = new Hashtable();
            parameters.Add("length", "6");
            parameters.Add("width", "4");
            parameters.Add("height", "4");
            parameters.Add("distance_unit", "in");
            parameters.Add("weight", 0 * 16 + 12);
            parameters.Add("mass_unit", "oz");

            var SPParcel = _apiResource.CreateParcel(parameters);
            return SPParcel;
        }
        private Address CreateToAddress()
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("object_purpose", "PURCHASE");
            parameters.Add("name", "David Sixsmith");
            parameters.Add("street1", "3 Rue de Sévigné");
            parameters.Add("street2", "");
            parameters.Add("city", "PARIS");
            parameters.Add("state", "FR");
            parameters.Add("zip", "75004");
            parameters.Add("country", "FR");
            parameters.Add("phone", "8444744726");
            parameters.Add("validate", "false");
            parameters.Add("email", "support@shipbob.com");
            parameters.Add("is_residential", true);
            parameters.Add("company", "");

            var address = _apiResource.CreateAddress(parameters);
            return address;
        }
        private Address CreateFromAddress()
        {
            Hashtable parameters2 = new Hashtable();
            parameters2.Add("object_purpose", "PURCHASE");
            parameters2.Add("name", "Google HQ");
            parameters2.Add("street1", "320 N Morgan St");
            parameters2.Add("street2", "#600");
            parameters2.Add("city", "Chicago");
            parameters2.Add("state", "IL");
            parameters2.Add("zip", "60607");
            parameters2.Add("country", "US");
            parameters2.Add("phone", "3128404100");
            parameters2.Add("validate", "false");
            parameters2.Add("email", "support@google.com");
            parameters2.Add("is_residential", true);
            parameters2.Add("company", "Google");


            var address2 = _apiResource.CreateAddress(parameters2);
            return address2;
        }
    }
}
