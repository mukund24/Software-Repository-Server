using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PubSource
{
    class Executive
    {
        [STAThread]
        static void Main(string[] args)
        {
            string path;
            Console.WriteLine("Enter path");
           Console.ReadLine();

            if (args.Length > 0)
                path = args[0];
            else
                path = "C:\\Users\\Mukund\\Downloads\\CSE 681\\CSE 681 Fall 11\\CODE\\";
            PubSource.FileMgr.go(path, "*.cs");
        }
    }
}
