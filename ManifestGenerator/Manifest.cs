////////////////////////////////////////////////////////////////////////////
// Mnifest.cs - Creation of Manifest Files of packages being checked In   //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This Module Defines a class Named Manifest which handle the operations of creating Manifest Files of the 
 * packages being checked In,check the type of dependencies.
 * 
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs
 * ManifestGenerator   --   Manifest.cs
 * SoftwareRepositoryServer --IRepositoryService.cs,RepositoryServer.svc.cs,User.dbml
 * 
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
using System.Collections;
using System.Xml;

namespace ManifestGenerator
{
    public class Manifest
    {
        string ManDir;
        string TestFile;
        public void setManifestDir(string path)
        {
            ManDir = path;
        }

        public string getManDir()
        {
            return ManDir;
        }

        public void setTestFile(string path)
        {
            TestFile = path;
        }

        public string getTestFile()
        {
            return TestFile;
        }  
        public FileInfo CreateManifest(FileInfo f, IList dependencies,string status,string username)
        {
            string fname = "";
            XmlTextWriter tw = new XmlTextWriter(getManDir()+"/" + f.Name + ".xml", Encoding.Default);
            tw.Formatting = Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("MANIFEST");
            tw.WriteStartElement("RI");
            tw.WriteString(username);
            tw.WriteEndElement();
            tw.WriteStartElement("PACKAGE");
            tw.WriteString(f.Name);
            tw.WriteEndElement();
            tw.WriteStartElement("STATUS");
            tw.WriteString(status);
            tw.WriteEndElement();
           
            tw.WriteStartElement("DEPENDENCIES");
            foreach (object o in dependencies)
            {
                string [] s=o.ToString().Split('\t');
                string[] x = s[s.Length-1].Split('-');
                string ver = x[x.Length-1];
                for (int i = 0; i < x.Length - 1; i++)
                {
                    fname =fname+ x[i];

                }  
                tw.WriteStartElement("DEPENDENCY");
                tw.WriteString(fname+ ".xml" +"-" +ver);
                tw.WriteEndElement();
                fname = "";
            }
            
            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Flush();
            tw.Close();
            FileInfo xml = new FileInfo(getManDir()+"/" + f.Name + ".xml");
            return xml;
     }
        //Checks whether dependency Status is OPEN
        public bool hadOpenDependency(IList dependencies)
        {
            foreach (object o in dependencies)
            {
                if (o.ToString().Contains("\tOPEN\t"))
                    return true;
            }
            return false;
        }

        //Deletes the manifest files generated on the client side once check in is Successful.
        public void DeleteManifests()
        { 
            DirectoryInfo di=new DirectoryInfo(getManDir());
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }
        }
    
#if (TEST_MANIFEST)
        static void Main(string[] args)
        {
            Console.WriteLine("==========================");
            Console.WriteLine("Testing Manifest Generator");
            Console.WriteLine("==========================");
            Console.WriteLine("========================================================================");
            Console.WriteLine("Generating XML manifest for MANIFEST.cs at Location ../../Manifest/");
            Console.WriteLine("==========================================================================");
            Manifest man = new Manifest();
            List<string> i=new List<string>();
            DirectoryInfo di;
            man.setTestFile("./Test.cs-1");
            FileInfo test = new FileInfo(man.getTestFile());
            i.Add(test.Name);
            man.setManifestDir("../../Manifests");
            
            if (!Directory.Exists(man.getManDir()))
                di = Directory.CreateDirectory(man.getManDir());
            else
                di = new DirectoryInfo(man.getManDir());
            
            FileInfo f=new FileInfo("../../Manifest.cs");
            man.CreateManifest(f, i, "CLOSED", "mukund");


            bool result=man.hadOpenDependency(i);
            if (result == false)
            {
                Console.WriteLine("MANIFEST.cs has No OPEN DEPENDENCIES ");
            }

            string option="";
            Console.WriteLine("DELETE MANIFESTS (Y/N)?");
            option=Console.ReadLine();
            if (option == "Y")
            {
                man.DeleteManifests();
                Console.WriteLine("Manifest file Deleted");
            }

        }

#endif

    }
}
