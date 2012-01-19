using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SoftwareRepositoryClient
{
    class CacheHandler
    {
        string Downloadscache;
        public void setCache(string cachedir)
        {
            Downloadscache = cachedir;
        }

        public string getCache()
        {
            return Downloadscache;
        }

        public string [] ScanCache(string[] result)
        {
            List<string> resultlist = new List<string>();
            DirectoryInfo di=new DirectoryInfo(getCache());
            if (!Directory.Exists(getCache()))
                di = Directory.CreateDirectory(getCache());
            else
                di = new DirectoryInfo(getCache());
            
            FileInfo[] files = di.GetFiles();
             for(int i=0;i<result.Length;i++)
            {
                resultlist.Add(result[i]);
            }
             foreach (FileInfo file in files)
                {
                    if (resultlist.Contains(file.Name))
                    {
                        resultlist.Remove(file.Name);
                    }
                }
            return resultlist.ToArray();
        }
    }
}
