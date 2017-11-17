using System;
using System.IO;
using System.Threading;
using System.Web.UI;
using Ajax;
using SIServers;
using TrafficApp;
using UDOs;

public partial class BingMap : Page
{
    private static ExplicitServer currentServer;
    private static bool collectingStats;

    [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
    public void SetK(int k)
    {
        currentServer.SetK(k);
    }

    [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
    public void SetWeight(double weight)
    {
        WeightedAverage.HistoricWeight = weight;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.RegisterTypeForAjax(typeof (BingMap));
        collectingStats = false;
        //Program.Main(null);
        //currentServer = Program.Server;


        //var serverThread = new Thread(send);
        //serverThread.Start();
        //Thread.Sleep(20 * 1000); //TODO: trigger an event that lets the synthetic data sender start working. OR find another solution //uncomment for demos.
        //SyntheticDataSender testData = new SyntheticDataSender(); // Sends Data each minute.  */ used a long time ago.
        //   RealArchivedDataSender realArchivedData = new RealArchivedDataSender(); // used for vldb demo 2010.
    }

   

    [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
    public void StopAll()
    {
        try
        {
            if (collectingStats)
            {
                currentServer.StopAverageQuery();
                collectingStats = false;
            }
            //System.IO.File.Delete("C:/Users/Jalal/Documents/My Dropbox/C# projects/Final Solution/WebApplication1/AverageSpeed.xml");
            File.Delete(Program.outputPath + @"AverageSpeed.xml");
        }
        catch
        {
        }
        try
        {
            File.Delete(Program.outputPath + @"PredictedSpeeds.xml");
        }
        catch
        {
        }
    }

    [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
    public void CollectStatistics(String lat1, String lng1, String lat2, String lng2)
    {
        try
        {
            // currentServer.GetWaitHandle().Set();
            //UDOs.ListOfSensors.SetList(Utils.Utilities.GetInstance().GetLinksInBetween(
            //    Double.Parse(lat1),Double.Parse(lng1),Double.Parse(lat2),Double.Parse(lng2)).ToArray());
            InsideFreewayRectangle.Rect = new Rectangle(Double.Parse(lat1), Double.Parse(lng1), Double.Parse(lat2),
                                                 Double.Parse(lng2));
            //TODO: synchronization of rect.
            if (!collectingStats)
            {
                currentServer.StartAverageQuery();
                collectingStats = true;
            }
            // currentServer = SIServer.GetInstance(Double.Parse(lat1), Double.Parse(lng1), Double.Parse(lat2), Double.Parse(lng2));
            //       currentServer.Implicit(Double.Parse(lat1), Double.Parse(lng1), Double.Parse(lat2), Double.Parse(lng2));

            //return lat1 + " " + lng1 + " " + lat2 + " " + lng2;
            //return "Streaminsight has started the average query";
        }
        catch
        {
        }
    }
}