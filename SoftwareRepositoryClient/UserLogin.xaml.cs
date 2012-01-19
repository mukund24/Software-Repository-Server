////////////////////////////////////////////////////////////////////////////
// UserLoginxaml.cs - Login WPF UI for the Client                         //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This Module provides the Login Fucnitonality for the Users of the SOftware Repository Server
 */
/* Required Files:
 * ===============
 * -----------------------------
 * Module            File Names
 * -----------------------------       
 * SoftwareRepositoryClient   --   MainWindowxaml.cs,FileTransfer.cs,userLoginxaml.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SoftwareRepositoryClient.SoftwareRepositoryService;

namespace SoftwareRepositoryClient
{
    /// <summary>
    /// Interaction logic for UserLogin.xaml
    /// </summary>
    public partial class UserLogin : Window
    {
        SoftwareRepositoryService.RepositoryServiceClient client;
        MainWindow m;
        bool status;
        public UserLogin(MainWindow parent)
        {
            m = parent;
            InitializeComponent();
            status = true;
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            client = new RepositoryServiceClient();
            string username = textBox1.Text;
            string password = passwordBox1.Password;

            status=client.AuthenticateUser(username, password);
            if (status == true)
            {
                this.Close();
                m.username = username;
                m.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("Invalid Credentials");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!status)
                m.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Close();
            m.username = "mukund";
            m.Visibility = Visibility.Visible;
            //status = true;
        }
    }
}
