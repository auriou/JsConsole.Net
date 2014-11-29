using Microsoft.Owin.StaticFiles.ContentTypes;

namespace JsConsole.Services
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            //Mappings.Add(".txt", "text/plain");
        }
    }
}