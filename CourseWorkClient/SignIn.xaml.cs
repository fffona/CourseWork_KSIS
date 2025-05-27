using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    { 
        public SignIn()
        {
            InitializeComponent();
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public int id { get; set; }
        }

        private async void Button_SignIn_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text.Trim();
            string password = passPasswordBox.Password.Trim();

            ErrorTextBlock.Visibility = Visibility.Hidden;
            ErrorLoginOrPassTextBlock.Visibility = Visibility.Hidden;
            TextBoxHelper.SetIsErrorState(loginTextBox, false);
            PasswordBoxHelper.SetIsErrorState(passPasswordBox, false);

            if (login == "")
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                TextBoxHelper.SetIsErrorState(loginTextBox, true);
            }
            else if (password == "")
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                PasswordBoxHelper.SetIsErrorState(passPasswordBox, true);
            }
            else
            {
                var requestData = new { Login = login, Password = ApiClient.Encrypt(password) };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await ApiClient.Instance.PostAsync("Users/authenticate", content); 
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ErrorLoginOrPassTextBlock.Visibility = Visibility.Visible;
                    TextBoxHelper.SetIsErrorState(loginTextBox, true);
                    PasswordBoxHelper.SetIsErrorState(passPasswordBox, true);
                    return;
                }
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                AuthState.Id = result.id;
                ApiClient.SetJwtToken(result.Token);
                if (await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getiscashier") == "false")
                {
                    AuthState.isAuthenticated = true;
                    Menu menu = new Menu();
                    menu.Show();
                    Close();
                } else
                {
                    var place = await ApiClient.Instance.GetFromJsonAsync<string?>($"Users/{AuthState.Id}/getplace");
                    Cashier cashier = new Cashier(place);
                    cashier.Show();
                    Close();
                }
            }
        }

        private void Button_SignUpWindow_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp = new SignUp();
            signUp.Show();
            Close();
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
    }
}
