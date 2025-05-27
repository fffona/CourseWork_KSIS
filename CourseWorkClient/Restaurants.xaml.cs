using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Net.Http;
using System.Net.Http.Json;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для Restaurants.xaml
    /// </summary>
    public partial class Restaurants : Window
    {
        public Restaurants()
        {
            InitializeComponent();
            if (AuthState.isAuthenticated)
            {
                Button_Authorization.Visibility = Visibility.Hidden;
                TextBlock_Authorization.Visibility = Visibility.Visible;
            }
            this.PreviewKeyDown += AdminWindow_PreviewKeyDown;

            Loaded += async (s, e) => await UILoad();
        }

        public class ApiKeyResponse
        {
            public string ApiKey { get; set; }
        }

        private async Task UILoad()
        {
            var keyResponse = await ApiClient.Instance.GetAsync("Map/api-key");
            keyResponse.EnsureSuccessStatusCode();
            var keyResult = await keyResponse.Content.ReadFromJsonAsync<ApiKeyResponse>();
            string apiKey = keyResult.ApiKey; 
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map.html"); ;
            string html = File.ReadAllText(htmlPath);
            html = html.Replace("{{API_KEY}}", apiKey);

            await webView.EnsureCoreWebView2Async(null);
            webView.NavigateToString(html);

            if (AuthState.isAuthenticated)
            {
                var username = await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getname");
                if (username.Length > 13) TextBlock_Authorization.Text = $"Здравствуйте,\n{username.Substring(0, 11)}...";
                else TextBlock_Authorization.Text = $"Здравствуйте,\n{username}";
            }
        }


        private void Button_Authorization_Click(object sender, RoutedEventArgs e)
        {
            SignIn signin = new SignIn();
            signin.Show();
            Close();
        }

        private void Button_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            Close();
        }

        private void Button_Cart_Click(object sender, RoutedEventArgs e)
        {
            Cart cart = new Cart();
            cart.Show();
            Close();
        }

        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            History history = new History();
            history.Show();
            Close();
        }

        private void Button_Restaurants_Click(object sender, RoutedEventArgs e)
        {
            Restaurants rest = new Restaurants();
            rest.Show();
            Close();
        }

        private void AdminWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.A) && e.Key == Key.D0)
            {
                Admin admin = new Admin();
                admin.Show();
                Close();
            }
        }
    }
}
