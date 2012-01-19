///////////////////////////////////////////////////////////////////////
///  Navigate.cs - Navigates a Directory Subtree, displaying files  ///
///  ver 1.2       and some of their properties                     ///
///                                                                 ///
///  Language:     Visual C#                                        ///
///  Platform:     Dell Dimension 8100, Windows Pro 2000, SP2       ///
///  Application:  CSE681 Example                                   ///
///  Author:       Jim Fawcett, CST 2-187, Syracuse Univ.           ///
///                (315) 443-3948, jfawcett@twcny.rr.com            ///
///////////////////////////////////////////////////////////////////////
/*
 *  Module Operations:
 *  ==================
 *  Recursively displays the contents of a directory tree
 *  rooted at a specified path, with specified file pattern.
 *
 *  Public Interface:
 *  =================
 *  Navigate nav = new Navigate();
 *  nav.go("c:\temp","*.cs");
 * 
 *  Maintenance History:
 *  ====================
 *  ver 2.0 : 15 Sep 11
 *  - converted go to a non-static member function so it can access
 *    a List<string> data member
 *  - added private List<string> member to store all the files
 *    found in all searched directories (stores fully qualified names)
 *  - callers now need to create a Navigate nav object to use function go
 *  - added function List<string> getSources() to retrieve the file specs
 *  ver 1.2 : 10 Sep 11
 *  - removed unnecessary SetCurrentDirectory in Navigate.go()
 *  ver 1.1 : 04 Sep 06
 *  - added file pattern as argument to member function go()
 *  ver 1.0 : 05 Sep 05
 *  - first release
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Project2
{
      public class Navigate
      {
        List<string> sourceCode = new List<string>();

        public List<string> getSources()
        {
          return sourceCode;
        }
        public void go(string path, string pattern)
        {
          path = Path.GetFullPath(path);

          // get all files in this directory and save them
          try
          {
              string[] files = Directory.GetFiles(path, pattern);
              sourceCode.AddRange(files);
              string[] dirs = Directory.GetDirectories(path);
              foreach (string dir in dirs)
                  go(dir, pattern);
          
          }
          catch
          {
              return;
          }
        }

    #if (TEST_NAVIGATE)
        static void Main(string[] args)
        {

            Console.WriteLine("\nTesting FILE FINDER");
            Console.WriteLine("====================\n");

            string path;
            int count=0;
                if (args.Length > 0)
                    path = args[0];
                else
                    path = Directory.GetCurrentDirectory();

            Navigate nav = new Navigate();
            nav.go(path, "*.cs");

            List<string> files = nav.getSources(); // fully qualified filenames

                foreach (string file in files)
                { 
                    count++;
                    Console.WriteLine("Processing file" + "  " + count+ "  " + file);
                    Console.WriteLine("");
                }
   
        }
    #endif
  }
}
