using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CryptoAPI
{
    public static class Extra
    {
        public static async Task<BitmapImage> BtnLoadFromFileAsync(string coinName)
        {
            return await Task.FromResult(new BitmapImage(new Uri($"https://coinlib.io/static/img/coins/small/{coinName.ToLower()}.png")));
        }

        public static void OpenProcess(string URL)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            process.StartInfo.Arguments = URL + " --new-window";
            process.Start();
        }

        public static string SendSlackMessage(string error)
        {
            string errormessage = DateTime.Now + ": " + error;
            SlackClient client = new SlackClient("https://hooks.slack.com/services/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");
            client.PostMessage(username: "CryptoAPI", text: errormessage, channel: "#cryptoapi-errors");
            return "It seems like you encountered a issue. Dont worry though its been reported";
        }
    }

    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public void PostMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["payload"] = payloadJson;
                var response = client.UploadValues(_uri, "POST", data);
                string responseText = _encoding.GetString(response);
            }
        }
    }

    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}