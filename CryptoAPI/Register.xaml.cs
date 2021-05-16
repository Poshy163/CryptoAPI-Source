using System.Windows;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Windows.Input;
using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System.Text;
using Newtonsoft.Json;
using System;

namespace CryptoAPI
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Website_Button(object sender, RoutedEventArgs e)
        {
            Extra.OpenProcess("https://github.com/Poshy163");
        }

        public static readonly string FilePath = Directory.GetCurrentDirectory() + @"\SaveInfo.txt";

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ToLogin()
        {
            Login login = new Login
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            login.Show();
            Close();
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            string pubKey = PublicKey.Text.Trim(), secretKey = SecretKey.Text.Trim(), ID = IDBox.Text.Trim();
            if (!IsValid(pubKey, secretKey, ID))
            {
                MessageBox.Show("Error one of these fields is not valid");
                return;
            }

            try
            {
                var client = new MongoClient("mongodb+srv://APIUSER:mdzJO9cfs4FGoxe3@userdata.1xmw5.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
                var database = client.GetDatabase("UserData");
                var collection = database.GetCollection<BsonDocument>("APIKEYS");
                var documents = collection.Find(new BsonDocument()).ToList();
                foreach (BsonDocument doc in documents)
                {
                    dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(doc));
                    if (jsonFile["ID"] == ID)
                    {
                        MessageBox.Show($"Database already contatins a ID called {ID}");
                        return;
                    }
                }
                var document = new BsonDocument
                {
                {"ID", ID.ToUpper()},
                {"PUBLICKEY", pubKey },
                {"PRIVATEKEY", secretKey },
                };
                collection.InsertOne(document);
            }
            catch (Exception ex)
            {
                Extra.SendSlackMessage(ex.Message);
            }
            ToLogin();
        }

        private bool IsValid(string pub, string secret, string ID)
        {
            if (string.IsNullOrEmpty(pub) || string.IsNullOrWhiteSpace(pub) || string.IsNullOrEmpty(secret) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrEmpty(ID) || string.IsNullOrWhiteSpace(ID))
                return false;
            return true;
        }

        public string ToJson(BsonDocument bson)
        {
            using var stream = new MemoryStream();
            using (BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
            }
            stream.Seek(0, SeekOrigin.Begin);
#pragma warning disable CS0618 // Type or member is obsolete
            using var reader = new Newtonsoft.Json.Bson.BsonReader(stream);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var jWriter = new JsonTextWriter(sw))
            {
                jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jWriter.WriteToken(reader);
            }
            return sb.ToString();
        }

        private void Exit_Application(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Infopage infopage = new Infopage();
            infopage.Show();
            Close();
        }

        private void PageLoaded()
        {
        }
    }
}