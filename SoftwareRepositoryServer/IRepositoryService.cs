////////////////////////////////////////////////////////////////////////////
//IRepositoryserver.cs - Defines the Service COntract Interface            //
//Author:Mukund Narayana Murthy SUID:50361-4612                           //
//  CSE681 - Software Modeling and Analysis, Fall 2011                    //
///////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 *This module defines the "Interface IRepositoryServer"  which defines the list of Opertion Contracts
 *Services to be provided by the SOftware repository Server to the client.
 *
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace SoftwareRepositoryServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRepositoryService
    {
        //Get the list of packages currently on the repository Server to be displayed on the Client UI
        [OperationContract]
        List<PackageInfo> GetPackages();
        //Get the latest numberical version number for the package before it is check in to the repository
        [OperationContract]
        int CheckInPackages(FileInfo filename);
        //Upload the latest version of the package into the repositry server
        [OperationContract]
        bool UploadPackages(string file, byte[] data,bool firstime);

        //Authenticate Users of the Repository Server System.
        [OperationContract]
        bool AuthenticateUser(string Username, string Password);

        //Check Whether the User Looged in is RI of the Package.
        [OperationContract]
        bool IsRI(string filename,string username);

        //Create a list of pacakges to be downloaded by tracking direct and indirect dependencies
        [OperationContract]
        List<string> CreateDownloadList(string pname,string fname);

        //Check whether an OPen Checkin can be cancelled
        [OperationContract]
        bool CanCancelOpenCheckIn(string filename,string username);

        //Cancelling an open check in
        [OperationContract]
        bool CancelOpenCheckIn(string filename);

        //Initiates the download/extraction of packages
        [OperationContract]
        void InitiateDownload(string filename);


        [OperationContract]
        byte[] DownloadPackage();

        [OperationContract]
        void CloseDownload();

    }
    //Data contract which defines the Package information to be rendered on the WPF client UI based on the information 
    // of the packages currently on the repository server.
    [DataContract]
    public class PackageInfo
    {
        private string file;
        private int VersionNum;
        private string CreationDateValue,StatusValue;

        [DataMember]
        public string fileName
        {
            get { return file; }
            set { file = value; }
        }

        [DataMember]
        public int VersionValue
        {
            get { return VersionNum; }
            set { VersionNum = value; }
        }

        [DataMember]
        public string CreationDate 
        {
            get { return CreationDateValue; }
            set { CreationDateValue = value; }
        }

        [DataMember]
        public string Status
        {
            get { return StatusValue; }
            set { StatusValue = value; }
        }

    }

}
