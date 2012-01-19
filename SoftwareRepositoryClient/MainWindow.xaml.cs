////////////////////////////////////////////////////////////////////////////
// MainWindowxaml.cs - Starter Executive for Project #4                   //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This module defines the Operation of the WPF client UI availble to the USERS
 * All the Enent Handling of Client Action on the WCF services offered is handles through this Module.
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs,FileTransfer.cs
 * ManifestGenerator   --   Manifest.cs
 * SoftwareRepositoryServer --IRepositoryService.cs,RepositoryServer.svc.cs,User.dbml
 * VersionManager --Version.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using SoftwareRepositoryClient.SoftwareRepositoryService;
using Microsoft.Win32;
using System.Collections;
using ManifestGenerator;



namespace SoftwareRepositoryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoftwareRepositoryService.RepositoryServiceClient client;
        public string username { get; set; }
       
        public MainWindow()
        {
            InitializeComponent();
        
            this.ResizeMode = ResizeMode.NoResize;
        }
        
        //Populates the ListBox on the CLient UI with new Packages Information whenever Changes are Made.
        private void PopulateListBox()
        {
            try
            {
                client = new SoftwareRepositoryService.RepositoryServiceClient();

                SoftwareRepositoryService.PackageInfo[] p = client.GetPackages();
                foreach (PackageInfo pi in p)
                {
                    Dispatcher.Invoke(new Action<PackageInfo>(AddListBoxItems), new object[] { pi });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PopulateCacheListBox()
        {
            try
            {  
                CacheHandler ch =new CacheHandler();
                ch.setCache("../../CacheDir");
                string path=ch.getCache();
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles();

                foreach (FileInfo file in files)
                {       
                    Dispatcher.Invoke(new Action<FileInfo>(AddCacheItems), new object[] { file });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddListBoxItems(PackageInfo pi)
        {
            try
            {
                listBox1.Items.Add(pi.CreationDate + "\t" + "Version" + "\t" + pi.VersionValue + "\t" + pi.Status + "\t" + pi.fileName);
                listBox2.Items.Add(pi.CreationDate + "\t" + "Version" + "\t" + pi.VersionValue + "\t" + pi.Status + "\t" + pi.fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddCacheItems(FileInfo file)
        {
            try
            {
                listBox4.Items.Add(file.Name + "\t" + file.CreationTime);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UserLogin u = new UserLogin(this);
                u.Show();
              //  this.Visibility = Visibility.Hidden;
                Thread t = new Thread(new ThreadStart(PopulateListBox));
                t.Start();
                Thread t2 = new Thread(new ThreadStart(PopulateCacheListBox));
                t2.Start();
                listBox5.Visibility = System.Windows.Visibility.Hidden;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }


        private void Browsepackage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialogbox = new OpenFileDialog();
                if ((bool)(dialogbox.ShowDialog()))
                {
                    listBox2.IsEnabled = true;
                    listBox3.IsEnabled = true;
                    textBox1.Text = dialogbox.FileName;
                    Addselected.IsEnabled = true;
                    Removeselected.IsEnabled = true;
                    button2.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Event handling the Addition of dependencies to the Package being checked In.
        private void Addselected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IList selecteditems = listBox2.SelectedItems;
                IList itemsadded = listBox3.Items;
                List<string> temp = new List<string>();
                string[] s;
                string[] n;
                bool Flag = false;
                foreach (object i in selecteditems)
                {
                    Flag = false;
                    //foreach (object j in itemsadded)
                    foreach (object j in listBox3.Items)
                    {
                        s = i.ToString().Split('-');
                        n = s[s.Length - 2].Split('\t');
                        if (j.ToString().Contains(n[n.Length - 1]))
                        {
                            Flag = true;
                            break;
                        }

                    }
                    if (Flag == false)
                    {
                        listBox3.Items.Add(i.ToString());

                    }
                }
                foreach (string x in temp)
                    listBox3.Items.Add(x);
                selecteditems.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Event handling the Removal of dependencies to the Package being checked In.
        private void Removeselected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while (listBox3.SelectedItems.Count > 0)
                {
                    listBox3.Items.Remove(listBox3.SelectedItems[0]);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    string status;
                    Manifest m = new Manifest();
                    m.setManifestDir("../../Manifests");
                    if (checkBox1.IsChecked == true)
                        status = "OPEN";
                    else if (m.hadOpenDependency(listBox3.Items))
                    {
                        status = "PENDING";
                    }
                    else
                        status = "CLOSED";

                    FileInfo file = new FileInfo(textBox1.Text);
                    IList dependencies = listBox3.Items;

                    //Generate the Manifest File of the currently being checked in package
                    FileInfo xml = m.CreateManifest(file, dependencies, status, username);
                    if (file != null)
                    {
                        int ver = client.CheckInPackages(file);
                        int xmlver = ver;//client.CheckInPackages(xml); 
                        string filename = file.Name + "-" + ver.ToString();
                        string xmlname = xml.Name + "-" + xmlver.ToString();
                        //bool result= U.Upload(filename);
                        int tempver = xmlver - 1;
                        bool result = client.IsRI(xml.Name + "-" + tempver, username);
                        if (result)
                        {
                            FileTransfer f = new FileTransfer();
                            f.FileUpload(file, filename, client);
                            f.FileUpload(xml, xmlname, client);
                            listBox1.Items.Clear();
                            listBox2.Items.Clear();
                            listBox3.Items.Clear();
                            checkBox1.IsChecked = false;
                            PopulateListBox();
                            m.DeleteManifests();
                            
                            listBox3.IsEnabled = false;

                        }
                        else
                        {
                            MessageBox.Show("User Needs to Be RI to do a check In");

                        }
                        button2.IsEnabled = false;
                        textBox1.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(listBox1.SelectedIndex>=0)
                {
                    CacheHandler ch = new CacheHandler();
                    ch.setCache("../../CacheDir");
                    string packagedata=listBox1.SelectedItem.ToString();
                    string [] package=packagedata.Split('-');
                    string []fname=package[package.Length-2].Split('\t');
                    string filename = fname[fname.Length - 1];
                    string ver=package[package.Length-1];
                    string xmlname = filename + ".xml-" + ver;
                    string dpath;
                    filename += "-" + ver;
                    string[] result = client.CreateDownloadList(xmlname, filename);
                    List<string> duplicate = new List<string>();
                    duplicate.AddRange(result);
                    string[] res = ch.ScanCache(result);
                    if (res != null)
                    {
                        result = res;
                    }
                    FileTransfer ft = new FileTransfer();
                    System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
                    if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        dpath = fd.SelectedPath;
                    else
                        dpath = ".";
                    for (int i = 0; i < result.Length; i++)
                    {
                        ft.DownloadPackages(result[i], ch.getCache(), client);
                    }
                    ft.Cache_To_DlodXfer(duplicate, dpath);
                    listBox4.Items.Clear();
                    PopulateCacheListBox();
                    MessageBox.Show("Downloaded Files Successfully");
                    listBox1.SelectedIndex = -1;
                    DownloadResults(duplicate, res);
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        
         }

        private void DownloadResults(List<string> duplicate, string[] res)
        {
            List<string> cacheList = new List<string>();
            cacheList.AddRange(res);
            listBox5.Items.Clear();
            foreach (string d in duplicate)
            {
                listBox5.Visibility = System.Windows.Visibility.Visible;
                if (cacheList.Contains(d))
                {
                    listBox5.Items.Add(d + "\t" + "File Downloaded From Server");
                }
                else
                {
                    listBox5.Items.Add(d + "\t" + "File Extracted From Cache");
                }
            }
        }

        
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string packagedata = listBox2.SelectedItem.ToString();
                string[] package = packagedata.Split('-');
                string[] fname = package[package.Length - 2].Split('\t');
                string filename = fname[fname.Length - 1];
                string ver = package[package.Length - 1];
                string xmlname = filename + ".xml-" + ver;
                filename += "-" + ver;
                bool result = client.CanCancelOpenCheckIn(xmlname,username);

                if (result==true)
                {
                    client.CancelOpenCheckIn(filename);
                    client.CancelOpenCheckIn(xmlname);
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();
                    checkBox1.IsChecked = false;
                    PopulateListBox();
                    MessageBox.Show("Cancelled Open Check In");

                }
                else
                {
                    MessageBox.Show("Can't Process Open Check In Cancellation Request" +"\n" +"1) A Pending package depends on this Open Check in"
                        +"\n"+ "2)User Is not RI" + "\n"+ "3)You tried to Cancel a Closed Check In" + "");

                }                
            }   
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
      }
   }


