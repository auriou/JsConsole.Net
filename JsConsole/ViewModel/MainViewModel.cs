using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JsConsole.Properties;
using JsConsole.Services;

namespace JsConsole.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private WebServerService _serverWeb;
        private ClientHubService _hubBrowserKit;
        private ClientConnected _selectedConnection;
        private FileSystemWatcher watcher;
        private string _urlServer;
        private string _selectedSnippet;
        private bool _isRunning = false;
        private bool _sendToAll = true;
        
        public RelayCommand StartOrStopWebServerCommand { get; private set; }
        public RelayCommand ExecScriptCommand { get; private set; }
        public RelayCommand InsertSnippetCommand { get; private set; }
        public RelayCommand OpenSnippetCommand { get; private set; }
        
        
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                var port = Settings.Default.Port;
                //ScriptDocument = new TextDocument();
                //LogDocument = new TextDocument();
                _urlServer = PermissionService.GetUrl(port);
                StartOrStopWebServerCommand = new RelayCommand(StartOrStopWebServer);
                ExecScriptCommand = new RelayCommand(ExecScript);
                InsertSnippetCommand = new RelayCommand(InsertSnippet);
                OpenSnippetCommand = new RelayCommand(OpenSnippet);
                _serverWeb = new WebServerService(port);
                _hubBrowserKit = new ClientHubService(_urlServer, ReceiveData, ReceiveFunctions);
                BrowserKitHub.ConnectionsChanged = ConnectionsChanged;
                Messenger.Default.Register<SendModelCommand>(this, ReceiveData);

                watcher = FileService.FileSnippetsWatcher();
                watcher.Created += watcherChanged;
                watcher.Deleted += watcherChanged;
                watcher.Renamed += watcherChanged;
            }
        }

        private void ReceiveData(SendModelCommand obj)
        {
            switch (obj.Type)
            {
                case SendModelCommand.CommandType.Script:
                    var idConnection = string.Empty;
                    if (!SendToAll && SelectedConnection != null)
                        idConnection = SelectedConnection.Id;
                    _hubBrowserKit.SendScript(obj.Message, idConnection);
                    break;
                default:
                    break;
            }
        }

        void watcherChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            RaisePropertyChanged(() => Snippets);
        }

        private void OpenSnippet()
        {
            FileService.OpenSnippetsExplorer();
        }

        private void InsertSnippet()
        {
            if(SelectedSnippet != null)
                Messenger.Default.Send(new SendViewCommand() { Message = FileService.GetSnippetFilePath(SelectedSnippet), Type = SendViewCommand.CommandType.Snippet });
        }

        private void ConnectionsChanged()
        {
            RaisePropertyChanged(() => this.Connections);
        }

        private void ReceiveData(string data)
        {
            Messenger.Default.Send(new SendViewCommand(){Message  = data, Type = SendViewCommand.CommandType.Log});
        }

        private void ReceiveFunctions(string data)
        {
            Messenger.Default.Send(new SendViewCommand() { Message = data, Type = SendViewCommand.CommandType.Functions });
        }

        private void ExecScript()
        {
            Messenger.Default.Send(new SendViewCommand() { Type = SendViewCommand.CommandType.GetScript });
        }

        private void StartOrStopWebServer()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _hubBrowserKit.Stop();
                Thread.Sleep(300);
                _serverWeb.Stop();
            }
            else
            {
                IsRunning = true;
                _serverWeb.Start();
                Thread.Sleep(300);
                _hubBrowserKit.Start();
            }
            
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set { Set(() => IsRunning, ref _isRunning, value); }
        }

        public bool SendToAll
        {
            get { return _sendToAll; }
            set { Set(() => SendToAll, ref _sendToAll, value); }
        }

        //public TextDocument ScriptDocument { get; set; }
        //public TextDocument LogDocument { get; set; }

        public ObservableCollection<ClientConnected> Connections
        {
            get { return new ObservableCollection<ClientConnected>(BrowserKitHub.Connections); }
        }

        public ClientConnected SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                if (SelectedConnection != null)
                {
                    if (value == null)
                    {
                        Set(() => SelectedConnection, ref _selectedConnection, null);
                    }
                    else if (!SelectedConnection.Equals(value))
                    {
                        ReceiveFunctions(value.Functions);
                        Set(() => SelectedConnection, ref _selectedConnection, value);
                    }
                }
                else
                {
                    if (value != null)
                    {
                        ReceiveFunctions(value.Functions);
                        Set(() => SelectedConnection, ref _selectedConnection, value);
                    }
                }

            }
        }

        public string Version
        {
            get { return FileService.GetAssemblyVersion(); }
        }

        public ObservableCollection<string> Snippets
        {
            get
            {
                return FileService.GetSnippets();
            }
        }
        public string SelectedSnippet
        {
            get { return _selectedSnippet; }
            set { Set(() => SelectedSnippet, ref _selectedSnippet, value); }
        }
        public int Port
        {
            get { return Settings.Default.Port; }
            set
            {
                if (!Settings.Default.Port.Equals(value))
                {
                    RaisePropertyChanged(() => this.Port);
                    Settings.Default.Port = value;
                    Settings.Default.Save();
                }
            }
        }
    }
}