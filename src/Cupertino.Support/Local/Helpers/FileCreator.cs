using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cupertino.Support.Local.Helpers
{
    public class FileCreator
    {
        public string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public void Create()
        {
            string textData = "Vicky test file content.";

            string[] tempFiles = 
            {
                @"\Vicky\Microsoft\Visual Studio\solution.txt",
                @"\Vicky\Microsoft\Visual Studio\debug.mp3",
                @"\Vicky\Microsoft\Visual Studio\class.cs",
                @"\Vicky\Microsoft\Sql Management Studio\query.txt",
                @"\Vicky\Apple\iPhone\store.txt",
                @"\Vicky\Apple\iPhone\calculator.mp3",
                @"\Vicky\Apple\iPhone\safari.cs",
            };

            foreach (string file in tempFiles)
            { 
                string fullPath = BasePath + file;
                string dirName = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                File.WriteAllText(fullPath, textData);
            }
        }
    }
}
