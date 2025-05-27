using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : Window
    {
        public History()
        {
            InitializeComponent();
            if (AuthState.isAuthenticated)
            {
                Button_Authorization.Visibility = Visibility.Hidden;
                TextBlock_Authorization.Visibility = Visibility.Visible;
                borderHistory.Visibility = Visibility.Visible;
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
                error.Margin = new Thickness(0, 100, 0, 0);
                mainStackPanel.Children.Add(error);
            }

            if (AuthState.isAuthenticated) Loaded += async (s, e) => await MakeNiceName();
        }
        private async Task MakeNiceName()
        {
            var username = await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getname");
            if (username.Length > 13) TextBlock_Authorization.Text = $"Здравствуйте,\n{username.Substring(0, 11)}...";
            else TextBlock_Authorization.Text = $"Здравствуйте,\n{username}";
        }


        private async void showProducts()
        {
            var history = await ApiClient.Instance.GetFromJsonAsync<string?>($"Users/{AuthState.Id}/gethistory");
            if (history != "" && history != null)
            {
                string[] blocks = history.Split(new[] { "\n\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < blocks.Length; j++)
                {
                    if (blocks[j] != null)
                    {
                        string[] lines = blocks[j].Split(new[] { '\n' }, StringSplitOptions.None);
                        string data = lines[0].Trim();
                        string total = lines[lines.Length - 1].Trim();

                        TextBlock datablock = new TextBlock
                        {
                            Text = $"{data}",
                            FontSize = 26,
                            FontFamily = new FontFamily("Faberge"),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            FontWeight = FontWeights.Bold,
                            Width = 250,
                            Margin = new Thickness(20, 20, 0, 0)
                        };
                        historyStackPanel.Children.Add(datablock);
                        TextBlock sum = new TextBlock
                        {
                            Text = $"{total}",
                            FontSize = 32,
                            FontFamily = new FontFamily("Faberge"),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            TextAlignment = TextAlignment.Right,
                            Margin = new Thickness(0, 0, 20, 20)
                        };

                        for (int i = 1; i < lines.Length - 2; i += 2)
                        {
                            string productLine = lines[i].Trim();
                            string imagePath = lines[i + 1].Trim();

                            try
                            {
                                if (!File.Exists(imagePath))
                                {
                                    throw new FileNotFoundException();
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                imagePath = "C:/Users/fona/source/repos/coursework3/Media/No_image_available.svg.png";
                            }

                            System.Windows.Controls.Image image = new System.Windows.Controls.Image
                            {
                                Source = new BitmapImage(new Uri($"{imagePath}")),
                                Width = 75,
                                Height = 75,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(33, 0, 0, 0)
                            };
                            TextBlock textblock = new TextBlock
                            {
                                Text = $"{productLine}",
                                FontSize = 21,
                                FontFamily = new FontFamily("Faberge"),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                TextAlignment = TextAlignment.Left,
                                TextWrapping = TextWrapping.Wrap,
                                Width = 250,
                                Margin = new Thickness(121, 0, 0, 0)
                            };

                            Grid newgrid = new Grid
                            {
                                Width = 450,
                                Height = 80
                            };
                            newgrid.Children.Add(image);
                            newgrid.Children.Add(textblock);
                            historyStackPanel.Children.Add(newgrid);
                        }

                        historyStackPanel.Children.Add(sum);

                        if (j < blocks.Length - 1)
                        {
                            Rectangle splitLine = new Rectangle
                            {
                                Height = 2,
                                Fill = new SolidColorBrush(Color.FromRgb(130, 130, 130)),
                                VerticalAlignment = VerticalAlignment.Bottom,
                            };
                            historyStackPanel.Children.Add(splitLine);
                        }
                    }
                }
            }
            else
            {
                TextBlock nullHistoryText = new TextBlock
                {
                    Text = "История заказов пуста.",
                    FontSize = 35,
                    FontFamily = new FontFamily("Faberge"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 500,
                    Height = 50,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(0, 10, 0, 0)
                };
                historyStackPanel.Children.Add(nullHistoryText);
            }
        }

        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            History history = new History();
            history.Show();
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
        private void Button_Restaurants_Click(object sender, RoutedEventArgs e)
        {
            Restaurants rest = new Restaurants();
            rest.Show();
            Close();
        }
    }
}
