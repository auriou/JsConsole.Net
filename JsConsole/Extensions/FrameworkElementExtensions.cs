using System;
using System.Windows;

namespace JsConsole.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void DoWhenLoaded<T>(this T element, Action<T> action) where T : FrameworkElement
        {
            if (element.IsLoaded)
            {
                action(element);
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = delegate(object sender, RoutedEventArgs e)
                          {
                              element.Loaded -= handler;
                              action(element);
                          };
                element.Loaded += handler;
            }
        }

        public static void DoWhenLoaded(this FrameworkElement element, Action action)
        {
            if (element.IsLoaded)
            {
                action();
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = delegate(object sender, RoutedEventArgs e)
                          {
                              element.Loaded -= handler;
                              action();
                          };
                element.Loaded += handler;
            }
        }
    }
}