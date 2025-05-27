using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using static System.Net.Mime.MediaTypeNames;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public Admin()
        {
            InitializeComponent();
        }

        private async void Button_ControlUsers_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            BackToMain.Visibility = Visibility.Collapsed;
            ControlUsersBorder.Visibility = Visibility.Visible;
            IdTextBox.Visibility = Visibility.Visible;
            delUserButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
            Users = await ApiClient.Instance.GetFromJsonAsync<ObservableCollection<User>>("Users") ?? new ObservableCollection<User>();
            usersDataGrid.ItemsSource = Users;
            usersDataGrid.CellEditEnding += async (s, ev) =>
            {
                if (ev.Row.Item is User editedUser)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(editedUser);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await ApiClient.Instance.PutAsync("Users", content);
                }
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private async void delUserButton_Click(object sender, RoutedEventArgs e)
        {
            int userId;
            if (int.TryParse(IdTextBox.Text, out userId))
            {
                var response = await ApiClient.Instance.DeleteAsync($"Users/{userId}");
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent) MessageBox.Show("Пользователь удален.");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) MessageBox.Show("Такого пользователя не найдено. Введите корректный ID.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Visible;
            BackToMain.Visibility = Visibility.Visible;
            ControlUsersBorder.Visibility = Visibility.Collapsed;
            IdTextBox.Visibility = Visibility.Collapsed;
            delUserButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            ControlMenuBorder.Visibility = Visibility.Collapsed;
            delMenuButton.Visibility = Visibility.Collapsed;
            CreateMenuButton.Visibility = Visibility.Collapsed;
        }

        private async void Button_ControlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            BackToMain.Visibility = Visibility.Collapsed;
            ControlMenuBorder.Visibility = Visibility.Visible;
            IdTextBox.Visibility = Visibility.Visible;
            delMenuButton.Visibility = Visibility.Visible;
            CreateMenuButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
            Products = await ApiClient.Instance.GetFromJsonAsync<ObservableCollection<Product>>("Products") ?? new ObservableCollection<Product>();
            menuDataGrid.ItemsSource = Products;
            menuDataGrid.CellEditEnding += async (s, ev) =>
            {
                if (ev.Row.Item is Product editedProd)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(editedProd);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await ApiClient.Instance.PutAsync("Products/edit", content);
                }
            };
        }

        private async void delMenuButton_Click(object sender, RoutedEventArgs e)
        {
            int productId;
            if (int.TryParse(IdTextBox.Text, out productId))
            {
                var response = await ApiClient.Instance.DeleteAsync($"Products/{productId}");
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent) MessageBox.Show("Товар удален.");
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) MessageBox.Show("Такого товара не найдено. Введите корректный ID.");

            }
        }

        private async void CreateMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var newProduct = new Product
            {
                Name = "Новый продукт",
                Image = "C:/Users/fona/source/repos/coursework3/Media/No_image_available.svg.png"
            };
            var json = JsonConvert.SerializeObject(newProduct);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await ApiClient.Instance.PostAsync($"Products", content);
            Products.Add(await ApiClient.Instance.GetFromJsonAsync<Product>($"Products/getlastprod"));
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            Close();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdminPassword.Password.Trim() == "22122005")
            {
                confirmButton.Visibility = Visibility.Collapsed;
                AdminPassword.Visibility = Visibility.Collapsed;
                MainMenu.Visibility = Visibility.Visible;
            }
            else MessageBox.Show("Пароль неверный.\nЕсли вы попали сюда случайно, лучше вернитесь в приложение :)");
        }
    }
}
