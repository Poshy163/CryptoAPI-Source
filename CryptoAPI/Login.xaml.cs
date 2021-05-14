﻿using System.Windows;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows.Input;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using System.IO;
using MongoDB.Bson.IO;
using System.Text;

namespace ModernUI
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
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
            System.Diagnostics.Process.Start("https://github.com/Poshy163");
        }

        private void DatabaseLogin(object sender, RoutedEventArgs e)
        {
            string ID = IDBox.Text.ToUpper();
            var client = new MongoClient("mongodb+srv://APIUSER:mdzJO9cfs4FGoxe3@userdata.1xmw5.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            var database = client.GetDatabase("UserData");
            var collection = database.GetCollection<BsonDocument>("APIKEYS");
            var documents = collection.Find(new BsonDocument()).ToList();

            foreach (BsonDocument doc in documents)
            {
                dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(doc));
                if (jsonFile["ID"] == ID)
                {
                    string PubKey = jsonFile["PUBLICKEY"];
                    string SecretKey = jsonFile["PRIVATEKEY"];
                    MainApp mainApp = new MainApp(PubKey, SecretKey, ID);
                    mainApp.Show();
                    mainApp.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    Close();
                    return;
                }
            }
            MessageBox.Show($"No User with the ID {ID} found");
        }

        private void LocalLogin(object sender, RoutedEventArgs e)
        {
            string ID = IDBox.Text.ToUpper();
            if (!File.Exists(Register.FilePath))
            {
                MessageBox.Show($"No data exists");
                return;
            }
            var str = new StreamReader(Register.FilePath);
            string[] strRes = str.ReadToEnd().Split(':');
            str.Close();
            if (strRes[0] == ID)
            {
                MessageBox.Show("Data exists");
            }
            else
            {
                MessageBox.Show("No data exists");
            }
        }

        public string ToJson(BsonDocument bson)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(stream))
                {
                    BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
                }
                stream.Seek(0, SeekOrigin.Begin);
#pragma warning disable CS0618 // Type or member is obsolete
                using (var reader = new Newtonsoft.Json.Bson.BsonReader(stream))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);
                    using (var jWriter = new JsonTextWriter(sw))
                    {
                        jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        jWriter.WriteToken(reader);
                    }
                    return sb.ToString();
                }
            }
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
    }
}