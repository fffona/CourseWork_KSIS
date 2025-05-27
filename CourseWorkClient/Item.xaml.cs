using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : Window
    {
        public Item()
        {
            InitializeComponent();
        }

        private Product prodForCart = new Product();
        private int switchcase = 0;
        public Item(Product product)
        {
            prodForCart = product;
            InitializeComponent();
            if (AuthState.isAuthenticated)
            {
                Button_Authorization.Visibility = Visibility.Hidden;
                TextBlock_Authorization.Visibility = Visibility.Visible;                
            }
            Image image = new Image
            {
                Source = new BitmapImage(new Uri($"{product.Image}")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock textBlockName = new TextBlock
            {
                Text = product.Name,
                FontFamily = new FontFamily("Mustica Pro SemBd"),
                FontSize = 40,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment= VerticalAlignment.Bottom
            };

            textBlockDesc.Text = $"{product.Description}\n\n";

            if (product.Cost.Contains("от"))
            {
                RadioButton1.IsChecked = true;
                string[] amounts = product.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1;  i < amounts.Length; i++)
                {
                    string[] parts = amounts[i].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    switch (i)
                    {
                        case 1:
                            RadioButton1.Content = parts[0];
                            RadioButton1.Visibility = Visibility.Visible;
                            break;
                        case 2:
                            RadioButton2.Content = parts[0];
                            RadioButton2.Visibility = Visibility.Visible;
                            break;
                        case 3:
                            RadioButton3.Content = parts[0];
                            RadioButton3.Visibility = Visibility.Visible;
                            break;
                        case 4:
                            RadioButton4.Content = parts[0];
                            RadioButton4.Visibility = Visibility.Visible;
                            break;
                    }
                }             
            } else textBlockDesc.Text += $"{product.Cost} руб.";

            ItemImageGrid.Children.Add(image);

            ItemGrid.Children.Add(textBlockName);
            Grid.SetRow(textBlockName, 0);

            if (AuthState.isAuthenticated) Loaded += async (s, e) => await MakeNiceUI();
        }

        private async Task MakeNiceUI()
        {
            var username = await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getname");
            if (username.Length > 13) TextBlock_Authorization.Text = $"Здравствуйте,\n{username.Substring(0, 11)}...";
            else TextBlock_Authorization.Text = $"Здравствуйте,\n{username}";

            var purchasestatus = await ApiClient.Instance.GetFromJsonAsync<int>($"Users/{AuthState.Id}/getpurchasestatus");
            if (purchasestatus == 1 || purchasestatus == 2 || purchasestatus == 3)
            {
                ActivePurchaseStatusText.Visibility = Visibility.Visible;
            }
            else
            {
                AddToCartButton.Visibility = Visibility.Visible;
                ItemCounter.Visibility = Visibility.Visible;
                Amount.Visibility = Visibility.Visible;
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

        private int count = 1;
        private async void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            switch (switchcase)
            {
                case 0:
                    break;
                case 1:
                    prodForCart.Name = Regex.Replace(prodForCart.Name, @"\s*\(.*?\)", "");
                    prodForCart.Name += $" ({RadioButton1.Content})";
                    break;
                case 2:
                    prodForCart.Name = Regex.Replace(prodForCart.Name, @"\s*\(.*?\)", "");
                    prodForCart.Name += $" ({RadioButton2.Content})";
                    break;
                case 3:
                    prodForCart.Name = Regex.Replace(prodForCart.Name, @"\s*\(.*?\)", "");
                    prodForCart.Name += $" ({RadioButton3.Content})";
                    break;
                case 4:
                    prodForCart.Name = Regex.Replace(prodForCart.Name, @"\s*\(.*?\)", "");
                    prodForCart.Name += $" ({RadioButton4.Content})";
                    break;
            }

            var cart = await ApiClient.Instance.GetFromJsonAsync<string?>($"Users/{AuthState.Id}/getcart");
            if (cart != null && cart.Contains(prodForCart.Name) && cart != "")
            {
                string[] prods = cart.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] parts = prods.FirstOrDefault(n => n.Contains(prodForCart.Name)).Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                count += Convert.ToInt16(parts[1]);
                int indexToRemove = Array.IndexOf(prods, prods.FirstOrDefault(n => n.Contains(prodForCart.Name)));
                string[] newProds = prods.Where((val, idx) => idx != indexToRemove).ToArray();
                cart = null;
                foreach (string newprod in newProds) {
                    cart += $"{newprod}\n";
                }
            }
            if (prodForCart.Cost.Contains("от"))
            {
                string[] amounts = prodForCart.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] diffparts;
                switch (switchcase)
                {
                    case 1:
                        diffparts = amounts[1].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        cart += $"{prodForCart.Name} - {count} - {diffparts[1]}\n";
                        break;
                    case 2:
                        diffparts = amounts[2].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        cart += $"{prodForCart.Name} - {count} - {diffparts[1]}\n";
                        break;
                    case 3:
                        diffparts = amounts[3].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        cart += $"{prodForCart.Name} - {count} - {diffparts[1]}\n";
                        break;
                    case 4:
                        diffparts = amounts[4].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        cart += $"{prodForCart.Name} - {count} - {diffparts[1]}\n";
                        break;
                }
            } else cart += $"{prodForCart.Name} - {count}\n";
            count = 1;
            NumberTextBlock.Text = count.ToString();
            var json = JsonSerializer.Serialize(cart);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await ApiClient.Instance.PutAsync($"Users/{AuthState.Id}/cart/add", content);

            MessageBox.Show("Товар добавлен в корзину.");
        }
        
        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            if (count != 1) count--;
            NumberTextBlock.Text = count.ToString();
        }
        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            if (count != 99) count++;
            NumberTextBlock.Text = count.ToString();
        }

        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            string[] amounts = prodForCart.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = amounts[1].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            textBlockDesc.Text = $"{prodForCart.Description}\n\n{parts[1]} руб.";
            switchcase = 1;
        }

        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            string[] amounts = prodForCart.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = amounts[2].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            textBlockDesc.Text = $"{prodForCart.Description}\n\n{parts[1]} руб.";
            switchcase = 2;
        }

        private void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            string[] amounts = prodForCart.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = amounts[3].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            textBlockDesc.Text = $"{prodForCart.Description}\n\n{parts[1]} руб.";
            switchcase = 3;
        }

        private void RadioButton4_Checked(object sender, RoutedEventArgs e)
        {
            string[] amounts = prodForCart.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = amounts[4].Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            textBlockDesc.Text = $"{prodForCart.Description}\n\n{parts[1]} руб.";
            switchcase = 4;
        }
    }
}
