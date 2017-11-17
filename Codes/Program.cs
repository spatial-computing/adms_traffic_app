/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the start Query for Bus, Rail, RMS, TravelTime in Send function
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Start storing Arterial, Freeway, Bus, Rail, RMS, TravelTime in Azure Table.
 * Date: 06/08/2011
 */

using System;
using System.IO;
using System.Threading;
using SIServers;
using UDOs;

namespace TrafficApp
{
    public class Program
    {
        //private static readonly string solutionPath =
        //   @"C:\Users\Jalal\Documents\My Dropbox\C# projects\Colin Solution\";
        public static readonly string InputDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\IOFiles\");

        //public static string outputPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string outputPath =
            //@"C:\Users\StreamInsight\Desktop\mta_svn\WebApplication1\data\xml\";
            @"C:\inetpub\wwwroot\WebApplication1\data\xml\";
            //@"C:\Users\StreamInsight\Desktop\mta_svn\jalalXML\";
            //@"C:\inetpub\wwwroot\ADMS\WebApplication1\data\xml\";
        //solutionPath + @"WebApplication1\"; //String.Empty;

        private static ExplicitServer currentServer;
        private static void send()
        {
            String DBAddress = String.Empty;
            currentServer = new ExplicitServer(DBAddress, Program.outputPath);
            //"C:/Users/Jalal/Documents/My Dropbox/C# projects/Final Solution/WebApplication1/");// String.Empty);
            
            currentServer.StartFreewayQuery();
            currentServer.StartArterialQuery();
            currentServer.StartBusQuery();
            currentServer.StartRailQuery();
            currentServer.StartRampQuery();
            currentServer.StartTravelQuery();
            currentServer.StartEventQuery();
            currentServer.StartCmsQuery();
            
            //currentServer.StartAzureStorageFreeway();
            //currentServer.StartAzureStorageArterial();
            //currentServer.StartAzureStorageBusGPS();
            //currentServer.StartAzureStorageRailGPS();
            //currentServer.StartAzureStorageRamp();
            //currentServer.StartAzureStorageTravelTime();
            //currentServer.StartAzureStorageEventData();
            //currentServer.StartAzureStorageCMS();

          //  currentServer.StartXMLHighwayQuery();
          //  currentServer.StartXMLArterialQuery();


        }

        public static void Main(string[] args)
        {
            var serverThread = new Thread(send);
            serverThread.Start();
        }

        
    }
}