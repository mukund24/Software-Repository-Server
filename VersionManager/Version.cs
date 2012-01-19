////////////////////////////////////////////////////////////////////////////
// Version.cs - Provides The functionality tracking version numbers       //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This MOdule defines a class Named "Version".
 * This module performs the operations of getting version numbers of the files on the repository server
 * so as to know the version number of the next check In.
 * version numbers are decided on the basis of the STATUS of the existing check in.
 * If the STATUS is Open the Version number is not incremented and the current file on the repository 
 * is replaced by the new incoming file.
 * If the STATUS is Close the version number is incremented by 1 from the preivous value and sent as the new 
 * version number for the file being checked in.
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs
 * SoftwareRepositoryServer --IRepositoryService.cs,RepositoryServer.svc.cs,User.dbml
 * VersionManager   -- Version.cs
 *   
 *   
 * Maintenance History:
 * --------------------
 * Version 1.0 Release 11/14/2011.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace VersionManager
{
    public class Version
    {
        string TestDir;
        public void setTestDir(string path)
        {
            TestDir = path;
        }

        public string getTestDir()
        {
            return TestDir;
        }

        //Get the Latest value of the package number for the new version to be checked in.
        public int GetPackageVersion(string path,FileInfo filename)
        {
            int tempver,latestversion=0,ver=1;
            string fname="";
            DirectoryInfo di=new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();
           foreach (FileInfo file in files)
           { 
            string [] s=file.Name.Split('-');
            //string tmp = s[0] + "."+ s[1];
           
               for (int i = 0; i < s.Length - 1; i++)
                {
                    fname += s[i];
                }  
 
                if (filename.Name ==fname)
                {
                    tempver = Convert.ToInt32(s[s.Length-1]);
                    if (tempver > latestversion)
                        latestversion = tempver;
                }
                fname = "";    
           }
           bool status = ischeckinClosed(path, filename.Name +".xml-"+ latestversion);
           if (status)
               ver = latestversion + 1;
           else
               ver = latestversion;
               return ver;
        }
        //Check whether the CheckIn is Closed
        private bool ischeckinClosed(string path, string filename)
        {
            bool flag = true;
            if (File.Exists(path + "\\" + filename))
            {
                XDocument xdoc = XDocument.Load(path + "\\" + filename);
                var q = from x in xdoc.Descendants()
                        where (x.Name == "STATUS")
                        select x;
                foreach (var elem in q)
                {
                    if (elem.Value == "CLOSED")
                        flag = true;
                    else
                        flag = false;
                }
            }
            return flag;
        }
#if (TEST_VERSION)
    static void Main(string[] args)
    {
        Console.WriteLine("================================");
        Console.WriteLine("    Testing VERSION MANAGER ");
        Console.WriteLine("================================\n");
        DirectoryInfo di;
        Version v = new Version();
        v.setTestDir("../../Test");
        if (!Directory.Exists(v.getTestDir()))
            di = Directory.CreateDirectory(v.getTestDir());
        else
            di = new DirectoryInfo(v.getTestDir());

        FileInfo f = new FileInfo("../../Version.cs");
        Console.WriteLine();
        FileInfo[] files = di.GetFiles();
        foreach (FileInfo file in files)
        {
            Console.WriteLine("Processing File :{0} for version Number", file.Name);
            int ver=v.GetPackageVersion(v.getTestDir(), file);
            Console.WriteLine();
            Console.WriteLine("File{0} is Version {1}", file.Name, ver);
            Console.WriteLine("---------------------------------------------");
        }
        
    }

#endif
    
    }
}
