using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BLAPI3
{
    enum BLAPI_Processor_status
    {
        BLAPI_Processor_PAID,
        BLAPI_Processor_PACKED
    }

    class BLAPI_Processor
    {
        private int bl_oauth1_last_timestamp;
        private string bl_consumer_key;
        private string bl_consumer_secret;
        private string bl_token_secret;
        private string bl_token_value;

        public BLAPI_Processor(int timestamp)
        {
            bl_oauth1_last_timestamp = timestamp;

            //initialize OAuth1 secrets and values
            BL_set_oauth1();
        }

        public void BL_set_oauth1()
        {
            //initialize OAuth1 secrets and values
            bl_consumer_key = Properties.Settings.Default.ConsumerKey;
            bl_consumer_secret = Properties.Settings.Default.ConsumerSecret;
            bl_token_secret = Properties.Settings.Default.TokenSecret;
            bl_token_value = Properties.Settings.Default.TokenValue;
        }

        private static string BL_escape_uri_rfc3986(string s)
        {
            var charsToEscape = new[] { "!", "*", "'", "(", ")" };
            var escaped = new StringBuilder(Uri.EscapeDataString(s));
            foreach (var t in charsToEscape)
            {
                escaped.Replace(t, Uri.HexEscape(t[0]));
            }
            return escaped.ToString();
        }

        private HttpWebResponse BL_HTTP_put_oauth1_request(string url, string parameters,
                                    string timestamp, string signature, string body)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + parameters);
            httpWebRequest.Method = "PUT";
            httpWebRequest.ContentType = "application/json";
            var body_b = Encoding.Default.GetBytes(body);

            httpWebRequest.ContentLength = body_b.Length;

            var bodyStream = httpWebRequest.GetRequestStream();
            bodyStream.Write(body_b, 0, body_b.Length);
            bodyStream.Close();

            var key = BL_escape_uri_rfc3986(bl_consumer_secret) + "&" + BL_escape_uri_rfc3986(bl_token_secret);
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timestamp));

            var signatureEncoding = new ASCIIEncoding();
            var keyBytes = signatureEncoding.GetBytes(key);
            var signatureBaseBytes = signatureEncoding.GetBytes(signature);
            string signatureString;
            using (var hmacsha1 = new HMACSHA1(keyBytes))
            {
                var hashBytes = hmacsha1.ComputeHash(signatureBaseBytes);
                signatureString = Convert.ToBase64String(hashBytes);
            }
            signatureString = BL_escape_uri_rfc3986(signatureString);

            string SimpleQuote(string s) => '"' + s + '"';
            var header =
                "OAuth realm=" + SimpleQuote("") + "," +
                "oauth_consumer_key=" + SimpleQuote(bl_consumer_key) + "," +
                "oauth_nonce=" + SimpleQuote(nonce) + "," +
                "oauth_signature_method=" + SimpleQuote("HMAC-SHA1") + "," +
                "oauth_timestamp=" + SimpleQuote(timestamp) + "," +
                "oauth_token=" + SimpleQuote(bl_token_value) + "," +
                "oauth_version=" + SimpleQuote("1.0") + "," +
                "oauth_signature= " + SimpleQuote(signatureString);
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, header);

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            return response;
        }

        private HttpWebResponse BL_HTTP_get_oauth1_request(string url, string parameters,
                                            string timestamp, string signature)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + parameters);
            httpWebRequest.Method = "GET";

            var key = BL_escape_uri_rfc3986(bl_consumer_secret) + "&" + BL_escape_uri_rfc3986(bl_token_secret);
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timestamp));

            var signatureEncoding = new ASCIIEncoding();
            var keyBytes = signatureEncoding.GetBytes(key);
            var signatureBaseBytes = signatureEncoding.GetBytes(signature);
            string signatureString;
            using (var hmacsha1 = new HMACSHA1(keyBytes))
            {
                var hashBytes = hmacsha1.ComputeHash(signatureBaseBytes);
                signatureString = Convert.ToBase64String(hashBytes);
            }
            signatureString = BL_escape_uri_rfc3986(signatureString);

            string SimpleQuote(string s) => '"' + s + '"';
            var header =
                "OAuth realm=" + SimpleQuote("") + "," +
                "oauth_consumer_key=" + SimpleQuote(bl_consumer_key) + "," +
                "oauth_nonce=" + SimpleQuote(nonce) + "," +
                "oauth_signature_method=" + SimpleQuote("HMAC-SHA1") + "," +
                "oauth_timestamp=" + SimpleQuote(timestamp) + "," +
                "oauth_token=" + SimpleQuote(bl_token_value) + "," +
                "oauth_version=" + SimpleQuote("1.0") + "," +
                "oauth_signature= " + SimpleQuote(signatureString);
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, header);

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            return response;
        }


        private string Bl_order_cost(string order_id)
        {
            string my_cost = "0";

            //Got to read information about each lot in the order to calculate seller's cost
            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id + "/items";
            bl_oauth1_last_timestamp++;

            var timeStamp = bl_oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = BL_escape_uri_rfc3986("GET") + "&";
            signatureBaseString += BL_escape_uri_rfc3986(url.ToLower()) + "&";
            signatureBaseString += BL_escape_uri_rfc3986(
                "oauth_consumer_key=" + BL_escape_uri_rfc3986(bl_consumer_key) + "&" +
                "oauth_nonce=" + BL_escape_uri_rfc3986(nonce) + "&" +
                "oauth_signature_method=" + BL_escape_uri_rfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + BL_escape_uri_rfc3986(timeStamp) + "&" +
                "oauth_token=" + BL_escape_uri_rfc3986(bl_token_value) + "&" +
                "oauth_version=" + BL_escape_uri_rfc3986("1.0"));

            var response = BL_HTTP_get_oauth1_request(url, "",
                                        timeStamp, signatureBaseString);

            var characterSet = ((HttpWebResponse)response).CharacterSet;
            var responsestream = response.GetResponseStream();
            var responseEncoding = characterSet == ""
               ? Encoding.UTF8
               : Encoding.GetEncoding(characterSet ?? "utf-8");

            using (responsestream)
            {
                var reader = new StreamReader(responsestream, responseEncoding);
                var result = reader.ReadToEnd();

                //deserialize 
                JObject json = JObject.Parse(result);
                JArray batches_arr = (JArray)json["data"];
                double my_cost_all_lots = 0;
                foreach (var item_arr in batches_arr)
                {
                    foreach (var item in item_arr)
                    {
                        double cost_per_lot = (double)item["order_cost"];
                        int count_per_lot = (int)item["quantity"];
                        my_cost_all_lots += cost_per_lot * count_per_lot;
                    }
                }
                my_cost = string.Format("{0:0.00}", my_cost_all_lots);
            }
            return my_cost;
        }

        public void BL_set_order_status(string order_id, BLAPI_Processor_status status)
        {
            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id +"/status";

            bl_oauth1_last_timestamp++;
            var timeStamp = bl_oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = BL_escape_uri_rfc3986("PUT") + "&";
            signatureBaseString += BL_escape_uri_rfc3986(url.ToLower()) + "&";
            signatureBaseString += BL_escape_uri_rfc3986(
                "oauth_consumer_key=" + BL_escape_uri_rfc3986(bl_consumer_key) + "&" +
                "oauth_nonce=" + BL_escape_uri_rfc3986(nonce) + "&" +
                "oauth_signature_method=" + BL_escape_uri_rfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + BL_escape_uri_rfc3986(timeStamp) + "&" +
                "oauth_token=" + BL_escape_uri_rfc3986(bl_token_value) + "&" +
                "oauth_version=" + BL_escape_uri_rfc3986("1.0"));

            string body_s = "";
            if (status == BLAPI_Processor_status.BLAPI_Processor_PACKED)
            {
                body_s = "{\"field\" : \"status\", \"value\" : \"PACKED\"}";
            }
            var response = BL_HTTP_put_oauth1_request(url, "",
                                        timeStamp, signatureBaseString, body_s);
            //Do not process response. 
            //TO DO: add response status code processing
        }

        private string Bl_order_info(string order_id)
        {
            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id;

            bl_oauth1_last_timestamp++;
            var timeStamp = bl_oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = BL_escape_uri_rfc3986("GET") + "&";
            signatureBaseString += BL_escape_uri_rfc3986(url.ToLower()) + "&";
            signatureBaseString += BL_escape_uri_rfc3986(
                "oauth_consumer_key=" + BL_escape_uri_rfc3986(bl_consumer_key) + "&" +
                "oauth_nonce=" + BL_escape_uri_rfc3986(nonce) + "&" +
                "oauth_signature_method=" + BL_escape_uri_rfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + BL_escape_uri_rfc3986(timeStamp) + "&" +
                "oauth_token=" + BL_escape_uri_rfc3986(bl_token_value) + "&" +
                "oauth_version=" + BL_escape_uri_rfc3986("1.0"));

            var response = BL_HTTP_get_oauth1_request(url, "",
                                        timeStamp, signatureBaseString);
            //Process response, build JSON object about this order
            var characterSet = ((HttpWebResponse)response).CharacterSet;
            var responsestream = response.GetResponseStream();
            var responseEncoding = characterSet == ""
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(characterSet ?? "utf-8");

            string json_response = "";
            using (responsestream)
            {
                var reader = new StreamReader(responsestream, responseEncoding);
                var result = reader.ReadToEnd();

                //deserialize 
                JObject json = JObject.Parse(result);
                string name = json["data"]["shipping"]["address"]["name"]["full"].ToString();
                string total = json["data"]["disp_cost"]["grand_total"].ToString();
                float total_f = float.Parse(total, CultureInfo.InvariantCulture.NumberFormat);
                total = string.Format("{0:0.00}", total_f);

                //calculate sellers cost
                string cost = Bl_order_cost(order_id);

                json_response = "{ \"name\":\"" + name + "\", \"total\":\"" + total
                           + "\", \"my_cost\":\"" + cost + "\", \"order_id\":\"" + order_id + "\"}";

            }
            return json_response;
        }

        private IEnumerable<string> Bl_orders_id_list(BLAPI_Processor_status status)
        {
            //returns list of received order_id in requested status
            const string url = "https://api.bricklink.com/api/store/v1/orders";
            string param1 = "direction=in";
            string param2 = "";
            if (status == BLAPI_Processor_status.BLAPI_Processor_PAID)
            {
                param2 = "status=paid";
            }
            if (status == BLAPI_Processor_status.BLAPI_Processor_PACKED)
            {
                param2 = "status=packed";
            }

            bl_oauth1_last_timestamp++;
            var timestamp = bl_oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timestamp));

            //calculate signature using nonce
            var signatureBaseString = BL_escape_uri_rfc3986("GET") + "&";
            signatureBaseString += BL_escape_uri_rfc3986(url.ToLower()) + "&";
            signatureBaseString += BL_escape_uri_rfc3986(
                param1 + "&" +
                "oauth_consumer_key=" + BL_escape_uri_rfc3986(bl_consumer_key) + "&" +
                "oauth_nonce=" + BL_escape_uri_rfc3986(nonce) + "&" +
                "oauth_signature_method=" + BL_escape_uri_rfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + BL_escape_uri_rfc3986(timestamp) + "&" +
                "oauth_token=" + BL_escape_uri_rfc3986(bl_token_value) + "&" +
                "oauth_version=" + BL_escape_uri_rfc3986("1.0") + "&" + param2);


            var response = BL_HTTP_get_oauth1_request(url, "?direction=in&" + param2,
                                        timestamp, signatureBaseString);
            //Process response. Extract "order_id" data from JSON into strings list
            var characterSet = ((HttpWebResponse)response).CharacterSet;
            var responsestream = response.GetResponseStream();
            var responseEncoding = characterSet == ""
               ? Encoding.UTF8
               : Encoding.GetEncoding(characterSet ?? "utf-8");

            IEnumerable<string> orders;
            using (responsestream)
            {
                var reader = new StreamReader(responsestream, responseEncoding);
                var result = reader.ReadToEnd();

                //deserialize 
                JObject json = JObject.Parse(result);
                orders =
                    from token in json["data"]
                    select (string)token["order_id"];
            }
            return orders;
        }

        public string Bl_orders_full_list(BLAPI_Processor_status status)
        {
            var first = true;

            //Build JSON string with information for all orders with requested status
            string full_list_json = "{ \"data\": [";

            var order_id_list = Bl_orders_id_list(status);
            foreach (var order_id in order_id_list)
            {
                if (!first)
                {
                    full_list_json += ",";
                }
                first = false;
                string order_info = Bl_order_info(order_id);
                full_list_json += order_info;
            }

            full_list_json += "]}";
            return full_list_json;
        }
    }
}
