using System.Windows;
using System.Windows.Controls;

namespace JsConsole.Controls
{
    public class IconHeader : ContentControl
    {
        public static readonly DependencyProperty IconeTemplateProperty = DependencyProperty.Register(
            "IconeTemplate", typeof(DataTemplate), typeof(IconHeader), new PropertyMetadata(default(DataTemplate)));


        public DataTemplate IconeTemplate
        {
            get { return (DataTemplate)GetValue(IconeTemplateProperty); }
            set { SetValue(IconeTemplateProperty, value); }
        }
    }
}