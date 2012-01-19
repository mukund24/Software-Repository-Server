////////////////////////////////////////////////////////////////////////////
//Repositoryserver.svc.cs - Implements all the Server Side Funtionality    //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 *This module defines the "Class RepositoryServer"  which is inherited from the Service Contract "Interface IRepositoryServer"
 *All the Service that are provided to the client are implemented in this module.
 *
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs,FileTransfer.cs
 * SoftwareRepositoryServer --IRepositoryService.cs,RepositoryServer.svc.cs,User.dbml
 * VersionManager --Version.cs
 * ManifestGenerator   --   Manifest.cs
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.IO;
using VersionManager;
using System.Xml.Linq;
using System.ServiceModel.Activation;
using System.Web;


namespace SoftwareRepositoryServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RepositoryService : IRepositoryService
    {
        private FileStream downloadReader;
   
        public List<PackageInfo> GetPackages()
        {
            try
            {
                List<PackageInfo> packagelist = new List<PackageInfo>();
                string path = HostingEnvironment.MapPath("~/Repository Server");
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles();
                string fname = "";
                VersionManager.Version v = new VersionManager.Version();
                foreach (FileInfo file in files)
                {
                    if (!file.Name.Contains(".xml-"))
                    {
                        //XDocument xdoc = new XDocument(file.Name + ".xml");
                        PackageInfo p = new PackageInfo();
                        string[] s = file.Name.Split('-');
                        for (int i = 0; i < s.Length - 1; i++)
                        {
                            fname += s[i];
                        }
                        p.fileName = file.Name;
                        p.VersionValue = Convert.ToInt32(s[s.Length - 1]);
                        p.CreationDate = file.CreationTime.ToString();
                        p.Status = Status(path, fname + ".xml-" + p.VersionValue);
                        packagelist.Add(p);
                        fname = "";
                    }
                }
                return packagelist;
            }
            catch
            {
                return null;
            
            }
        
        }



        private string Status(string path, string xname)
        {
            try
            {
                if (File.Exists(path + "\\" + xname))
                {
                    XDocument xdoc = XDocument.Load(path + "\\" + xname);
                    var q = (from x in xdoc.Descendants()
                             where (x.Name == "STATUS")
                             select x).Single();
                    return q.Value;

                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public int CheckInPackages(FileInfo filename)
        {
            try
            {
                VersionManager.Version v = new VersionManager.Version();
                string path = HostingEnvironment.MapPath("~/Repository Server");

                int ver = v.GetPackageVersion(path, filename);
                return ver;
            }
            catch
            {
                return 1;
            }
        
        }


        public bool UploadPackages(string file,byte [] data,bool firstime)
        {
            try
            {
                FileStream fs;
                string path = HostingEnvironment.MapPath("~/Repository Server");
                if (firstime)
                {
                     fs = new FileStream(path + "\\" + file, FileMode.Create, FileAccess.Write);
                }
                else
                    fs = new FileStream(path + "\\" + file, FileMode.Append, FileAccess.Write);
                fs.Write(data, 0, data.Length);
                fs.Close();
                return true;
            }
            catch  
            {
                return false;
              
            }

        }


        public bool AuthenticateUser(string Username, string Password)
        {
            UsersDataContext u = new UsersDataContext();
            var query = from x in u.RepUsers
                        where x.Username == Username && x.Password == Password
                        select x;
            if (query.Count() > 0)
                return true;
            else
                return false;

        }


        public bool IsRI(string xmlname, string username)
        {
            string path = HostingEnvironment.MapPath("~/Repository Server");
            bool flag = true;
            
            if (File.Exists(path + "\\" + xmlname))     
                { 
                    XDocument xdoc = XDocument.Load(path + "\\" + xmlname);
                    var q = (from x in xdoc.Descendants()
                            where (x.Name == "RI")
                            select x).Single();
                        if (q.Value == username)
                            flag= true;
                        else
                            flag= false;                     
                
                }
            return flag;

        }


        public List<string> CreateDownloadList(string xmlname,string filename)
        {
            try
            {
                string path = HostingEnvironment.MapPath("~/Repository Server");
                Queue<string> dwnlist = new Queue<string>();
                List<string> result = new List<string>();
                List<string> visited = new List<string>();
                if (File.Exists(path + "\\" + xmlname))
                {
                    dwnlist.Enqueue(xmlname);
                    while (dwnlist.Count > 0)
                    {
                        string elt = dwnlist.Dequeue();
                        string[] s2 = elt.Split('-');
                        string ver2 = s2[s2.Length - 1]; 
                        XDocument doc = XDocument.Load(path + "\\" + elt);
                        var query = from x in doc.Elements("MANIFEST").
                                    Elements("DEPENDENCIES").
                                    Elements()
                                    select x;
                        
                        var query2 = (from y in doc.Descendants()
                            where (y.Name == "PACKAGE")
                            select y).Single();
                        string filename2=query2.Value + "-" +ver2;
                        if (!result.Contains(filename2))
                        {
                            result.Add(filename2);
                            foreach (var elem in query)
                            {
                                dwnlist.Enqueue(elem.Value);
                            }
                        }

                    }
                }
                return result;
            }
            catch
             {
                    return null;
             }
     }

        public bool CanCancelOpenCheckIn(string filename,string Username)
        {
            try
            {
                bool flag = false;
                string path = HostingEnvironment.MapPath("~/Repository Server");
                if (Status(path, filename) == "OPEN" &&IsRI(filename,Username))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    FileInfo[] xfiles = di.GetFiles();
                    flag = ScanManifest(filename, flag, path, xfiles);
                    return flag;
                }
                else
                {
                    flag = false;
                    return flag;
                }

            }
            catch
            {
                return false;
            }
        }

        private static bool ScanManifest(string filename, bool flag, string path, FileInfo[] xfiles)
        {
            foreach (FileInfo file in xfiles)
            {
                flag = true;
                if (file.Name.Contains(".xml-"))
                {
                    XDocument xdoc = XDocument.Load(path + "\\" + file);
                    var query = (from x in xdoc.Descendants()
                                 where (x.Name == "STATUS")
                                 select x).Single();
                    var q = from y in xdoc.Elements("MANIFEST").
                                    Elements("DEPENDENCIES").
                                    Elements()
                            select y;
                    if (query.Value == "PENDING")
                    {
                        foreach (var b in q)
                        {
                            if (b.Value == filename)
                            {
                                flag = false;
                                break;
                            }
                            else
                                flag = true;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (!flag)
                        break;
                }

            }
            return flag;
        }

        
        public bool CancelOpenCheckIn(string filename)
        {
            try
            {
                bool flag = false;
                string path = HostingEnvironment.MapPath("~/Repository Server");
                if (File.Exists(path + "\\" + filename))
                {
                    File.Delete(path + "\\" + filename);
                    flag = true;
                }
                else
                      flag = false;
                return flag;        
            }
  
           catch
            { 
            return false;
            }
        
        }


        public void InitiateDownload(string filename)
        {
            try
            {
                string path = HostingEnvironment.MapPath("~/Repository Server") + "\\" + filename;
                downloadReader = new FileStream(path, FileMode.Open, FileAccess.Read);
                HttpContext.Current.Session["download"] = downloadReader;
            }

            catch
            {
                return;
            }
       }

        public byte[] DownloadPackage()
        {
            try
            {
                int bytesRead;
                downloadReader = (FileStream)HttpContext.Current.Session["download"];
                while (true)
                {
                    long BlockSize = 512;
                    long Remainder = (int)(downloadReader.Length - downloadReader.Position);
                    if (Remainder == 0)
                        return null;
                    long size = Math.Min(BlockSize, Remainder);
                    byte[] block = new byte[size];
                    bytesRead = downloadReader.Read(block, 0, block.Length);
                    return block;
                }
            }
            catch
            {
                return null;
            }
        }

        public void CloseDownload()
        {
            try
            {
                downloadReader = (FileStream)HttpContext.Current.Session["download"];
                downloadReader.Close();
            }
            catch
            {
                return;
            }
        }
    }
}
