using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net.Http;
using System.Linq;
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
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private async void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text.Trim();
            string password = passwordPassBox.Password.Trim();
            string repeatPassword = repeatPasswordPassBox.Password.Trim();

            passwordErrorTextBlock.Visibility = Visibility.Hidden;
            ErrorTextBlock.Visibility = Visibility.Hidden;
            ErrorLoginTextBlock.Visibility = Visibility.Hidden;
            TextBoxHelper.SetIsErrorState(loginTextBox, false);
            PasswordBoxHelper.SetIsErrorState(passwordPassBox, false);
            PasswordBoxHelper.SetIsErrorState(repeatPasswordPassBox, false);

            if (login == "")
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                TextBoxHelper.SetIsErrorState(loginTextBox, true);
            }
            else if (password == "")
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                PasswordBoxHelper.SetIsErrorState(passwordPassBox, true);
            }
            else if (repeatPassword == "")
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                PasswordBoxHelper.SetIsErrorState(repeatPasswordPassBox, true);
            }
            else if (await ApiClient.Instance.GetFromJsonAsync<bool>($"Users/{login}/signup"))
            {
                ErrorLoginTextBlock.Visibility = Visibility.Visible;
                TextBoxHelper.SetIsErrorState(loginTextBox, true);
            }
            else if (password != repeatPassword)
            {
                passwordErrorTextBlock.Visibility = Visibility.Visible;
                PasswordBoxHelper.SetIsErrorState(repeatPasswordPassBox, true);
            }
            else
            {
                var requestData = new { Login = login, Password = ApiClient.Encrypt(password) };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await ApiClient.Instance.PostAsync($"Users/add", content);

                SignIn signIn = new SignIn();
                signIn.Show();
                Close();
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
    }
}
