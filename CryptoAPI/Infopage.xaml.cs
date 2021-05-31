using System;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Diagnostics;

namespace CryptoAPI
{
    public partial class Infopage : Window
    {
        public Infopage()
        {
            InitializeComponent();
        }

        public string CurrentVersion = "1.4";

        private void CheckForUpdate()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                var json = client.GetAsync("https://api.github.com/repos/Poshy163/CryptoAPI/commits").Result.Content.ReadAsStringAsync().Result;
                dynamic commits = JArray.Parse(json);
                string lastCommit = commits[0].commit.message;
                string LatestRealese = lastCommit.Split("\n")[0].Split(" ")[1];
                if (CurrentVersion != LatestRealese)
                {
                    MessageBoxResult result = MessageBox.Show("There is a new update, would you like to install it?", "CryptoAPI update", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            Extra.OpenProcess("https://github.com/Poshy163/CryptoAPI");
                            Application.Current.Shutdown();
                            break;
                    }
                }
            }
            catch { }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Start_Button(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            Close();
        }

        private void Register_Button(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            Close();
        }

        public static Point PointToWindow(UIElement element, Point pointOnElement)
        {
            Window wnd = Window.GetWindow(element);
            if (wnd == null)
            {
                throw new InvalidOperationException("target element is not yet connected to the Window drawing surface");
            }
            return element.TransformToAncestor(wnd).Transform(pointOnElement);
        }

        private void Website_Button(object sender, RoutedEventArgs e)
        {
            Extra.OpenProcess("https://github.com/Poshy163");
        }

        private void Exit_Application(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            CheckForUpdate();
        }
    }
}