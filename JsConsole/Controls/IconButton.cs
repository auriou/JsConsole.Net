using System.Windows;
using System.Windows.Controls;

namespace JsConsole.Controls
{
    public class IconButton : RadioButton
    {
        public static readonly DependencyProperty IconeTemplateProperty = DependencyProperty.Register(
            "IconeTemplate", typeof(DataTemplate), typeof(IconButton), new PropertyMetadata(default(DataTemplate)));


        public DataTemplate IconeTemplate
        {
            get { return (DataTemplate)GetValue(IconeTemplateProperty); }
            set { SetValue(IconeTemplateProperty, value); }
        }
    }
}