using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace CryptoAPI
{
    public partial class MainApp : Window
    {
        public static string PublicKey, SecretKey, ID;
        private static dynamic AllcoinjsonFile;

        public MainApp(string tPublicKey, string tSecretKey, string tID)
        {
            PublicKey = tPublicKey;
            SecretKey = tSecretKey;
            ID = tID;
            InitializeComponent();
            Statpanel.Opacity = 0;
            WelcomeUser();
        }

        private async void WelcomeUser()
        {
            Welcometxt.Text = $"Welcome {ID.ToLowerInvariant()}!";
            AllcoinjsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Balances"]));
            int AmountOfCoins = 0;
            float CoinsPrice = 0;
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    string[] file = ((AllcoinjsonFile["balances"][i]).ToString()).Split('"');
                    CoinsPrice = AllcoinjsonFile["balances"][i][file[1]]["audbalance"] + CoinsPrice;
                    AmountOfCoins++;
                }
                catch { break; }
            }
            PortTotal.Text = $"You currently own ${Math.Round(CoinsPrice, 2)} in {AmountOfCoins} diffrent coins";
            LoadTopCoins();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Website_Button(object sender, RoutedEventArgs e)
        {
            Extra.OpenProcess("https://github.com/Poshy163");
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Infopage mainapp = new Infopage();
            mainapp.Show();
            Close();
        }

        private void MessageBoxShow(string message)
        {
            MessageBox.Show(message);
        }

        private async void NetProft(object sender, RoutedEventArgs e)
        {
            dynamic jsonFile;
            float TotalReferral = 0, TotalAffiliate = 0, TotalAmountInCoin = 0, TotalDeposit = 0, TotalWithdrawal = 0;

            #region Referral

            try
            {
                jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Referral"]));
                if (jsonFile["status"] == "error")
                {
                    MessageBox.Show($"{jsonFile["message"]}");
                    return;
                }
                for (var i = 0; i <= int.MaxValue; i++)
                {
                    try
                    {
                        TotalReferral = jsonFile["payments"][i]["audamount"] + TotalReferral;
                    }
                    catch { break; }
                }
            }
            catch { }

            #endregion Referral

            #region Total Amount

            jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Balances"]));
            if (jsonFile["status"] == "error")
            {
                MessageBox.Show($"{jsonFile["message"]}");
                return;
            }
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    string[] file = ((jsonFile["balances"][i]).ToString()).Split('"');
                    TotalAmountInCoin = jsonFile["balances"][i][file[1]]["audbalance"] + TotalAmountInCoin;
                }
                catch { break; }
            }

            #endregion Total Amount

            #region Deposit

            jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Deposit"]));
            if (jsonFile["status"] == "error")
            {
                MessageBox.Show($"{jsonFile["message"]}");
                return;
            }
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    TotalDeposit = jsonFile["deposits"][i]["amount"] + TotalDeposit;
                }
                catch { break; }
            }

            #endregion Deposit

            #region Withdrawal

            jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Withdrawal"]));
            if (jsonFile["status"] == "error")
            {
                MessageBox.Show($"{jsonFile["message"]}");
                return;
            }
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    TotalWithdrawal = jsonFile["withdrawals"][i]["amount"] - TotalWithdrawal;
                }
                catch { break; }
            }

            #endregion Withdrawal

            #region Affiliate

            try
            {
                jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Affiliate"]));
                for (var i = 0; i <= int.MaxValue; i++)
                {
                    try
                    {
                        TotalAffiliate = jsonFile["payments"][i]["amount"] + TotalAffiliate;
                    }
                    catch { break; }
                }
            }
            catch { }

            #endregion Affiliate

            float FinalNetProfit = (TotalReferral + TotalAffiliate + TotalAmountInCoin + TotalWithdrawal) - TotalDeposit;
            MessageBox.Show($"You have a net profit of ${FinalNetProfit}");
        }

        private async void Totalrefferals(object sender, RoutedEventArgs e)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Referral"]));
            if (jsonFile["status"] == "error")
            {
                MessageBox.Show($"{jsonFile["message"]}");
                return;
            }
            int TotalPayment = 0, peopleReferred = 0;
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    TotalPayment = (jsonFile["payments"][i]["audamount"]) + TotalPayment;
                    peopleReferred++;
                }
                catch { break; }
            }
            MessageBoxShow($"You have referred {peopleReferred} people. for a total of ${TotalPayment} in BTC");
        }

        private async void TotalDepositAmount(object sender, RoutedEventArgs e)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Deposit"]));
            if (jsonFile["status"] == "error")
            {
                MessageBox.Show($"{jsonFile["message"]}");
                return;
            }
            float DepositAmount = 0;
            for (var i = 0; i <= int.MaxValue; i++)
            {
                try
                {
                    DepositAmount = jsonFile["deposits"][i]["amount"] + DepositAmount;
                }
                catch { break; }
            }
            MessageBoxShow($"You have deposited ${DepositAmount}");
        }

        private void SpecKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SpecCoinClick(sender, e);
            }
        }

        private async void SpecCoinClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SpecCointxt.Text))
            {
                MessageBox.Show("Empty Input Field");
                return;
            }
            else
            {
                string Coin = SpecCointxt.Text.ToUpper().Trim();
                dynamic jsonFile = JsonConvert.DeserializeObject(await API.MakeRequest(API.Root + API.APIlink["Balance"] + Coin));
                if (jsonFile["status"] != "error")
                {
                    try
                    {
                        MessageBox.Show($"You own: {jsonFile["balance"][Coin]["balance"]} {Coin} worth ${jsonFile["balance"][Coin]["audbalance"]}");
                    }
                    catch
                    {
                        MessageBox.Show("You do not own this coin");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show($"{jsonFile["message"]}");
                    return;
                }
            }
        }

        private void Exit_Application(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async void LoadTopCoins()
        {
            Dictionary<string, float> localdic = new Dictionary<string, float>();
            int localloop = 0, Topx = 5;
            Dictionary<string, CryptoCoin> TopList = new Dictionary<string, CryptoCoin>();

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    string[] file = ((AllcoinjsonFile["balances"][i]).ToString()).Split('"');
                    string info = AllcoinjsonFile["balances"][i] + "";
                    float coinAmount = AllcoinjsonFile["balances"][i][file[1]]["audbalance"];
                    localdic.Add(info, coinAmount);
                }
                catch
                {
                    break;
                }
            }

            foreach (KeyValuePair<string, float> author in localdic.OrderByDescending(key => key.Value))
            {
                if (localloop < Topx)
                {
                    CryptoCoin tcoin = new CryptoCoin(author.Key, author.Value);
                    TopList.Add(tcoin.CoinName, tcoin);
                }
                localloop++;
            }

            #region arrays to assign

            TextBlock[] textblocknames = new TextBlock[] { info1, info2, info3, info4, info5 };
            Image[] imagenames = new Image[] { Dynamicpic1, Dynamicpic2, Dynamicpic3, Dynamicpic4, Dynamicpic5 };
            TextBlock[] txt24arr = new TextBlock[] { txt241, txt242, txt243, txt244, txt245 };
            Image[] pic24arr = new Image[] { pic241, pic242, pic243, pic244, pic245 };

            #endregion arrays to assign

            for (int i = 0; i < textblocknames.Length; i++)
            {
                string change24hr = await API.GetIncrease(TopList.ElementAt(i).Key);

                txt24arr[i].Text = change24hr;
                if (change24hr.Contains("-"))
                    pic24arr[i].Source = new BitmapImage(new Uri(@"/Images/RedDOWN.png", UriKind.Relative));
                else
                    pic24arr[i].Source = new BitmapImage(new Uri(@"/Images/GreenUP.png", UriKind.Relative));

                textblocknames[i].Text = TopList.ElementAt(i).Value.FormatResponse();
                imagenames[i].Source = await Extra.BtnLoadFromFileAsync(TopList.ElementAt(i).Key);
            }
            Statpanel.Opacity = 100;
        }
    }

    public partial struct API
    {
        public static readonly string Root = $"https://www.coinspot.com.au/api/v2/ro";

        public static Dictionary<string, string> APIlink = new Dictionary<string, string>()
        {
            {"Status", "/status"},
            {"Balance", "/my/balance/"},
            {"Balances", "/my/balances"},
            {"Referral","/my/referralpayments"},
            {"Affiliate", " /my/affiliatepayments"},
            {"SendReceive","/my/sendreceive" },
            {"Transaction", "/my/transactions" },
            {"Deposit", "/my/deposits" },
            {"Withdrawal", "/my/withdrawals"},
            {"Open Market", "/my/transactions/open"},
        };

        public static WebClient client = new WebClient();

        public static async Task<string> GetIncrease(string coinName)
        {
            string URL = "https://www.coinspot.com.au/buy/" + coinName.ToLower();
            try
            {
                try
                {
                    string htmlCode = client.DownloadString(URL).Split("flaticon-upload-1")[1].Split("</div>")[0].Split(";")[2];
                    return htmlCode;
                }
                catch
                {
                    string htmlCode = client.DownloadString(URL).Split("flaticon-download")[1].Split("</div>")[0].Split(";")[2];
                    return htmlCode;
                }
            }
            catch (Exception ex)
            {
                Extra.SendSlackMessage(ex.Message);
                return "ERROR";
            }
        }

        public static async Task<string> MakeRequest(string url)
        {
            try
            {
                long nonce = DateTime.Now.Ticks;
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var hash = new StringBuilder();
                string signatureBaseString = "{\"nonce\":\"" + nonce + "\"}";
                request.Content = new StringContent(signatureBaseString, Encoding.UTF8, "application/json");
                hash.Append(SHA512_ComputeHash(request.Content.ReadAsStringAsync().Result, MainApp.SecretKey));
                request.Headers.Add("key", MainApp.PublicKey);
                request.Headers.Add("sign", hash.ToString());
                HttpResponseMessage response = await client.SendAsync(request);
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "ERROR";
            }
        }

        public static string SHA512_ComputeHash(string text, string secretKey)
        {
            var hash = new StringBuilder();
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }

    public class CryptoCoin
    {
        public float CoinPriceAUD, CoinAmount, Rate;
        public string CoinName;

        public CryptoCoin(string jsonFile, float t_CoinPriceAUD)
        {
            CoinPriceAUD = t_CoinPriceAUD;
            GetAddionalInfo(JsonConvert.DeserializeObject(jsonFile));
        }

        public void GetAddionalInfo(dynamic jsonFile)
        {
            string[] file = ((jsonFile).ToString()).Split('"');
            CoinName = file[1];
            CoinAmount = jsonFile[CoinName]["balance"];
            Rate = jsonFile[CoinName]["rate"];
        }

        public string FormatResponse()
        {
            return $"{CoinName} | ${CoinPriceAUD}";
        }
    }
}