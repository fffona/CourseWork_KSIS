using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using static System.Net.Mime.MediaTypeNames;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для Cart.xaml
    /// </summary>
    public partial class Cart : Window
    {
        private List<string> suggestions;

        public Cart()
        {
            InitializeComponent();
            suggestions = new List<string>
            {
                "Логойский тракт, 35", "ул. Шугаева, 2", "ул.П.Мстиславца, 11-4, пом.68",
                "ул. Сурганова, 63", "пр. Независимости, 52", "ул. Тимирязева, 76",
                "ТЦ Green City (ул. Притыцкого, 156-9)", "ул. Притыцкого, 58", "ул. Притыцкого, 28",
                "ул. Шаранговича, 23", "пр-т Дзержинского, 59", "пр. Дзержинского, 96",
                "ТРЦ DiaMond city (Щомыслицкий с/c, 32/4, пом. 209, Минская область, Минский район)",
                "ТЦ МОМО (пр. Партизанский, 150А)", "ул. Уборевича, 93", "ул. Максима Танка, 2",
                "ул. Немига, 12Б", "пр. Независимости, 23", "ул. Кирова, 1", "ул. Бобруйская, 6 (ТРЦ Galileo)",
                "г. Могилев, пр. Пушкинский, 12 (ТЦ «Арбат»)", "г. Витесбк, ул. Терешковой, 1", "г. Гомель, ул. Советская, 68",
                "г. Брест, ул. Октябрьской революции, д. 1а.", "г. Гродно, ул. Горновых, 9", "г. Гродно, ТРК TRINITI (проспект Янки Купалы 87, 3 этаж)"
            };

            if (AuthState.isAuthenticated)
            {
                Button_Authorization.Visibility = Visibility.Hidden;
                TextBlock_Authorization.Visibility = Visibility.Visible;
                borderCart.Visibility = Visibility.Visible;
                showProducts();
            }
            if (!AuthState.isAuthenticated)
            {
                TextBlock error = new TextBlock
                {
                    Text = "Вы не вошли в аккаунт!",
                    FontSize = 35,
                    FontFamily = new FontFamily("Faberge"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Width = 400,
                    Height = 60,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                error.Margin = new Thickness(0,100,0,0);
                mainStackPanel.Children.Add(error);
            }

            if (AuthState.isAuthenticated) Loaded += async (s, e) => await MakeNiceName();
        }
        private string restaurantName = null;
        private string PaymentMethod = null;
        private async Task MakeNiceName()
        {
            var username = await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getname");
            if (username.Length > 13) TextBlock_Authorization.Text = $"Здравствуйте,\n{username.Substring(0, 11)}...";
            else TextBlock_Authorization.Text = $"Здравствуйте,\n{username}";
        }
        private async void showProducts()
        {
            var cart = await ApiClient.Instance.GetFromJsonAsync<string?>($"Users/{AuthState.Id}/getcart");
            double result = 0;

            if (cart != "" && cart != null)
            {
                string[] prods = cart.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string prod in prods)
                {
                    if (prod != null)
                    {
                        string[] parts = prod.Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        Product product = new Product();
                        string temp = parts[0];
                        if (parts.Length == 3) temp = Regex.Replace(temp, @"\s*\(.*?\)", "");
                        var response = await ApiClient.Instance.GetAsync($"Products/{temp}");
                        if (response.IsSuccessStatusCode)
                        {
                            product = await response.Content.ReadFromJsonAsync<Product>();
                            System.Windows.Controls.Image image = new System.Windows.Controls.Image
                            {
                                Source = new BitmapImage(new Uri($"{product.Image}")),
                                Width = 90,
                                Height = 90,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            image.Margin = new Thickness(35, 0, 0, 0);
                            TextBlock textblock = new TextBlock
                            {
                                Text = $"{parts[0]}",
                                FontSize = 25,
                                FontFamily = new FontFamily("Faberge"),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                TextAlignment = TextAlignment.Left,
                                TextWrapping = TextWrapping.Wrap,
                                Width = 250
                            };
                            textblock.Margin = new Thickness(160, 0, 0, 0);
                            TextBlock amount = new TextBlock
                            {
                                Text = $"х{parts[1]}",
                                FontSize = 25,
                                FontFamily = new FontFamily("Faberge"),
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            amount.Margin = new Thickness(0, 0, 165, 0);
                            TextBlock cost = new TextBlock
                            {
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                FontFamily = new FontFamily("Faberge"),
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            if (product.Cost.Contains("от")) cost.Text = $"{parts[2]}";
                            else cost.Text = $"{product.Cost}";
                            cost.Margin = new Thickness(0, 0, 40, 0);
                            Button openItemInfo = new Button
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(35, 0, 0, 0),
                                Width = 340,
                                Height = 90,
                                Style = (Style)FindResource("DefaultButton"),
                                Background = Brushes.Transparent,
                                BorderThickness = new Thickness(0),
                                Foreground = Brushes.Transparent
                            };
                            openItemInfo.Click += (sender, e) =>
                            {
                                Item item = new Item(product);
                                item.Show();
                                Close();
                            };
                            Grid itemCounter = new Grid
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Margin = new Thickness(0, 0, 200, 0),
                                Width = 40,
                                Height = 30
                            };
                            itemCounter.ColumnDefinitions.Add(new ColumnDefinition());
                            itemCounter.ColumnDefinitions.Add(new ColumnDefinition());
                            Button decrementButton = new Button
                            {
                                Style = (Style)FindResource("GrayButton"),
                                Background = new SolidColorBrush(Color.FromRgb(194, 194, 194)),
                                Content = "-",
                                FontSize = 30,
                                Cursor = Cursors.Hand,
                                FontFamily = new FontFamily("Faberge"),
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            decrementButton.Click += async (sender, e) =>
                            {
                                int indexToChange = Array.IndexOf(prods, prods.FirstOrDefault(n => n.Contains(temp)));
                                if (Convert.ToInt16(parts[1]) != 1)
                                {
                                    parts[1] = (Convert.ToInt16(parts[1]) - 1).ToString();
                                    if (parts.Length == 3) prods[indexToChange] = $"{parts[0]} - {parts[1]} - {parts[2]}";
                                    else prods[indexToChange] = $"{parts[0]} - {parts[1]}";
                                    cart = null;
                                    foreach (string newprod in prods)
                                    {
                                        cart += $"{newprod}\n";
                                    }
                                }
                                else
                                {
                                    string[] newProds = prods.Where((val, idx) => idx != indexToChange).ToArray();
                                    cart = null;
                                    foreach (string newprod in newProds)
                                    {
                                        cart += $"{newprod}\n";
                                    }
                                }
                                var json = JsonSerializer.Serialize(cart);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                await ApiClient.Instance.PutAsync($"Users/{AuthState.Id}/cart/add", content);
                                cartStackPanel.Children.Clear();
                                showProducts();
                            };
                            Grid.SetColumn(decrementButton, 0);
                            itemCounter.Children.Add(decrementButton);
                            Button incrementButton = new Button
                            {
                                Style = (Style)FindResource("GrayButton"),
                                Background = new SolidColorBrush(Color.FromRgb(194, 194, 194)),
                                Content = "+",
                                FontSize = 30,
                                Cursor = Cursors.Hand,
                                FontFamily = new FontFamily("Faberge"),
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            incrementButton.Click += async (sender, e) =>
                            {
                                if (Convert.ToInt16(parts[1]) != 99)
                                {
                                    int indexToChange = Array.IndexOf(prods, prods.FirstOrDefault(n => n.Contains(temp)));
                                    parts[1] = (Convert.ToInt16(parts[1]) + 1).ToString();
                                    if (parts.Length == 3) prods[indexToChange] = $"{parts[0]} - {parts[1]} - {parts[2]}";
                                    else prods[indexToChange] = $"{parts[0]} - {parts[1]}";
                                    cart = null;
                                    foreach (string newprod in prods)
                                    {
                                        cart += $"{newprod}\n";
                                    }
                                    var json = JsonSerializer.Serialize(cart);
                                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    await ApiClient.Instance.PutAsync($"Users/{AuthState.Id}/cart/add", content);
                                    cartStackPanel.Children.Clear();
                                    showProducts();
                                }
                            };
                            Grid.SetColumn(incrementButton, 1);
                            itemCounter.Children.Add(incrementButton);

                            Grid newgrid = new Grid
                            {
                                Width = 680,
                                Height = 110
                            };

                            newgrid.Children.Add(image);
                            newgrid.Children.Add(textblock);
                            newgrid.Children.Add(amount);
                            newgrid.Children.Add(cost);
                            var purchasestatus = await ApiClient.Instance.GetFromJsonAsync<int>($"Users/{AuthState.Id}/getpurchasestatus");
                            if (purchasestatus != 1 && purchasestatus != 2 && purchasestatus != 3) newgrid.Children.Add(itemCounter);
                            newgrid.Children.Add(openItemInfo);
                            cartStackPanel.Children.Add(newgrid);

                            if (product.Cost.Contains("от")) result += Convert.ToInt16(parts[1]) * Convert.ToDouble(parts[2], CultureInfo.InvariantCulture);
                            else result += Convert.ToInt16(parts[1]) * Convert.ToDouble(product.Cost, CultureInfo.InvariantCulture);
                        }
                    }
                }
                if (result != 0)
                {
                    TextBlock sum = new TextBlock
                    {
                        Text = $"Итого:\n{result} руб.",
                        FontSize = 38,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Faberge"),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        TextAlignment = TextAlignment.Right
                    };
                    sum.Margin = new Thickness(0, 0, 40, 20);
                    Button purchaseButton = new Button
                    {
                        Width = 120,
                        Height = 40,
                        Content = "Оплатить",
                        FontFamily = new FontFamily("Faberge"),
                        FontSize = 23,
                        Cursor = Cursors.Hand,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(40, 0, 0, 20),
                        Background = new SolidColorBrush(Color.FromRgb(247, 206, 54))
                    };
                    purchaseButton.Click += PurchaseButton_Click;
                    Grid purchaseGrid = new Grid();
                    purchaseButton.Style = (Style)FindResource("YellowButton");
                    TextBlock declined = new TextBlock // -1
                    {
                        Text = "Заказ отклонен.",
                        Foreground = new SolidColorBrush(Color.FromRgb(194, 48, 48)),
                        FontSize = 22,
                        FontFamily = new FontFamily("Faberge"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(170, 0, 0, 28)
                    };
                    TextBlock cooking = new TextBlock // 2
                    {
                        Text = "Готовим ваш заказ :)",
                        Foreground = new SolidColorBrush(Color.FromRgb(64, 145, 10)),
                        FontSize = 22,
                        FontFamily = new FontFamily("Faberge"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(40, 0, 0, 24)
                    };
                    TextBlock ready = new TextBlock // 3
                    {
                        Text = "Заказ готов к получению.",
                        Foreground = new SolidColorBrush(Color.FromRgb(64, 145, 10)),
                        FontSize = 22,
                        FontFamily = new FontFamily("Faberge"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(40, 0, 0, 24)
                    };
                    TextBlock waiting = new TextBlock // 1
                    {
                        Text = "Ожидание ответа ресторана...",
                        Foreground = new SolidColorBrush(Color.FromRgb(112, 112, 112)),
                        FontSize = 22,
                        FontFamily = new FontFamily("Faberge"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(40, 0, 0, 24)
                    };

                    var purchasestatus = await ApiClient.Instance.GetFromJsonAsync<int>($"Users/{AuthState.Id}/getpurchasestatus");
                    if (purchasestatus == -1)
                    {
                        purchaseGrid.Children.Add(declined);
                    }
                    purchaseGrid.Children.Add(sum);
                    if (purchasestatus == 2)
                    {
                        purchaseGrid.Children.Add(cooking);
                    }
                    else if (purchasestatus == 3)
                    {
                        purchaseGrid.Children.Add(ready);
                    }
                    else if (purchasestatus == 1)
                    {
                        purchaseGrid.Children.Add(waiting);
                    }
                    else purchaseGrid.Children.Add(purchaseButton);
                    cartStackPanel.Children.Add(purchaseGrid);
                }
            }
            if (cart == "" || cart == null || result == 0)
            {
                TextBlock nullCartText = new TextBlock
                {
                    Text = "Корзина пуста.",
                    FontSize = 35,
                    FontFamily = new FontFamily("Faberge"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 400,
                    Height = 50,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                cartStackPanel.Children.Add(nullCartText);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            mainStackPanel.Children.Remove(borderCart);
            Border border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 100, 0, 0),
                CornerRadius = new CornerRadius(30),
                Width = 505,
                Height = 304,
                Background = new SolidColorBrush(Color.FromRgb(255, 250, 250)),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 37,
                    Color = Colors.DarkGray,
                    ShadowDepth = 10
                }
            };
            Grid grid = new Grid();
            TextBlock textBlock = new TextBlock
            {
                Text = "Оплата",
                FontSize = 58,
                FontFamily = new FontFamily("Mustica Pro SemBd"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -3, 0, 228)
            };
            grid.Children.Add(textBlock);

            Rectangle animRectangle = new Rectangle
            {
                Width = 135,
                Height = 5,
                Fill = Brushes.Firebrick,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(125, 69, 0, 230)
            };

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 135.0,
                Duration = new Duration(TimeSpan.FromSeconds(1.65)),
                AccelerationRatio = 0.1,
                DecelerationRatio = 0.9,
                AutoReverse = false
            };
            Storyboard.SetTarget(widthAnimation, animRectangle);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Rectangle.WidthProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            animRectangle.Loaded += (s, ev) => storyboard.Begin();

            grid.Children.Add(animRectangle);

            TextBox searchBox = new TextBox
            {
                Style = (Style)FindResource("TypeInTextBox"),
                Name = "SearchBox",
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Margin = new Thickness(36, 93, 36, 0),
                Foreground = Brushes.Black,
                CaretBrush = Brushes.Black,
                SelectionBrush = new SolidColorBrush(Color.FromRgb(19, 12, 31)),
                Text = string.Empty
            };
            MaterialDesignThemes.Wpf.HintAssist.SetHint(searchBox, "Адрес ресторана...");
            grid.Children.Add(searchBox);

            GroupBox groupBox = new GroupBox
            {
                Style = (Style)FindResource("DefaultGroupBox"),
                Header = "Способ оплаты:",
                FontSize = 20,
                FontFamily = new FontFamily("Faberge"),
                Margin = new Thickness(26, 153, 310, 78)
            };

            StackPanel stackPanel = new StackPanel();
            RadioButton radioButton1 = new RadioButton
            {
                Style = (Style)FindResource("DefaultRadioButton"),
                Content = "Безналичная",
                GroupName = "OptionGroup"
            };
            radioButton1.Checked += RadioButton_Checked;

            RadioButton radioButton2 = new RadioButton
            {
                Style = (Style)FindResource("DefaultRadioButton"),
                Content = "Наличная",
                GroupName = "OptionGroup"
            };
            radioButton2.Checked += RadioButton_Checked;

            stackPanel.Children.Add(radioButton1);
            stackPanel.Children.Add(radioButton2);
            groupBox.Content = stackPanel;

            Button button = new Button
            {
                Style = (Style)FindResource("YellowButton"),
                Content = "Оплатить",
                FontSize = 24,
                Height = 36,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontFamily = new FontFamily("Faberge"),
                Cursor = System.Windows.Input.Cursors.Hand,
                Background = new SolidColorBrush(Color.FromRgb(250, 209, 47)),
                Margin = new Thickness(194, 0, 195, 23)
            };

            ListBox suggestionsBox = new ListBox
            {
                Margin = new Thickness(36, 131, 36, 26),
                FontSize = 15,
                Visibility = Visibility.Collapsed,
                Background = new SolidColorBrush(Color.FromRgb(255, 250, 250))
            };
            searchBox.TextChanged += (send, E) =>
            {
                string query = searchBox.Text;

                if (string.IsNullOrWhiteSpace(query))
                {
                    suggestionsBox.Visibility = Visibility.Collapsed;
                    suggestionsBox.ItemsSource = null;
                    restaurantName = null;
                    return;
                }

                var filteredSuggestions = suggestions
                    .Where(s => s.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filteredSuggestions.Any())
                {
                    suggestionsBox.Visibility = Visibility.Visible;
                    suggestionsBox.ItemsSource = filteredSuggestions;
                }
                else
                {
                    suggestionsBox.Visibility = Visibility.Collapsed;
                    suggestionsBox.ItemsSource = null;
                }

                restaurantName = query;
            };
            suggestionsBox.SelectionChanged += (sendr, Ev) =>
            {
                if (suggestionsBox.SelectedItem != null)
                {
                    searchBox.Text = suggestionsBox.SelectedItem.ToString();
                    suggestionsBox.Visibility = Visibility.Collapsed;
                }
            };

            TextBlock errorRestaurantName = new TextBlock
            {
                Text = "Укажите ресторан.",
                Foreground = new SolidColorBrush(Color.FromRgb(194, 48, 48)),
                FontSize = 14,
                FontFamily = new FontFamily("Faberge"),
                Margin = new Thickness(36, 136, 36, 0),
                Visibility = Visibility.Collapsed
            };
            TextBlock errorRadioButton = new TextBlock
            {
                Text = "Укажите способ оплаты.",
                Foreground = new SolidColorBrush(Color.FromRgb(194, 48, 48)),
                FontSize = 14,
                FontFamily = new FontFamily("Faberge"),
                Margin = new Thickness(36, 226, 310, 0),
                Visibility = Visibility.Collapsed
            };
            grid.Children.Add(errorRestaurantName);
            grid.Children.Add(errorRadioButton);
            grid.Children.Add(groupBox);
            grid.Children.Add(button);
            grid.Children.Add(suggestionsBox);
            button.Click += async (Sender, Event) => {
                if (restaurantName == null || restaurantName == "")
                {
                    errorRestaurantName.Visibility = Visibility.Visible;
                    TextBoxHelper.SetIsErrorState(searchBox, true);
                }
                else
                {
                    restaurantName = restaurantName.Trim();
                    errorRestaurantName.Visibility = Visibility.Collapsed;
                    TextBoxHelper.SetIsErrorState(searchBox, false);
                }
                if (radioButton1.IsChecked == false && radioButton2.IsChecked == false)
                {
                    errorRadioButton.Visibility = Visibility.Visible;
                    groupBox.BorderBrush = new SolidColorBrush(Color.FromRgb(194, 48, 48));
                    groupBox.BorderThickness = new Thickness(1);
                } else
                {
                    errorRadioButton.Visibility = Visibility.Collapsed;
                    groupBox.BorderThickness = new Thickness(0);
                }
                if (restaurantName != null && restaurantName != "" && (radioButton1.IsChecked != false || radioButton2.IsChecked != false))
                {
                    MessageBox.Show("Вы можете отслеживать статус готовности вашего заказа в корзине. Не забывайте обновлять информацию (путем повторного нажатия на вкладку \"Корзина\")!");
                    var json = JsonSerializer.Serialize($"{restaurantName}\n{PaymentMethod}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await ApiClient.Instance.PutAsync($"Users/{AuthState.Id}/purchase", content);
                    Cart cart = new Cart();
                    cart.Show();
                    Close();
                }
            };
            border.Child = grid;
            mainStackPanel.Children.Add(border);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton = sender as RadioButton;
            PaymentMethod = selectedRadioButton.Content as string;
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
