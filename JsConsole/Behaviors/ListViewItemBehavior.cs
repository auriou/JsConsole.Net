using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace JsConsole.Behaviors
{
    public static class ListViewItemBehavior
    {

        public static bool GetIsBroughtIntoViewWhenSelected(ListView listView)
        {
            return (bool)listView.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        public static void SetIsBroughtIntoViewWhenSelected(
          ListViewItem listViewItem, bool value)
        {
            listViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "IsBroughtIntoViewWhenSelected",
            typeof(bool),
            typeof(ListViewItemBehavior),
            new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

 
        static void OnIsBroughtIntoViewWhenSelectedChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var item = depObj as ListView;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.SourceUpdated += OnSourceUpdated;
            else
                item.SourceUpdated -= OnSourceUpdated;
        }

        static void OnSourceUpdated(object sender, RoutedEventArgs e)
        {
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            var item = e.OriginalSource as ListView;
            if (item != null)
            {
                if (item.ItemsSource != null)
                {
                    ((INotifyCollectionChanged)item.ItemsSource).CollectionChanged += OncollectionChanged;
                }
            }  
        }
        private static void OncollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (item.Items.Count > 0)
            //{
            //    item.ScrollIntoView(item.Items[item.Items.Count - 1]);
            //    //item.BringIntoView();
            //}
        }
    }
}
