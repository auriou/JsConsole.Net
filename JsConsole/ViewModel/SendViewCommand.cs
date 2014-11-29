namespace JsConsole.ViewModel
{
    public class SendViewCommand
    {
        public enum CommandType
        {
            Log,
            Functions,
            Snippet,
            GetScript
        }
        public string Message { get; set; }
        public CommandType Type { get; set; }
    }
}