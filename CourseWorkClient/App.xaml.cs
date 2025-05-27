using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace CourseWorkClient
{
    public static class AuthState
    {
        public static bool isAuthenticated { get; set; }
        public static int Id { get; set; }
    }

    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty IsErrorStateProperty =
        DependencyProperty.RegisterAttached(
        "IsErrorState",
        typeof(bool),
        typeof(PasswordBoxHelper),
        new PropertyMetadata(false));

        public static bool GetIsErrorState(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsErrorStateProperty);
        }

        public static void SetIsErrorState(DependencyObject obj, bool value)
        {
            obj.SetValue(IsErrorStateProperty, value);
        }
    }

    public static class TextBoxHelper
    {
        public static readonly DependencyProperty IsErrorStateProperty =
        DependencyProperty.RegisterAttached(
        "IsErrorState",
        typeof(bool),
        typeof(TextBoxHelper),
        new PropertyMetadata(false));

        public static bool GetIsErrorState(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsErrorStateProperty);
        }

        public static void SetIsErrorState(DependencyObject obj, bool value)
        {
            obj.SetValue(IsErrorStateProperty, value);
        }
    }

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}
