using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Kucoin
{
    public partial class Form2 : Form
    {
        
        public Form2()
        {
            InitializeComponent();
        }
        public string symbol { get; set; }
        public string secret { get; set; }
        public string key { get; set; }
        public string pass { get; set; }
        public string price { get; set; }
        public string amount { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            symbol = textBox1.Text.ToUpper();
            price = textBox2.Text;
            amount = textBox3.Text;


            //post request
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            double CurrentTime = (DateTimeOffset.Now - epoch).TotalMilliseconds;

            string uri = "/api/v1/orders";

            byte[] keyByte = Encoding.ASCII.GetBytes(secret);
            byte[] passBytes = Encoding.ASCII.GetBytes(pass);

            var hmac = new HMACSHA256(keyByte);
            byte[] hashpass = hmac.ComputeHash(passBytes);
            var passphrase = Convert.ToBase64String(hashpass);


            IDictionary<string, object> reqParams = new Dictionary<string, object>()
            {
                { "clientOid", "1" },
                { "side", "buy" },
                { "symbol", symbol },
                { "price", price },
                { "size", amount }

            };
            string json_data = reqParams != null ? JsonConvert.SerializeObject(reqParams) : "";

            var Content = new StringContent(json_data, Encoding.UTF8, "application/json");
            string token = uri + json_data;
            var stringToSign = ((long)CurrentTime).ToString() + "POST" + token;




            byte[] messageBytes = Encoding.ASCII.GetBytes(stringToSign);

            var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            var signatureResult = Convert.ToBase64String(hashmessage);



            using (var client = new HttpClient())
            {
                var endpoint = new Uri("https://api.kucoin.com/api/v1/orders");
                client.DefaultRequestHeaders.Add("KC-API-KEY", key);
                client.DefaultRequestHeaders.Add("KC-API-TIMESTAMP", ((long)CurrentTime).ToString());
                client.DefaultRequestHeaders.Add("KC-API-SIGN", signatureResult);
                client.DefaultRequestHeaders.Add("KC-API-PASSPHRASE", passphrase);
                client.DefaultRequestHeaders.Add("KC-API-KEY-VERSION", "2");
                Console.WriteLine(signatureResult);
                var result = client.PostAsync(endpoint, Content).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                MessageBox.Show(json);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            symbol = textBox1.Text.ToUpper();
            price = textBox2.Text;
            amount = textBox3.Text;


            //get request
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            double CurrentTime = (DateTimeOffset.Now - epoch).TotalMilliseconds;
            string uri = string.Format("/api/v1/orders?status=active&symbol={0}&price={1}", this.symbol, this.price); 
            byte[] keyByte = Encoding.ASCII.GetBytes(secret);
            byte[] passBytes = Encoding.ASCII.GetBytes(pass);

            var hmac = new HMACSHA256(keyByte);
            byte[] hashpass = hmac.ComputeHash(passBytes);
            var passphrase = Convert.ToBase64String(hashpass);

            
            string token = uri;

            var stringToSign = ((long)CurrentTime).ToString() + "GET" + token;

            byte[] messageBytes = Encoding.ASCII.GetBytes(stringToSign);

            var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            var signatureResult = Convert.ToBase64String(hashmessage);

            var client = new HttpClient();
            var endpoint = new Uri("https://api.kucoin.com" + uri);
            client.DefaultRequestHeaders.Add("KC-API-KEY", key);
            client.DefaultRequestHeaders.Add("KC-API-TIMESTAMP", ((long)CurrentTime).ToString());
            client.DefaultRequestHeaders.Add("KC-API-SIGN", signatureResult);
            client.DefaultRequestHeaders.Add("KC-API-PASSPHRASE", passphrase);
            client.DefaultRequestHeaders.Add("KC-API-KEY-VERSION", "2");
            var result = client.GetAsync(endpoint).Result;
            var json = result.Content.ReadAsStringAsync().Result;

            //поиск id 
            string id = "\"id\"";
            char end = '"';
            int indexOfChar = json.IndexOf(id); // равно 4

            id = "";
            int i = indexOfChar + 6;
            while (json[i] != end)
            {
                id += json[i];
                i++;
            }
            

           
            

            //delete request
            CurrentTime = (DateTimeOffset.Now - epoch).TotalMilliseconds;

            uri = string.Format("/api/v1/orders/{0}", id);

            keyByte = Encoding.ASCII.GetBytes(secret);
            passBytes = Encoding.ASCII.GetBytes(pass);

            hmac = new HMACSHA256(keyByte);
            hashpass = hmac.ComputeHash(passBytes);
            passphrase = Convert.ToBase64String(hashpass);


            token = uri;

            stringToSign = ((long)CurrentTime).ToString() + "DELETE" + token;

            messageBytes = Encoding.ASCII.GetBytes(stringToSign);

            hmacsha256 = new HMACSHA256(keyByte);
            hashmessage = hmacsha256.ComputeHash(messageBytes);
            signatureResult = Convert.ToBase64String(hashmessage);

            client = new HttpClient();
            endpoint = new Uri("https://api.kucoin.com" + uri);
            client.DefaultRequestHeaders.Add("KC-API-KEY", key);
            client.DefaultRequestHeaders.Add("KC-API-TIMESTAMP", ((long)CurrentTime).ToString());
            client.DefaultRequestHeaders.Add("KC-API-SIGN", signatureResult);
            client.DefaultRequestHeaders.Add("KC-API-PASSPHRASE", passphrase);
            client.DefaultRequestHeaders.Add("KC-API-KEY-VERSION", "2");
            result = client.DeleteAsync(endpoint).Result;
            json = result.Content.ReadAsStringAsync().Result;
            MessageBox.Show(json);
                
            


        }
    }
}
