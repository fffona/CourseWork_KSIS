using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWorkClient
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private string category = "Бургеры и Роллы";
        public Menu()
        {
            InitializeComponent();
            if (AuthState.isAuthenticated)
            {
                Button_Authorization.Visibility = Visibility.Hidden;
                TextBlock_Authorization.Visibility = Visibility.Visible;
            }
            MakeCategoryName("Бургеры и Роллы");
            PerformProduct("burgers", 0);

            if (AuthState.isAuthenticated) Loaded += async (s, e) => await MakeNiceName();
        }

        private async Task MakeNiceName()
        {
            var username = await ApiClient.Instance.GetFromJsonAsync<string>($"Users/{AuthState.Id}/getname");
            if (username.Length > 13) TextBlock_Authorization.Text = $"Здравствуйте,\n{username.Substring(0, 11)}...";
            else TextBlock_Authorization.Text = $"Здравствуйте,\n{username}";
        }

        private void MakeCategoryName(string name)
        {
            TextBlock categoryName = new TextBlock
            {
                Text = name,
                FontSize = 40,
                FontFamily = new FontFamily("Faberge"),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center
            };
            categoryName.Margin = new Thickness(0, -20, 0, 0);

            menuList.Children.Add(categoryName);
            Grid.SetColumn(categoryName, 1);
        }
        private void ClearGrid()
        {
            int row = 0;
            int column = 0;
            while (row < menuList.RowDefinitions.Count && column < menuList.ColumnDefinitions.Count)
            {
                if (menuList.Children.Count != 0)
                {
                    menuList.Children.Clear();
                    column++;
                    if (column >= menuList.ColumnDefinitions.Count)
                    {
                        column = 0;
                        row++;
                    }
                }
                else break;
            }
        }
        private async void PerformProduct(string prodType, int numSort)
        {
            var prod = await ApiClient.Instance.GetFromJsonAsync<List<Product>>($"Products/get/{prodType}");
            if (numSort != 0)
            {
                foreach (var one in prod)
                {
                    if (one.Cost.Contains("от"))
                    {
                        one.Cost = one.Cost.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(' ')[1];
                    }
                }
            }
            if (numSort == 1)
            {
                prod.Sort((x, y) => Convert.ToDouble(x.Cost, CultureInfo.InvariantCulture).CompareTo(Convert.ToDouble(y.Cost, CultureInfo.InvariantCulture)));
                foreach (var one in prod)
                {
                    one.Cost = await ApiClient.Instance.GetFromJsonAsync<string?>($"Products/{one.Name}/getcost");
                }
            } else if (numSort == 2)
            {
                prod.Sort((x, y) => Convert.ToDouble(y.Cost, CultureInfo.InvariantCulture).CompareTo(Convert.ToDouble(x.Cost, CultureInfo.InvariantCulture)));
                foreach (var one in prod)
                {
                    one.Cost = await ApiClient.Instance.GetFromJsonAsync<string?>($"Products/{one.Name}/getcost");
                }
            }
            int row = 0;
            int column = 0;
            foreach (var one in prod)
            {
                Button button = new Button
                {
                    Style = (Style)FindResource("AnimButton")
                };
                button.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri($"{one.Image}")),
                    Stretch = Stretch.Uniform,
                    Viewport = new Rect(0.22, 0.14, 0.55, 0.55)
                };
                TextBlock textblock = new TextBlock
                {
                    Text = $"{one.Name}\n{one.Cost.Split('\n')[0].Trim()} руб.",
                    FontSize = 25,
                    FontFamily = new FontFamily("Faberge"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                button.Content = textblock;
                button.Click += (sender, e) =>
                {
                    Item item = new Item(one);
                    item.Show();
                    Close();
                };

                menuList.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                column++;
                if (column >= menuList.ColumnDefinitions.Count)
                {
                    column = 0;
                    row++;
                }
            }
        }

        private void RectChange(int num)
        {
            rect1.Visibility = Visibility.Hidden;
            rect2.Visibility = Visibility.Hidden;
            rect3.Visibility = Visibility.Hidden;
            rect4.Visibility = Visibility.Hidden;
            rect5.Visibility = Visibility.Hidden;
            rect6.Visibility = Visibility.Hidden;
            rect7.Visibility = Visibility.Hidden;
            rect8.Visibility = Visibility.Hidden;
            rect9.Visibility = Visibility.Hidden;
            switch(num)
            {
                case 1:
                    rect1.Visibility = Visibility.Visible;
                    break;
                case 2:
                    rect2.Visibility = Visibility.Visible;
                    break;
                case 3:
                    rect3.Visibility = Visibility.Visible;
                    break;
                case 4:
                    rect4.Visibility = Visibility.Visible;
                    break;
                case 5:
                    rect5.Visibility = Visibility.Visible;
                    break;
                case 6:
                    rect6.Visibility = Visibility.Visible;
                    break;
                case 7:
                    rect7.Visibility = Visibility.Visible;
                    break;
                case 8:
                    rect8.Visibility = Visibility.Visible;
                    break;
                case 9:
                    rect9.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ButtonBurger_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Бургеры и Роллы";
            MakeCategoryName("Бургеры и Роллы");
            PerformProduct("burgers", 0);
            RectChange(1);
        }

        private void ButtonChicken_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Курочка";
            MakeCategoryName("Курочка");
            PerformProduct("chicken", 0);
            RectChange(2);
        }

        private void ButtonFries_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Картошка";
            MakeCategoryName("Картошка");
            PerformProduct("fries", 0);
            RectChange(3);
        }

        private void ButtonRodnoyVkus_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Родной вкус";
            MakeCategoryName("Родной вкус");
            PerformProduct("rodnoyvkus", 0);
            RectChange(4);
        }

        private void ButtonBreakfast_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Завтрак до 10:00";
            MakeCategoryName("Завтрак до 10:00");
            PerformProduct("breakfast", 0);
            RectChange(5);
        }

        private void ButtonDrinks_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Напитки";
            MakeCategoryName("Напитки");
            PerformProduct("drinks", 0);
            RectChange(6);
        }

        private void ButtonDeserts_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Десерты";
            MakeCategoryName("Десерты");
            PerformProduct("deserts", 0);
            RectChange(7);
        }

        private void ButtonKidsMeal_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Кидс Мил";
            MakeCategoryName("Кидс Мил");
            PerformProduct("kidsmeal", 0);
            RectChange(8);
        }

        private void ButtonSets_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            category = "Сеты";
            MakeCategoryName("Сеты");
            PerformProduct("sets", 0);
            RectChange(9);
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

        private void Button_Sort1_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            switch (category)
            {
                case "Бургеры и Роллы":
                    category = "Бургеры и Роллы";
                    MakeCategoryName("Бургеры и Роллы");
                    PerformProduct("burgers", 1);
                    break;
                case "Курочка":
                    category = "Курочка";
                    MakeCategoryName("Курочка");
                    PerformProduct("chicken", 1);
                    break;
                case "Картошка":
                    category = "Картошка";
                    MakeCategoryName("Картошка");
                    PerformProduct("fries", 1);
                    break;
                case "Родной вкус":
                    category = "Родной вкус";
                    MakeCategoryName("Родной вкус");
                    PerformProduct("rodnoyvkus", 1);
                    break;
                case "Завтрак до 10:00":
                    category = "Завтрак до 10:00";
                    MakeCategoryName("Завтрак до 10:00");
                    PerformProduct("breakfast", 1);
                    break;
                case "Напитки":
                    category = "Напитки";
                    MakeCategoryName("Напитки");
                    PerformProduct("drinks", 1);
                    break;
                case "Десерты":
                    category = "Десерты";
                    MakeCategoryName("Десерты");
                    PerformProduct("deserts", 1);
                    break;
                case "Кидс Мил":
                    category = "Кидс Мил";
                    MakeCategoryName("Кидс Мил");
                    PerformProduct("kidsmeal", 1);
                    break;
                case "Сеты":
                    category = "Сеты";
                    MakeCategoryName("Сеты");
                    PerformProduct("sets", 1);
                    break;
            }
            Sort1Button.Visibility = Visibility.Collapsed;
            Sort2Button.Visibility = Visibility.Visible;
        }

        private void Button_Sort2_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
            switch (category)
            {
                case "Бургеры и Роллы":
                    category = "Бургеры и Роллы";
                    MakeCategoryName("Бургеры и Роллы");
                    PerformProduct("burgers", 2);
                    break;
                case "Курочка":
                    category = "Курочка";
                    MakeCategoryName("Курочка");
                    PerformProduct("chicken", 2);
                    break;
                case "Картошка":
                    category = "Картошка";
                    MakeCategoryName("Картошка");
                    PerformProduct("fries", 2);
                    break;
                case "Родной вкус":
                    category = "Родной вкус";
                    MakeCategoryName("Родной вкус");
                    PerformProduct("rodnoyvkus", 2);
                    break;
                case "Завтрак до 10:00":
                    category = "Завтрак до 10:00";
                    MakeCategoryName("Завтрак до 10:00");
                    PerformProduct("breakfast", 2);
                    break;
                case "Напитки":
                    category = "Напитки";
                    MakeCategoryName("Напитки");
                    PerformProduct("drinks", 2);
                    break;
                case "Десерты":
                    category = "Десерты";
                    MakeCategoryName("Десерты");
                    PerformProduct("deserts", 2);
                    break;
                case "Кидс Мил":
                    category = "Кидс Мил";
                    MakeCategoryName("Кидс Мил");
                    PerformProduct("kidsmeal", 2);
                    break;
                case "Сеты":
                    category = "Сеты";
                    MakeCategoryName("Сеты");
                    PerformProduct("sets", 2);
                    break;
            }
            Sort1Button.Visibility = Visibility.Visible;
            Sort2Button.Visibility = Visibility.Collapsed;
        }
    }
}
