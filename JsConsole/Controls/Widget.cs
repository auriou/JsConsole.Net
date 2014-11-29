using System.Windows;
using System.Windows.Controls;

namespace JsConsole.Controls
{
    public class Widget : HeaderedContentControl
    {
        public static readonly DependencyProperty IconeTemplateProperty = DependencyProperty.Register(
            "IconeTemplate", typeof(DataTemplate), typeof(Widget), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Widget), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public DataTemplate IconeTemplate
        {
            get { return (DataTemplate) GetValue(IconeTemplateProperty); }
            set { SetValue(IconeTemplateProperty, value); }
        }
    }
}
