using CommunityToolkit.Mvvm.Input;
using Cupertino.Support.Local.Helpers;
using Cupertino.Support.Local.Models;
using Jamesnet.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cupertino.Forms.Local.ViewModels
{
    public partial class CupertinoWindowViewModel : ObservableBase
    {
        public List<FileItem> Files { get; set; }
        public CupertinoWindowViewModel()
        {
            FileCreator fileCreator = new();
            fileCreator.Create();

            int depth = 0;
            string root = fileCreator.BasePath + "/Vicky";
            List<FileItem> source = new();

            GetFiles(root, source, depth);

            Files = source;
        }

        private void GetFiles(string root, List<FileItem> source, int depth)
        {
            string[] dirs = Directory.GetDirectories(root);
            string[] files = Directory.GetFiles(root);

            foreach (string dir in dirs )
            {
                FileItem item = new();
                item.Name = Path.GetFileNameWithoutExtension(dir);
                item.Path = dir;
                item.Size = null;
                item.Type = "Folder";
                item.Depth = depth;
                item.Children = new();

                source.Add(item);

                GetFiles(dir, item.Children, depth + 1);
            }

            foreach (string file in files)
            {
                FileItem item = new();
                item.Name = Path.GetFileNameWithoutExtension(file);
                item.Path = file;
                item.Size = new FileInfo(file).Length;
                item.Type = "File";
                item.Extension = new FileInfo(file).Extension;
                item.Depth = depth;

                source.Add(item);
            }
        }

        [RelayCommand]
        private void Selection(FileItem item)
        {
            string name = item.Name;
        }
    }
}
