using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PubSource
{
   public class FileMgr
    {
        public static void go(string path, string pattern)
        {
            path = Path.GetFullPath(path);
            Console.Write("\n\n  {0}", path);

            // get all files in this directory and display them

            string[] files = Directory.GetFiles(path, pattern); 
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                FileInfo fi = new FileInfo(file);
                DateTime dt = File.GetLastWriteTime(file);
                Console.Write("\n   {0,-20} {1,8} bytes  {2}", name, fi.Length, dt);
            }
            // for each subdirectory in this directory display its files
            // recursively

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
                go(dir, pattern);
        }
    }
}
