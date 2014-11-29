using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using JsConsole.ViewModel;
using Microsoft.AspNet.SignalR;

namespace JsConsole
{
    public partial class MainWindow
    {
        private List<string> _completationData;
        private CompletionWindow _completionWindow;
        public MainWindow()
        {
            InitializeComponent();

            // Make long polling connections wait a maximum of 110 seconds for a
            // response. When that time expires, trigger a timeout command and
            // make the client reconnect.
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            // Wait a maximum of 30 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

            // For transports other than long polling, send a keepalive packet every
            // 10 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);


            //ClientListBox.Items.Add("TEXTE");
            
            Messenger.Default.Register<SendViewCommand>(this, ReceiveData);
            Assembly assembly = GetType().Assembly;
            using (
                Stream s = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + "CustomHighlighting.xshd")
                )
            {
                using (var reader = new XmlTextReader(s))
                {
                    LogEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            var ctrlSpace = new RoutedCommand();
            ctrlSpace.InputGestures.Add(new KeyGesture(Key.Space, ModifierKeys.Control));
            var cb = new CommandBinding(ctrlSpace, OnCtrlSpaceCommand);
            CommandBindings.Add(cb);

            JsEditor.TextArea.TextEntered += OnTextEntered;

            _completationData = new List<string>();
            //_completationData.Add("Item1()");
            //_completationData.Add("OtherItem()");
            //_completationData.Add("Itemfdfsfsdfsdfsdfdsfdsfdsfsdfsdfds()");
            //_completationData.Add("object.test()");
            //_completationData.Add("object.log()");
            //_completationData.Add("object.obj1.log()");
            //_completationData.Add("object.obj.log()");
           
        }


        private void OnCtrlSpaceCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OnCompletion(null, true);
        }

        private void OnTextEntered(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            OnCompletion(textCompositionEventArgs.Text, false);
        }

        public static string GetLeftString(string str, char separ, int posi)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            var res = str;
            var xPosi = -1;
            for (var i = 0; i < posi + 1; i++)
            {
                xPosi = str.IndexOf(separ, xPosi + 1);
                if (xPosi == -1)
                {
                    break;
                }
            }
            if (xPosi > 0)
            {
                res = str.Substring(0, xPosi);
            }
            return res;
        }

        private void OnCompletion(string enteredText, bool controlSpace)
        {
            if (!controlSpace & (enteredText == null || enteredText == "\n" || string.IsNullOrWhiteSpace(enteredText)))
            {
                if (_completionWindow != null)
                {
                    _completionWindow.Close();
                }
                return;
            }

            Debug.WriteLine("TEXT " + enteredText);

            #region get last word into var text
            int offset = JsEditor.TextArea.Caret.Offset;
            int i = 0;
            string text = "";
            while ((offset - i) > 0)
            {
                i++;
                text = JsEditor.Document.GetText(offset - i, i);
                if (text.StartsWith(" ") || text.StartsWith("\n"))
                {
                    text = text.Substring(1);
                    break;
                }
            }
            #endregion


            #region create completation if exist correspondance

            const char separ = '.';
            var nbObj = text.Count(p => p == '.');

            var cplCompletionDatas =
                _completationData.Where(u => u.ToLower().StartsWith(text.ToLower())).
                    Select(x => GetLeftString(x, separ, nbObj)).OrderBy(p => p).Distinct().ToList();

            if (cplCompletionDatas.Count > 0)
            {
                _completionWindow = new CompletionWindow(JsEditor.TextArea);
                _completionWindow.MinWidth = 300;
                //completionWindow.Width = Double.NaN;

                //_completionWindow.CloseWhenCaretAtBeginning = controlSpace;
                _completionWindow.StartOffset = offset - text.Length;
                foreach (var cplCompletionData in cplCompletionDatas)
                {
                    _completionWindow.CompletionList.CompletionData.Add(new MyCompletionData(cplCompletionData));
                }

                _completionWindow.CompletionList.SelectItem(text);
                if (_completionWindow.CompletionList.SelectedItem != null)
                {
                    _completionWindow.Show();
                    _completionWindow.Closed += delegate { _completionWindow = null; };
                }
            }
            else
            {
                if(_completionWindow != null)
                    _completionWindow.Close();
            }
            #endregion
        }


        public void ReceiveData(SendViewCommand obj)
        {
            switch (obj.Type)
            {
                case SendViewCommand.CommandType.GetScript:
                    var script = !string.IsNullOrWhiteSpace(JsEditor.SelectedText) ? JsEditor.SelectedText : JsEditor.Text;
                    Messenger.Default.Send(new SendModelCommand() { Message = script, Type = SendModelCommand.CommandType.Script });
                    break;
                case SendViewCommand.CommandType.Snippet:
                    using (var stream = new StreamReader(obj.Message))
                    {
                        var res = stream.ReadToEnd();
                        SendText(JsEditor, res);
                    }
                    break;
                case SendViewCommand.CommandType.Functions:
                    if (string.IsNullOrEmpty(obj.Message))
                        return;
                    _completationData.Clear();
                    var resSplit = obj.Message.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var resString = "";
                    var line = "   ";
                    foreach (var fnStr in resSplit)
                    {
                        _completationData.Add(fnStr);
                        line += fnStr + ";";
                        if (line.Length > 40)
                        {
                            resString += line + Environment.NewLine;
                            line = "   ";
                        }
                    }
                    resString = "{{Functions:\n" + resString + "}}";
                    SendText(LogEditor, resString);
                    break;
                case SendViewCommand.CommandType.Log:
                    SendText(LogEditor, obj.Message);
                    break;
                default:
                    SendText(LogEditor, obj.Message);
                    break;
            }
        }

        private void SendText(TextEditor editor, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            editor.Dispatcher.Invoke(() =>
            {
                if (editor.Document.TextLength > 0)
                    editor.Document.Insert(editor.Document.TextLength, Environment.NewLine);
                editor.Document.Insert(editor.Document.TextLength, text);
                editor.ScrollToLine(editor.Document.LineCount);
            });
        }

        private void ClearLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            LogEditor.Clear();
        }

        private void ClearJsButton_OnClick(object sender, RoutedEventArgs e)
        {
            JsEditor.Clear();
        }
    }
}