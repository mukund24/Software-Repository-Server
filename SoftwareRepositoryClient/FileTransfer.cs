////////////////////////////////////////////////////////////////////////////
// FileTransfer.cs - Provides The functionality of upload/Download        //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This module performs he File transfer Operations involved in the Checkin and Extraction process
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs,FileTransfer.cs
 * SoftwareRepositoryServer --IRepositoryService.cs,RepositoryServer.svc.cs,User.dbml
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
using SoftwareRepositoryClient.SoftwareRepositoryService;
namespace SoftwareRepositoryClient
{
    class FileTransfer
    {
        public void FileUpload(FileInfo file, string filename,SoftwareRepositoryService.RepositoryServiceClient client)
        {

            FileStream fs = null;
            try
            {
                bool firsttime = true;
                fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
                int bytesRead = 0;
                //int count = 0;
                while (true)
                {
                    long BlockSize = 5000;
                    long Remainder = (int)(fs.Length - fs.Position);
                    if (Remainder == 0)
                        break;
                    long size = Math.Min(BlockSize, Remainder);
                    byte[] block = new byte[size];
                    bytesRead = fs.Read(block, 0, block.Length);
                    client.UploadPackages(filename, block, firsttime);
                    firsttime = false;
                }
                fs.Close();
                return;
            }
            catch
            {
                if (fs != null)
                    fs.Close();
                
                return;
            }
        }

        public void DownloadPackages(string fname,string dpath,SoftwareRepositoryService.RepositoryServiceClient client)
        {
            try
            {
                //FileStream fs = new FileStream(dpath + "\\" + fname.Split('-')[0], FileMode.Create, FileAccess.Write);
               FileStream fs = new FileStream(dpath + "\\" + fname, FileMode.Create, FileAccess.Write);
                client.InitiateDownload(fname);
                byte[] block;
                while (true)
                {
                    block = client.DownloadPackage();
                    if (block == null)
                        break;
                    fs.Write(block, 0, block.Length);
                }
                client.CloseDownload();
                fs.Close();

            }
            catch
            {
                return;
            }

        }

        public void Cache_To_DlodXfer(List<string> cachestuff,string path)
        {
            CacheHandler ch =new CacheHandler();
            ch.setCache("../../CacheDir");
            //DirectoryInfo di = new DirectoryInfo(path);
            foreach (string file in cachestuff)
            {
                FileInfo f = new FileInfo(ch.getCache() +"\\"+file);
                f.CopyTo(path +"\\" + f.Name.Split('-')[0], true);
            }

        
        }

//#if (TEST_FILETRANSFER)
//        static void Main(string[] args)
//        {
//            Console.WriteLine("=============================");
//            Console.WriteLine("Testing File Transfer Service");
//            Console.WriteLine("=============================");
//            Console.WriteLine("========================================================================");
//            Console.WriteLine("Generating XML manifest for MANIFEST.cs at Location ../../Manifest/");
//            Console.WriteLine("==========================================================================");
//            FileTransfer ft = new FileTransfer();
//            FileInfo f = new FileInfo("../../FileTransfer.cs");
//            string filename = f.Name + "-" + 1;
//            SoftwareRepositoryService.RepositoryServiceClient client = new RepositoryServiceClient(); ;
//            ft.FileUpload(f, filename, client);

//        }

//#endif

    }
}
