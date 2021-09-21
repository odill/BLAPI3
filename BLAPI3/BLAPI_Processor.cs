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
    class BLAPI_Processor
    {
        public int oauth1_last_timestamp  // property
        { get; set; }

        public BLAPI_Processor(int timestamp)
        {
            oauth1_last_timestamp = timestamp;
        }
        private static string EscapeUriDataStringRfc3986(string s)
        {
            var charsToEscape = new[] { "!", "*", "'", "(", ")" };
            var escaped = new StringBuilder(Uri.EscapeDataString(s));
            foreach (var t in charsToEscape)
            {
                escaped.Replace(t, Uri.HexEscape(t[0]));
            }
            return escaped.ToString();
        }

        private HttpWebResponse send_http_oauth1_request(string url, string parameters,
                                            string consumerkey, string consumersecret,
                                            string tokenvalue, string tokensecret,
                                            string timestamp, string signature)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + parameters);
            httpWebRequest.Method = "GET";

            var key = EscapeUriDataStringRfc3986(consumersecret) + "&" + EscapeUriDataStringRfc3986(tokensecret);
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
            signatureString = EscapeUriDataStringRfc3986(signatureString);

            string SimpleQuote(string s) => '"' + s + '"';
            var header =
                "OAuth realm=" + SimpleQuote("") + "," +
                "oauth_consumer_key=" + SimpleQuote(consumerkey) + "," +
                "oauth_nonce=" + SimpleQuote(nonce) + "," +
                "oauth_signature_method=" + SimpleQuote("HMAC-SHA1") + "," +
                "oauth_timestamp=" + SimpleQuote(timestamp) + "," +
                "oauth_token=" + SimpleQuote(tokenvalue) + "," +
                "oauth_version=" + SimpleQuote("1.0") + "," +
                "oauth_signature= " + SimpleQuote(signatureString);
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, header);

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            return response;
        }

        private string Bl_order_cost(string order_id)
        {
            string my_cost = "0";
            string consumerKey = Properties.Settings.Default.ConsumerKey;
            string consumerSecret = Properties.Settings.Default.ConsumerSecret;
            string tokenSecret = Properties.Settings.Default.TokenSecret;
            string tokenValue = Properties.Settings.Default.TokenValue;

            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id + "/items";
            oauth1_last_timestamp++;

            var timeStamp = oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = EscapeUriDataStringRfc3986("GET") + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(url.ToLower()) + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(
                "oauth_consumer_key=" + EscapeUriDataStringRfc3986(consumerKey) + "&" +
                "oauth_nonce=" + EscapeUriDataStringRfc3986(nonce) + "&" +
                "oauth_signature_method=" + EscapeUriDataStringRfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + EscapeUriDataStringRfc3986(timeStamp) + "&" +
                "oauth_token=" + EscapeUriDataStringRfc3986(tokenValue) + "&" +
                "oauth_version=" + EscapeUriDataStringRfc3986("1.0"));

            var response = send_http_oauth1_request(url, "",
                                        consumerKey, consumerSecret,
                                        tokenValue, tokenSecret,
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
                //(1.7289).ToString("#.##")
                //string.Format("{0:0.00}", number)
                my_cost = string.Format("{0:0.00}", my_cost_all_lots);
            }
            return my_cost;
        }

        public void BL_set_order_status(string order_id, string status)
        {
            string consumerKey = Properties.Settings.Default.ConsumerKey;
            string consumerSecret = Properties.Settings.Default.ConsumerSecret;
            string tokenSecret = Properties.Settings.Default.TokenSecret;
            string tokenValue = Properties.Settings.Default.TokenValue;
            
            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id +"/status";

            oauth1_last_timestamp++;
            var timeStamp = oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = EscapeUriDataStringRfc3986("PUT") + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(url.ToLower()) + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(
                "oauth_consumer_key=" + EscapeUriDataStringRfc3986(consumerKey) + "&" +
                "oauth_nonce=" + EscapeUriDataStringRfc3986(nonce) + "&" +
                "oauth_signature_method=" + EscapeUriDataStringRfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + EscapeUriDataStringRfc3986(timeStamp) + "&" +
                "oauth_token=" + EscapeUriDataStringRfc3986(tokenValue) + "&" +
                "oauth_version=" + EscapeUriDataStringRfc3986("1.0"));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "PUT";
            httpWebRequest.ContentType = "application/json";
            string body_s = "{\"field\" : \"status\", \"value\" : \"PACKED\"}";
            var body_b = Encoding.Default.GetBytes(body_s);

            httpWebRequest.ContentLength = body_b.Length;

            var bodyStream = httpWebRequest.GetRequestStream();
            bodyStream.Write(body_b, 0, body_b.Length);
            bodyStream.Close();

            var key = EscapeUriDataStringRfc3986(consumerSecret) + "&" + EscapeUriDataStringRfc3986(tokenSecret);

            var signatureEncoding = new ASCIIEncoding();
            var keyBytes = signatureEncoding.GetBytes(key);
            var signatureBaseBytes = signatureEncoding.GetBytes(signatureBaseString);
            string signatureString;
            using (var hmacsha1 = new HMACSHA1(keyBytes))
            {
                var hashBytes = hmacsha1.ComputeHash(signatureBaseBytes);
                signatureString = Convert.ToBase64String(hashBytes);
            }
            signatureString = EscapeUriDataStringRfc3986(signatureString);

            string SimpleQuote(string s) => '"' + s + '"';
            var header =
                "OAuth realm=" + SimpleQuote("") + "," +
                "oauth_consumer_key=" + SimpleQuote(consumerKey) + "," +
                "oauth_nonce=" + SimpleQuote(nonce) + "," +
                "oauth_signature_method=" + SimpleQuote("HMAC-SHA1") + "," +
                "oauth_timestamp=" + SimpleQuote(timeStamp) + "," +
                "oauth_token=" + SimpleQuote(tokenValue) + "," +
                "oauth_version=" + SimpleQuote("1.0") + "," +
                "oauth_signature= " + SimpleQuote(signatureString);
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, header);

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

        }

        private string Bl_order_info(string order_id)
        {
            string consumerKey = Properties.Settings.Default.ConsumerKey;
            string consumerSecret = Properties.Settings.Default.ConsumerSecret;
            string tokenSecret = Properties.Settings.Default.TokenSecret;
            string tokenValue = Properties.Settings.Default.TokenValue;

            string url = "https://api.bricklink.com/api/store/v1/orders/" + order_id;

            oauth1_last_timestamp++;
            var timeStamp = oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = EscapeUriDataStringRfc3986("GET") + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(url.ToLower()) + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(
                "oauth_consumer_key=" + EscapeUriDataStringRfc3986(consumerKey) + "&" +
                "oauth_nonce=" + EscapeUriDataStringRfc3986(nonce) + "&" +
                "oauth_signature_method=" + EscapeUriDataStringRfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + EscapeUriDataStringRfc3986(timeStamp) + "&" +
                "oauth_token=" + EscapeUriDataStringRfc3986(tokenValue) + "&" +
                "oauth_version=" + EscapeUriDataStringRfc3986("1.0"));

            var response = send_http_oauth1_request(url, "",
                                        consumerKey, consumerSecret,
                                        tokenValue, tokenSecret,
                                        timeStamp, signatureBaseString);

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

        private IEnumerable<string> Bl_orders_id_list()
        {
            string consumerKey = Properties.Settings.Default.ConsumerKey;
            string consumerSecret = Properties.Settings.Default.ConsumerSecret;
            string tokenSecret = Properties.Settings.Default.TokenSecret;
            string tokenValue = Properties.Settings.Default.TokenValue;

            const string url = "https://api.bricklink.com/api/store/v1/orders";
            string param1 = "direction=in";
            string param2 = "status=paid";

            oauth1_last_timestamp++;
            var timeStamp = oauth1_last_timestamp.ToString();
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(timeStamp));

            var signatureBaseString = EscapeUriDataStringRfc3986("GET") + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(url.ToLower()) + "&";
            signatureBaseString += EscapeUriDataStringRfc3986(
                param1 + "&" +
                "oauth_consumer_key=" + EscapeUriDataStringRfc3986(consumerKey) + "&" +
                "oauth_nonce=" + EscapeUriDataStringRfc3986(nonce) + "&" +
                "oauth_signature_method=" + EscapeUriDataStringRfc3986("HMAC-SHA1") + "&" +
                "oauth_timestamp=" + EscapeUriDataStringRfc3986(timeStamp) + "&" +
                "oauth_token=" + EscapeUriDataStringRfc3986(tokenValue) + "&" +
                "oauth_version=" + EscapeUriDataStringRfc3986("1.0") + "&" + param2);


            var response = send_http_oauth1_request(url, "?direction=in&status=paid",
                                        consumerKey, consumerSecret,
                                        tokenValue, tokenSecret,
                                        timeStamp, signatureBaseString);

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

        public string Bl_orders_full_list()
        {
            var first = true;
            string full_list_json = "{ \"data\": [";

            var order_id_list = Bl_orders_id_list();
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
