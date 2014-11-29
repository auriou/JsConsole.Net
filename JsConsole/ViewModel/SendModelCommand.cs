namespace JsConsole.ViewModel
{
    public class SendModelCommand
    {
        public enum CommandType
        {
            Script
        }
        public string Message { get; set; }
        public CommandType Type { get; set; }
    }
}