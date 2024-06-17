using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cupertino.Support.Local.Models
{
    public class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public long? Size { get; set; }
        public int Depth { get; set; }
        public List<FileItem> Children { get; set; }
    }
}
