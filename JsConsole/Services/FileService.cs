using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JsConsole.Services
{
    public class FileService
    {
        private static readonly string SnippetPath = AppDomain.CurrentDomain.BaseDirectory + "Snippets";

        static FileService()
        {
            if (!Directory.Exists(SnippetPath))
            {
                Directory.CreateDirectory(SnippetPath);
            }
        }
        public static void OpenExplorer(string file)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                             {
                                                 FileName = "explorer.exe",
                                                 Arguments = file
                                             });
        }

        public static void OpenSnippetsExplorer()
        {
            OpenExplorer(SnippetPath);
        }

        public static ObservableCollection<string> GetSnippets()
        {
            if (Directory.Exists(SnippetPath))
            {
                var res = Directory.GetFiles(SnippetPath, "*.js").Select(Path.GetFileNameWithoutExtension).Where(p => !String.IsNullOrEmpty(p)).ToList();
                return new ObservableCollection<string>(res);
            }
            return new ObservableCollection<string>();
        }

        public static string GetSnippetFilePath(string name)
        {
            return String.Format("{0}\\{1}.js", SnippetPath, name);
        }


        public static string GetAssemblyVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var res = string.Format("v{0}.{1}.{2} by p.auriou", version.Major, version.Minor, version.Build);
            return res;
        }

        public static FileSystemWatcher FileSnippetsWatcher()
        {
            var fileSystemWatcher = new FileSystemWatcher {Path = SnippetPath, Filter = "*.js", EnableRaisingEvents = true};
            return fileSystemWatcher;

        }
    }
}