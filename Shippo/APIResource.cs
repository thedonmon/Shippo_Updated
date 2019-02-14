/*
* Copyright 2011 - 2012 Xamarin, Inc., 2011 - 2012 Joe Dluzen
*
* Author(s):
*  Gonzalo Paniagua Javier (gonzalo@xamarin.com)
*  Joe Dluzen (jdluzen@gmail.com)
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shippo
{
    public class APIResource
    {
        public static readonly string api_endpoint = "https://api.goshippo.com/";
        static readonly string user_agent = "Shippo/v1 CSharpBindings/1.0";
        public static readonly int RatesReqTimeout = 25;
        public static readonly int TransactionReqTimeout = 25;
        static readonly Encoding encoding = Encoding.UTF8;
        String accessToken;
        String apiVersion;

        // API Resource Constructor
        public APIResource(string inputToken)
        {
            accessToken = inputToken;
            TimeoutSeconds = 25;
            apiVersion = null;
        }

        public virtual void SetApiVersion(String version)
        {
            apiVersion = version;
        }

        #region Shared
        // Setup Request handles headers, credentials etc for WebRequests
        protected virtual WebRequest SetupRequest(string method, string url)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = method;
            if (req is HttpWebRequest)
            {
                ((HttpWebRequest)req).UserAgent = user_agent;
            }

            /* ENABLE BLOCK FOR BASIC AUTH
            req.Credentials = credential;
            req.PreAuthenticate = true; */

            // Disable lines below for basic auth
            string tokenType = "ShippoToken";
            if (accessToken.StartsWith("oauth."))
            {
                tokenType = "Bearer";
            }
            req.Headers.Add("Authorization", string.Format("{0} {1}", tokenType, accessToken));
            if (apiVersion != null)
            {
                req.Headers.Add("Shippo-API-Version", apiVersion);
            }
            req.Timeout = TimeoutSeconds * 1000;
            // When Performing POST requests it is important that we set the headers to json
            if (method == "POST" || method == "PUT")
                req.ContentType = "application/json";
            return req;
        }
        // Return response as String
        static string GetResponseAsString(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
            {
                return sr.ReadToEnd();
            }
        }
        static async Task<string> GetResponseAsStringAsync(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
            {
                return await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }
        // GET Requests
        public virtual T DoRequest<T>(string endpoint, string method = "GET", string body = null)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = DoRequest(endpoint, method, body);
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
        public virtual async Task<T> DoRequestAsync<T>(string endpoint, string method = "GET", string body = null)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = await DoRequestAsync(endpoint, method, body).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
        // GET Requests Helper
        public virtual string DoRequest(string endpoint)
        {
            return DoRequest(endpoint, "GET", null);
        }
        public virtual async Task<string> DoRequestAsync(string endpoint)
        {
            return await DoRequestAsync(endpoint, "GET", null).ConfigureAwait(false);
        }
        // Requests Main Function
        public virtual string DoRequest(string endpoint, string method, string body)
        {
            string result = null;
            WebRequest req = SetupRequest(method, endpoint);
            if (body != null)
            {
                byte[] bytes = encoding.GetBytes(body.ToString());
                req.ContentLength = bytes.Length;
                using (Stream st = req.GetRequestStream())
                {
                    st.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (WebResponse resp = (WebResponse)req.GetResponse())
                {
                    result = GetResponseAsString(resp);
                }
            }
            catch (WebException wexc)
            {
                if (wexc.Response != null)
                {
                    string json_error = GetResponseAsString(wexc.Response);
                    HttpStatusCode status_code = HttpStatusCode.BadRequest;
                    HttpWebResponse resp = wexc.Response as HttpWebResponse;
                    if (resp != null)
                        status_code = resp.StatusCode;

                    if ((int)status_code <= 500)
                        throw new ShippoException(json_error, wexc);
                }
                throw;
            }
            return result;
        }
        public virtual async Task<string> DoRequestAsync(string endpoint, string method, string body)
        {
            string result = null;
            WebRequest req = SetupRequest(method, endpoint);
            if (body != null)
            {
                byte[] bytes = encoding.GetBytes(body.ToString());
                req.ContentLength = bytes.Length;
                using (Stream st = await req.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await st.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }
            }

            try
            {
                using (WebResponse resp = await req.GetResponseAsync().ConfigureAwait(false))
                {
                    result = await GetResponseAsStringAsync(resp).ConfigureAwait(false);
                }
            }
            catch (WebException wexc)
            {
                if (wexc.Response != null)
                {
                    string json_error = await GetResponseAsStringAsync(wexc.Response).ConfigureAwait(false);
                    HttpStatusCode status_code = HttpStatusCode.BadRequest;
                    HttpWebResponse resp = wexc.Response as HttpWebResponse;
                    if (resp != null)
                        status_code = resp.StatusCode;

                    if ((int)status_code <= 500)
                        throw new ShippoException(json_error, wexc);
                }
                throw;
            }
            return result;
        }

        protected virtual StringBuilder UrlEncode(IUrlEncoderInfo infoInstance)
        {
            StringBuilder str = new StringBuilder();
            infoInstance.UrlEncode(str);
            if (str.Length > 0)
                str.Length--;
            return str;
        }
        // Generate URL Encoded parameters for GET requests
        public String generateURLEncodedFromHashmap(Hashtable propertyMap)
        {
            StringBuilder str = new StringBuilder();
            foreach (DictionaryEntry pair in propertyMap)
            {
                str.AppendFormat("{0}={1}&", pair.Key, pair.Value);
            }
            str.Length--;

            return str.ToString();
        }
        // Serialize parameters into JSON for POST requests
        public String serialize<T>(T data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return JsonConvert.SerializeObject(data, settings);
        }

        #endregion

        #region Address

        public Address CreateAddress(Hashtable parameters)
        {
            string ep = String.Format("{0}/addresses", api_endpoint);
            var address = DoRequest<Address>(ep, "POST", serialize(parameters));
            return address;
        }

        public Address RetrieveAddress(String id)
        {
            string ep = String.Format("{0}/addresses/{1}", api_endpoint, id);
            return DoRequest<Address>(ep, "GET");
        }

        public Address ValidateAddress(String id)
        {
            string ep = String.Format("{0}/addresses/{1}/validate", api_endpoint, id);
            return DoRequest<Address>(ep, "GET");
        }

        public async Task<ShippoCollection<Address>> AllAddresssAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/addresses?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Address>>(ep).ConfigureAwait(false);
        }
        public async Task<Address> CreateAddressAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/addresses", api_endpoint);
            var address = await DoRequestAsync<Address>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
            return address;
        }
        public async Task<Address> CreateAddressAsync(Address parameters)
        {
            string ep = String.Format("{0}/addresses", api_endpoint);
            var address = await DoRequestAsync<Address>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
            return address;
        }
        public async Task<Address> RetrieveAddressAsync(String id)
        {
            string ep = String.Format("{0}/addresses/{1}", api_endpoint, id);
            return await DoRequestAsync<Address>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<Address> ValidateAddressAsync(String id)
        {
            string ep = String.Format("{0}/addresses/{1}/validate", api_endpoint, id);
            return await DoRequestAsync<Address>(ep, "GET").ConfigureAwait(false);
        }


        public ShippoCollection<Address> AllAddresss(Hashtable parameters)
        {
            string ep = String.Format("{0}/addresses?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Address>>(ep);
        }

        #endregion

        #region Parcel

        public Parcel CreateParcel(Hashtable parameters)
        {
            string ep = String.Format("{0}/parcels", api_endpoint);
            return DoRequest<Parcel>(ep, "POST", serialize(parameters));
        }

        public Parcel RetrieveParcel(String id)
        {
            string ep = String.Format("{0}/parcels/{1}", api_endpoint, id);
            return DoRequest<Parcel>(ep, "GET");
        }

        public ShippoCollection<Parcel> AllParcels(Hashtable parameters)
        {
            string ep = String.Format("{0}/parcels?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Parcel>>(ep);
        }
        public async Task<Parcel> CreateParcelAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/parcels", api_endpoint);
            return await DoRequestAsync<Parcel>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Parcel> CreateParcelAsync(Parcel parameters)
        {
            string ep = String.Format("{0}/parcels", api_endpoint);
            return await DoRequestAsync<Parcel>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<Parcel> RetrieveParcelAsync(String id)
        {
            string ep = String.Format("{0}/parcels/{1}", api_endpoint, id);
            return await DoRequestAsync<Parcel>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<Parcel>> AllParcelsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/parcels?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Parcel>>(ep).ConfigureAwait(false);
        }

        #endregion

        #region Shipment

        public Shipment CreateShipment(Hashtable parameters)
        {
            string ep = String.Format("{0}/shipments", api_endpoint);
            return DoRequest<Shipment>(ep, "POST", serialize(parameters));
        }

        public Shipment RetrieveShipment(String id)
        {
            string ep = String.Format("{0}/shipments/{1}", api_endpoint, id);
            return DoRequest<Shipment>(ep, "GET");
        }

        public ShippoCollection<Shipment> AllShipments(Hashtable parameters)
        {
            string ep = String.Format("{0}/shipments?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Shipment>>(ep);
        }
        public async Task<Shipment> CreateShipmentAsync(Shipment parameters)
        {
            string ep = String.Format("{0}/shipments", api_endpoint);
            return await DoRequestAsync<Shipment>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Shipment> CreateShipmentAsync(ShipmentRequest parameters)
        {
            string ep = String.Format("{0}/shipments", api_endpoint);
            return await DoRequestAsync<Shipment>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Shipment> CreateShipmentAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/shipments", api_endpoint);
            return await DoRequestAsync<Shipment>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<Shipment> RetrieveShipmentAsync(String id)
        {
            string ep = String.Format("{0}/shipments/{1}", api_endpoint, id);
            return await DoRequestAsync<Shipment>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<Shipment>> AllShipmentsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/shipments?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Shipment>>(ep).ConfigureAwait(false);
        }


        #endregion

        #region Rate

        public ShippoCollection<Rate> CreateRate(Hashtable parameters)
        {
            string ep = String.Format("{0}/shipments/{1}/rates/{2}", api_endpoint, parameters["id"], parameters["currency_code"]);
            return DoRequest<ShippoCollection<Rate>>(ep, "GET");
        }

        public ShippoCollection<Rate> GetShippingRatesSync(String objectId)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("id", objectId);
            parameters.Add("currency_code", "");
            return GetShippingRatesSync(parameters);
        }

        public ShippoCollection<Rate> GetShippingRatesSync(Hashtable parameters)
        {

            String object_id = (String)parameters["id"];
            Shipment shipment = RetrieveShipment(object_id);
            String object_status = (String)shipment.Status;
            long startTime = DateTimeExtensions.UnixTimeNow();

            while (object_status.Equals("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals("WAITING", StringComparison.OrdinalIgnoreCase))
            {
                if (DateTimeExtensions.UnixTimeNow() - startTime > RatesReqTimeout)
                {
                    throw new RequestTimeoutException(
                        "A timeout has occured while waiting for your rates to generate. Try retreiving the Shipment object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                shipment = RetrieveShipment(object_id);
                object_status = (String)shipment.Status;
            }

            return CreateRate(parameters);
        }
        public async Task<ShippoCollection<Rate>> GetShippingRatesAsync(Hashtable parameters)
        {

            String object_id = (String)parameters["id"];
            Shipment shipment = await RetrieveShipmentAsync(object_id).ConfigureAwait(false);
            String object_status = (String)shipment.Status;
            long startTime = DateTimeExtensions.UnixTimeNow();

            while (object_status.Equals("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals("WAITING", StringComparison.OrdinalIgnoreCase))
            {
                if (DateTimeExtensions.UnixTimeNow() - startTime > RatesReqTimeout)
                {
                    throw new RequestTimeoutException(
                        "A timeout has occured while waiting for your rates to generate. Try retreiving the Shipment object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                shipment = await RetrieveShipmentAsync(object_id).ConfigureAwait(false);
                object_status = (String)shipment.Status;
            }

            return CreateRate(parameters);
        }

        public Rate RetrieveRate(String id)
        {
            string ep = String.Format("{0}/rates/{1}", api_endpoint, id);
            return DoRequest<Rate>(ep, "GET");
        }
        public async Task<Rate> RetrieveRateAsync(String id)
        {
            string ep = String.Format("{0}/rates/{1}", api_endpoint, id);
            return await DoRequestAsync<Rate>(ep, "GET").ConfigureAwait(false);
        }


        #endregion

        #region Transaction

        public Transaction CreateTransaction(Hashtable parameters)
        {
            string ep = String.Format("{0}/transactions", api_endpoint);
            return DoRequest<Transaction>(ep, "POST", serialize(parameters));
        }

        public Transaction CreateTransactionSync(Hashtable parameters)
        {
            string ep = String.Format("{0}/transactions", api_endpoint);
            Transaction transaction = DoRequest<Transaction>(ep, "POST", serialize(parameters));
            String object_id = (String)transaction.ObjectId;
            String object_status = (String)transaction.Status;
            long startTime = DateTimeExtensions.UnixTimeNow();

            while (object_status.Equals("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals("WAITING", StringComparison.OrdinalIgnoreCase))
            {
                if (DateTimeExtensions.UnixTimeNow() - startTime > TransactionReqTimeout)
                {
                    throw new RequestTimeoutException(
                        "A timeout has occured while waiting for your label to generate. Try retreiving the Transaction object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                transaction = RetrieveTransaction(object_id);
                object_status = (String)transaction.Status;
            }

            return transaction;
        }

        public Transaction RetrieveTransaction(String id)
        {
            string ep = String.Format("{0}/transactions/{1}", api_endpoint, id);
            return DoRequest<Transaction>(ep, "GET");
        }

        public ShippoCollection<Transaction> AllTransactions(Hashtable parameters)
        {
            string ep = String.Format("{0}/transactions?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Transaction>>(ep);
        }
        public async Task<Transaction> CreateTransactionAsync(Transaction parameters)
        {
            string ep = String.Format("{0}/transactions", api_endpoint);
            return await DoRequestAsync<Transaction>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<Transaction> CreateTransactionAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/transactions", api_endpoint);
            Transaction transaction = await DoRequestAsync<Transaction>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
            String object_id = (String)transaction.ObjectId;
            String object_status = (String)transaction.Status;
            long startTime = DateTimeExtensions.UnixTimeNow();

            while (object_status.Equals("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals("WAITING", StringComparison.OrdinalIgnoreCase))
            {
                if (DateTimeExtensions.UnixTimeNow() - startTime > TransactionReqTimeout)
                {
                    throw new RequestTimeoutException(
                        "A timeout has occured while waiting for your label to generate. Try retreiving the Transaction object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                transaction = await RetrieveTransactionAsync(object_id).ConfigureAwait(false);
                object_status = (String)transaction.Status;
            }

            return transaction;
        }

        public async Task<Transaction> RetrieveTransactionAsync(String id)
        {
            string ep = String.Format("{0}/transactions/{1}", api_endpoint, id);
            return await DoRequestAsync<Transaction>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<Transaction>> AllTransactionsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/transactions?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Transaction>>(ep).ConfigureAwait(false);
        }

        #endregion

        #region CustomsItem

        public CustomsItem CreateCustomsItem(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/items", api_endpoint);
            return DoRequest<CustomsItem>(ep, "POST", serialize(parameters));
        }

        public CustomsItem RetrieveCustomsItem(String id)
        {
            string ep = String.Format("{0}/customs/items/{1}", api_endpoint, id);
            return DoRequest<CustomsItem>(ep, "GET");
        }

        public ShippoCollection<CustomsItem> AllCustomsItems(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/items?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<CustomsItem>>(ep);
        }
        public async Task<CustomsItem> CreateCustomsItemAsync(CustomsItem parameters)
        {
            string ep = String.Format("{0}/customs/items", api_endpoint);
            return await DoRequestAsync<CustomsItem>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<CustomsItem> CreateCustomsItemAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/items", api_endpoint);
            return await DoRequestAsync<CustomsItem>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<CustomsItem> RetrieveCustomsItemAsync(String id)
        {
            string ep = String.Format("{0}/customs/items/{1}", api_endpoint, id);
            return await DoRequestAsync<CustomsItem>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<CustomsItem>> AllCustomsItemsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/items?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<CustomsItem>>(ep).ConfigureAwait(false);
        }


        #endregion

        #region CustomsDeclaration

        public CustomsDeclaration CreateCustomsDeclaration(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/declarations", api_endpoint);
            return DoRequest<CustomsDeclaration>(ep, "POST", serialize(parameters));
        }

        public CustomsDeclaration RetrieveCustomsDeclaration(String id)
        {
            string ep = String.Format("{0}/customs/declarations/{1}", api_endpoint, id);
            return DoRequest<CustomsDeclaration>(ep, "GET");
        }

        public ShippoCollection<CustomsDeclaration> AllCustomsDeclarations(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/declarations?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<CustomsDeclaration>>(ep);
        }
        public async Task<CustomsDeclaration> CreateCustomsDeclarationAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/declarations", api_endpoint);
            return await DoRequestAsync<CustomsDeclaration>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<CustomsDeclaration> CreateCustomsDeclarationAsync(CustomsDeclaration parameters)
        {
            string ep = String.Format("{0}/customs/declarations", api_endpoint);
            return await DoRequestAsync<CustomsDeclaration>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }


        public async Task<CustomsDeclaration> RetrieveCustomsDeclarationAsync(String id)
        {
            string ep = String.Format("{0}/customs/declarations/{1}", api_endpoint, id);
            return await DoRequestAsync<CustomsDeclaration>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<CustomsDeclaration>> AllCustomsDeclarationsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/customs/declarations?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<CustomsDeclaration>>(ep).ConfigureAwait(false);
        }


        #endregion

        #region CarrierAccount

        public CarrierAccount CreateCarrierAccount(Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts", api_endpoint);
            return DoRequest<CarrierAccount>(ep, "POST", serialize(parameters));
        }

        public CarrierAccount UpdateCarrierAccount(String object_id, Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return DoRequest<CarrierAccount>(ep, "PUT", serialize(parameters));
        }

        public CarrierAccount RetrieveCarrierAccount(String object_id)
        {
            string ep = String.Format("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return DoRequest<CarrierAccount>(ep, "GET");
        }

        public ShippoCollection<CarrierAccount> AllCarrierAccount(Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<CarrierAccount>>(ep);
        }

        public ShippoCollection<CarrierAccount> AllCarrierAccount()
        {
            string ep = String.Format("{0}/carrier_accounts", api_endpoint);
            return DoRequest<ShippoCollection<CarrierAccount>>(ep);
        }

        public async Task<CarrierAccount> CreateCarrierAccountAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts", api_endpoint);
            return await DoRequestAsync<CarrierAccount>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<CarrierAccount> CreateCarrierAccountAsync(CarrierAccount parameters)
        {
            string ep = String.Format("{0}/carrier_accounts", api_endpoint);
            return await DoRequestAsync<CarrierAccount>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<CarrierAccount> UpdateCarrierAccountAsync(String object_id, Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return await DoRequestAsync<CarrierAccount>(ep, "PUT", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<CarrierAccount> RetrieveCarrierAccountAsync(String object_id)
        {
            string ep = String.Format("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return await DoRequestAsync<CarrierAccount>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<CarrierAccount>> AllCarrierAccountAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/carrier_accounts?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<CarrierAccount>>(ep).ConfigureAwait(false);
        }

        public async Task<ShippoCollection<CarrierAccount>> AllCarrierAccountAsync()
        {
            string ep = String.Format("{0}/carrier_accounts", api_endpoint);
            return await DoRequestAsync<ShippoCollection<CarrierAccount>>(ep).ConfigureAwait(false);
        }


        #endregion

        #region Refund

        public Refund CreateRefund(Hashtable parameters)
        {
            string ep = String.Format("{0}/refunds", api_endpoint);
            return DoRequest<Refund>(ep, "POST", serialize(parameters));
        }

        public Refund RetrieveRefund(String id)
        {
            string ep = String.Format("{0}/refunds/{1}", api_endpoint, id);
            return DoRequest<Refund>(ep, "GET");
        }

        public ShippoCollection<Refund> AllRefunds(Hashtable parameters)
        {
            string ep = String.Format("{0}/refunds?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Refund>>(ep);
        }
        public async Task<Refund> CreateRefundAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/refunds", api_endpoint);
            return await DoRequestAsync<Refund>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Refund> CreateRefundAsync(Refund parameters)
        {
            string ep = String.Format("{0}/refunds", api_endpoint);
            return await DoRequestAsync<Refund>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }


        public async Task<Refund> RetrieveRefundAsync(String id)
        {
            string ep = String.Format("{0}/refunds/{1}", api_endpoint, id);
            return await DoRequestAsync<Refund>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<Refund>> AllRefundsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/refunds?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Refund>>(ep).ConfigureAwait(false);
        }

        #endregion

        #region Manifest

        public Manifest CreateManifest(Hashtable parameters)
        {
            string ep = String.Format("{0}/manifests", api_endpoint);
            return DoRequest<Manifest>(ep, "POST", serialize(parameters));
        }

        public Manifest RetrieveManifest(String id)
        {
            string ep = String.Format("{0}/manifests/{1}", api_endpoint, id);
            return DoRequest<Manifest>(ep, "GET");
        }

        public ShippoCollection<Manifest> AllManifests(Hashtable parameters)
        {
            string ep = String.Format("{0}/manifests?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return DoRequest<ShippoCollection<Manifest>>(ep);
        }
        public async Task<Manifest> CreateManifestAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/manifests", api_endpoint);
            return await DoRequestAsync<Manifest>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Manifest> CreateManifestAsync(Manifest parameters)
        {
            string ep = String.Format("{0}/manifests", api_endpoint);
            return await DoRequestAsync<Manifest>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Manifest> RetrieveManifestAsync(String id)
        {
            string ep = String.Format("{0}/manifests/{1}", api_endpoint, id);
            return await DoRequestAsync<Manifest>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<ShippoCollection<Manifest>> AllManifestsAsync(Hashtable parameters)
        {
            string ep = String.Format("{0}/manifests?{1}", api_endpoint, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<ShippoCollection<Manifest>>(ep).ConfigureAwait(false);
        }


        #endregion

        #region Batch

        public Batch CreateBatch(String carrierAccount, String servicelevelToken, ShippoEnums.LabelFiletypes labelFiletype,
                                  String metadata, List<BatchShipment> batchShipments)
        {
            string ep = String.Format("{0}/batches", api_endpoint);
            Hashtable parameters = new Hashtable();
            parameters.Add("default_carrier_account", carrierAccount);
            parameters.Add("default_servicelevel_token", servicelevelToken);
            if (labelFiletype != ShippoEnums.LabelFiletypes.NONE)
                parameters.Add("label_filetype", labelFiletype);
            parameters.Add("metadata", metadata);
            parameters.Add("batch_shipments", batchShipments);
            return DoRequest<Batch>(ep, "POST", serialize(parameters));
        }

        public Batch RetrieveBatch(String id, uint page, ShippoEnums.ObjectResults objectResults)
        {
            string ep = String.Format("{0}/batches/{1}", api_endpoint, WebUtility.HtmlEncode(id));
            Hashtable parameters = new Hashtable();
            if (page > 0)
                parameters.Add("page", page);
            if (objectResults != ShippoEnums.ObjectResults.none)
                parameters.Add("object_results", objectResults);
            if (parameters.Count != 0)
                ep = String.Format("{0}?{1}", ep, generateURLEncodedFromHashmap(parameters));
            return DoRequest<Batch>(ep, "GET");
        }

        public Batch AddShipmentsToBatch(String id, List<String> shipmentIds)
        {
            string ep = String.Format("{0}/batches/{1}/add_shipments", api_endpoint, WebUtility.HtmlEncode(id));
            List<Hashtable> shipments = new List<Hashtable>();
            foreach (String shipmentId in shipmentIds)
            {
                Hashtable shipmentTable = new Hashtable();
                shipmentTable.Add("shipment", shipmentId);
                shipments.Add(shipmentTable);
            }

            return DoRequest<Batch>(ep, "POST", serialize(shipments));
        }

        public Batch RemoveShipmentsFromBatch(String id, List<String> shipmentIds)
        {
            string ep = String.Format("{0}/batches/{1}/remove_shipments", api_endpoint, WebUtility.HtmlEncode(id));
            return DoRequest<Batch>(ep, "POST", serialize(shipmentIds));
        }

        public Batch PurchaseBatch(String id)
        {
            string ep = String.Format("{0}/batches/{1}/purchase", api_endpoint, WebUtility.HtmlEncode(id));
            return DoRequest<Batch>(ep, "POST");
        }
        public async Task<Batch> CreateBatchAsync(String carrierAccount, String servicelevelToken, ShippoEnums.LabelFiletypes labelFiletype,
                                  String metadata, List<BatchShipment> batchShipments)
        {
            string ep = String.Format("{0}/batches", api_endpoint);
            Hashtable parameters = new Hashtable();
            parameters.Add("default_carrier_account", carrierAccount);
            parameters.Add("default_servicelevel_token", servicelevelToken);
            if (labelFiletype != ShippoEnums.LabelFiletypes.NONE)
                parameters.Add("label_filetype", labelFiletype);
            parameters.Add("metadata", metadata);
            parameters.Add("batch_shipments", batchShipments);
            return await DoRequestAsync<Batch>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }

        public async Task<Batch> RetrieveBatchAsync(String id, uint page, ShippoEnums.ObjectResults objectResults)
        {
            string ep = String.Format("{0}/batches/{1}", api_endpoint, WebUtility.HtmlEncode(id));
            Hashtable parameters = new Hashtable();
            if (page > 0)
                parameters.Add("page", page);
            if (objectResults != ShippoEnums.ObjectResults.none)
                parameters.Add("object_results", objectResults);
            if (parameters.Count != 0)
                ep = String.Format("{0}?{1}", ep, generateURLEncodedFromHashmap(parameters));
            return await DoRequestAsync<Batch>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<Batch> AddShipmentsToBatchAsync(String id, List<String> shipmentIds)
        {
            string ep = String.Format("{0}/batches/{1}/add_shipments", api_endpoint, WebUtility.HtmlEncode(id));
            List<Hashtable> shipments = new List<Hashtable>();
            foreach (String shipmentId in shipmentIds)
            {
                Hashtable shipmentTable = new Hashtable();
                shipmentTable.Add("shipment", shipmentId);
                shipments.Add(shipmentTable);
            }

            return await DoRequestAsync<Batch>(ep, "POST", serialize(shipments)).ConfigureAwait(false);
        }

        public async Task<Batch> RemoveShipmentsFromBatchAsync(String id, List<String> shipmentIds)
        {
            string ep = String.Format("{0}/batches/{1}/remove_shipments", api_endpoint, WebUtility.HtmlEncode(id));
            return await DoRequestAsync<Batch>(ep, "POST", serialize(shipmentIds)).ConfigureAwait(false);
        }

        public async Task<Batch> PurchaseBatchAsync(String id)
        {
            string ep = String.Format("{0}/batches/{1}/purchase", api_endpoint, WebUtility.HtmlEncode(id));
            return await DoRequestAsync<Batch>(ep, "POST").ConfigureAwait(false);
        }


        #endregion

        #region Track

        public Track RetrieveTracking(String carrier, String id)
        {
            string encodedCarrier = WebUtility.HtmlEncode(carrier);
            string encodedId = WebUtility.HtmlEncode(id);
            string ep = String.Format("{0}/tracks/{1}/{2}", api_endpoint, encodedCarrier, encodedId);
            return DoRequest<Track>(ep, "GET");
        }

        public Track RegisterTrackingWebhook(Hashtable parameters)
        {
            // For now the trailing '/' is required.
            string ep = String.Format("{0}/tracks/", api_endpoint);
            return DoRequest<Track>(ep, "POST", serialize(parameters));
        }
        public async Task<Track> RetrieveTrackingAsync(String carrier, String id)
        {
            string encodedCarrier = WebUtility.HtmlEncode(carrier);
            string encodedId = WebUtility.HtmlEncode(id);
            string ep = String.Format("{0}/tracks/{1}/{2}", api_endpoint, encodedCarrier, encodedId);
            return await DoRequestAsync<Track>(ep, "GET").ConfigureAwait(false);
        }

        public async Task<Track> RegisterTrackingWebhookAsync(Hashtable parameters)
        {
            // For now the trailing '/' is required.
            string ep = String.Format("{0}/tracks/", api_endpoint);
            return await DoRequestAsync<Track>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }
        public async Task<Track> RegisterTrackingWebhookAsync(Track parameters)
        {
            // For now the trailing '/' is required.
            string ep = String.Format("{0}/tracks/", api_endpoint);
            return await DoRequestAsync<Track>(ep, "POST", serialize(parameters)).ConfigureAwait(false);
        }


        #endregion

        public int TimeoutSeconds { get; set; }
    }
}
