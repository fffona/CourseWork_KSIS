using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
    /// Логика взаимодействия для Cashier.xaml
    /// </summary>
    public partial class Cashier : Window
    {

        public Cashier()
        {
            InitializeComponent();
        }
        public Cashier(string place)
        {
            InitializeComponent();
            borderCashier.Visibility = Visibility.Visible;
            showOrders(place);
        }

        private async void showOrders(string place)
        {
            int usersCount = 0;
            var Users = await ApiClient.Instance.GetFromJsonAsync<ObservableCollection<User>>("Users") ?? new ObservableCollection<User>();
            foreach (User user in Users)
            {
                string[] placeParts;
                if (user.Place != null)
                {
                    placeParts = user.Place.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                } else
                {
                    continue;
                }
                double result = 0;
                if ((user.PurchaseStatus == 1 || user.PurchaseStatus == 2 || user.PurchaseStatus == 3) && placeParts[0].Trim() == place)
                {
                    string[] prods = user.Cart.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string prod in prods)
                    {
                        if (prod != null)
                        {
                            string[] cartParts = prod.Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                            string cartName = cartParts[0].Trim();
                            if (cartParts.Length == 3) cartName = Regex.Replace(cartName, @"\s*\(.*?\)", "");
                            var response = await ApiClient.Instance.GetAsync($"Products/{cartName}/getcost");
                            if (response.IsSuccessStatusCode)
                            {
                                var cost = await response.Content.ReadFromJsonAsync<string?>();
                                var image_request = await ApiClient.Instance.GetFromJsonAsync<string>($"Products/{cartName}/getimage");
                                string amount = cartParts[1];
                                if (cartParts.Length == 3) result += Convert.ToInt16(cartParts[1]) * Convert.ToDouble(cartParts[2], CultureInfo.InvariantCulture);
                                else result += Convert.ToInt16(cartParts[1]) * Convert.ToDouble(cost, CultureInfo.InvariantCulture);

                                TextBlock textblock = new TextBlock
                                {
                                    Text = $"{cartParts[0]} ({cartParts[1]} шт.)",
                                    FontSize = 21,
                                    FontFamily = new FontFamily("Faberge"),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    TextAlignment = TextAlignment.Left,
                                    TextWrapping = TextWrapping.Wrap,
                                    Width = 335,
                                    Margin = new Thickness(108, 0, 0, 0)
                                };
                                System.Windows.Controls.Image image = new System.Windows.Controls.Image
                                {
                                    Source = new BitmapImage(new Uri($"{image_request}")),
                                    Width = 75,
                                    Height = 75,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(20, 0, 0, 0)
                                };
                                Grid newgrid = new Grid
                                {
                                    Width = 450,
                                    Height = 80,
                                    Margin = new Thickness(0, 10, 0, 0)
                                };
                                newgrid.Children.Add(image);
                                newgrid.Children.Add(textblock);
                                cashierStackPanel.Children.Add(newgrid);
                            }
                        }
                    }
                    if (result != 0)
                    {
                        usersCount++;
                        TextBlock sum = new TextBlock
                        {
                            Text = $"{result} р.",
                            FontSize = 32,
                            FontFamily = new FontFamily("Faberge"),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Top,
                            TextAlignment = TextAlignment.Right,
                        };
                        TextBlock paymentMethod = new TextBlock
                        {
                            Text = $"{placeParts[1]}",
                            FontSize = 17,
                            FontFamily = new FontFamily("Faberge"),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom,
                        };
                        Grid payGrid = new Grid
                        {
                            Width = 100,
                            Height = 50,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(0, 0, 20, 0)
                        };
                        payGrid.Children.Add(paymentMethod);
                        payGrid.Children.Add(sum);
                        cashierStackPanel.Children.Add(payGrid);

                        if (user.PurchaseStatus == 1)
                        {
                            Button confirmButton = new Button
                            {
                                Width = 90,
                                Height = 30,
                                Content = "Принять",
                                FontFamily = new FontFamily("Faberge"),
                                FontSize = 19,
                                BorderThickness = new Thickness(0),
                                Cursor = Cursors.Hand,
                                Foreground = Brushes.White,
                                Background = new SolidColorBrush(Color.FromRgb(64, 145, 10)),
                                HorizontalAlignment = HorizontalAlignment.Left
                            };
                            confirmButton.Style = (Style)FindResource("GreenButton");
                            Button declineButton = new Button
                            {
                                Width = 110,
                                Height = 30,
                                Content = "Отклонить",
                                FontFamily = new FontFamily("Faberge"),
                                FontSize = 19,
                                Foreground = Brushes.White,
                                BorderThickness = new Thickness(0),
                                Cursor = Cursors.Hand,
                                Margin = new Thickness(95, 0, 0, 0),
                                Background = Brushes.Firebrick,
                                HorizontalAlignment = HorizontalAlignment.Right
                            };
                            declineButton.Style = (Style)FindResource("RedButton");
                            confirmButton.Click += async (sender, e) =>
                            {
                                user.PurchaseStatus = 2;
                                await ApiClient.Instance.PutAsync($"Users/{user.id}/purchasestatus", null);
                                cashierStackPanel.Children.Clear();
                                showOrders(place);
                            };
                            declineButton.Click += async (sender, e) =>
                            {
                                user.PurchaseStatus = -1;
                                var json = JsonSerializer.Serialize(user);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                await ApiClient.Instance.PutAsync("Users", content);
                                cashierStackPanel.Children.Clear();
                                showOrders(place);
                            };
                            Grid answerButtons = new Grid()
                            {
                                Width = 210,
                                Height = 30,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Margin = new Thickness(20, 0, 0, 20)
                            };
                            answerButtons.Children.Add(declineButton);
                            answerButtons.Children.Add(confirmButton);
                            cashierStackPanel.Children.Add(answerButtons);
                        }
                        if (user.PurchaseStatus == 2)
                        {
                            Button readyButton = new Button
                            {
                                Width = 90,
                                Height = 30,
                                Content = "Готов",
                                FontFamily = new FontFamily("Faberge"),
                                FontSize = 19,
                                Foreground = Brushes.White,
                                Cursor = Cursors.Hand,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Margin = new Thickness(20, 0, 0, 20),
                                Background = new SolidColorBrush(Color.FromRgb(64, 145, 10))
                            };
                            readyButton.Style = (Style)FindResource("GreenButton");
                            readyButton.Click += async (sender, e) =>
                            {
                                user.PurchaseStatus = 3;
                                await ApiClient.Instance.PutAsync($"Users/{user.id}/purchasestatus", null);
                                cashierStackPanel.Children.Clear();
                                showOrders(place);
                            };
                            cashierStackPanel.Children.Add(readyButton);
                        }
                        if (user.PurchaseStatus == 3)
                        {
                            Button overButton = new Button
                            {
                                Width = 90,
                                Height = 30,
                                Content = "Отдан",
                                FontFamily = new FontFamily("Faberge"),
                                FontSize = 19,
                                Foreground = Brushes.White,
                                Cursor = Cursors.Hand,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Margin = new Thickness(20, 0, 0, 20),
                                Background = new SolidColorBrush(Color.FromRgb(64, 145, 10))
                            };
                            overButton.Style = (Style)FindResource("GreenButton");
                            overButton.Click += async (sender, e) =>
                            {
                                user.PurchaseStatus = 0;
                                DateTime today = DateTime.Today;
                                string formattedDate = today.ToString("dd.MM.yyyy");
                                user.History += $"{formattedDate}\n";
                                foreach (string prod in prods)
                                {
                                    string[] cartParts = prod.Trim().Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                    string cartName = cartParts[0];
                                    if (cartParts.Length == 3) cartName = Regex.Replace(cartName, @"\s*\(.*?\)", "");
                                    var response = await ApiClient.Instance.GetAsync($"Products/{cartName}/getimage");
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var image_request = await response.Content.ReadFromJsonAsync<string>();
                                        user.History += $"{cartParts[0]} ({cartParts[1]} шт.)\n{image_request}\n";
                                    }
                                }
                                user.History += $"{result} руб.\n\n\n";
                                user.Cart = null;
                                var json = JsonSerializer.Serialize(user);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                await ApiClient.Instance.PutAsync("Users", content);
                                cashierStackPanel.Children.Clear();
                                showOrders(place);
                            };
                            cashierStackPanel.Children.Add(overButton);
                        }

                        Rectangle splitLine = new Rectangle
                        {
                            Height = 1,
                            Width = 390,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Fill = Brushes.DarkGray,
                            VerticalAlignment = VerticalAlignment.Bottom,
                        };
                        cashierStackPanel.Children.Add(splitLine);
                    }
                }
            }
            if (usersCount == 0)
            {
                TextBlock nullCashierText = new TextBlock
                {
                    Text = "Пока нет заказов.",
                    FontSize = 35,
                    FontFamily = new FontFamily("Faberge"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 400,
                    Height = 50,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                cashierStackPanel.Children.Add(nullCashierText);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
