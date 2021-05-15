using System;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Input;
using System.IO;

namespace ModernUI
{
    public partial class Infopage : Window
    {
        public Infopage()
        {
            InitializeComponent();
        }

        private void CheckForUpdate()
        {
            MessageBoxResult result = MessageBox.Show("There is a new update, would you like to install it?", "Update", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    System.Diagnostics.Process.Start("https://github.com/Poshy163");
                    System.Windows.Application.Current.Shutdown();
                    break;

                case MessageBoxResult.No:
                    break;
            }
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
            System.Diagnostics.Process.Start("https://github.com/Poshy163");
        }

        private void Exit_Application(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnpageLoad()
        {
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            CheckForUpdate();
        }
    }
}