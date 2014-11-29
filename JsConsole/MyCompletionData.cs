using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace JsConsole
{
    public class MyCompletionData : ICompletionData
    {
        private ImageType _type;
        public static ImageSource MethoImage = LoadBitmap("Method");
        public static ImageSource ClassImage = LoadBitmap("Class");
        public MyCompletionData(string text)
        {
            this.Text = text;
            _type = text.Contains(")") ? ImageType.Method : ImageType.Class;
        }

        public MyCompletionData(string text, ImageType type)
        {
            this.Text = text;
            _type = type;
        }

        public enum ImageType
        {
            Class,
            Method
        }

        public ImageSource Image
        {
            get { return _type == ImageType.Class ? ClassImage : MethoImage; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return null; }
        }

        public double Priority { get; private set; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        static BitmapImage LoadBitmap(string name)
        {
            var image = new BitmapImage(new Uri("pack://application:,,,/Images/" + name + ".png"));
            image.Freeze();
            return image;
        }
    }
}