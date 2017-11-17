using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Memory
{
    public class BootUp
    {
        public static int ServerPort = 11112;

        public static readonly string InputDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\IOFiles\");
        public readonly Dictionary<int, FreewaySensorInfo> freewayLinkLocDic;
        private static BootUp instance;

        public BootUp(string DBAddress)
        {
             //string cwd = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            try{
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"HighwayTraffic.xml");
                }
            catch(FileNotFoundException e){}
            try
            {
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"AverageSpeed.xml");
            }
            catch(FileNotFoundException e){}
            try
            {
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"PredictedSpeeds.xml");
            }
            catch (FileNotFoundException e) { }
            freewayLinkLocDic = new Dictionary<int, FreewaySensorInfo>();
            LoadArchivedLatLongs(DBAddress); 
            

            // uncomment this line to load latlongs from geodb
            //LoadLatLongs(DBAddress); 
        }

        #region Load Lat Long information
        private void LoadArchivedLatLongs(String FileAddress) //TODO: temporary function for the time Database is not available. You can load all objects in a better way later.
        {


            String path = Path.Combine(InputDataPath, @"SensorInfo\");
            BinaryFormatter bformatter = new BinaryFormatter();
            Stream stream = File.Open(path + @"keys.ser",
                                      FileMode.Open);
            List<Int32> sensorIDs = (List<Int32>)bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"lats.ser",
                                     FileMode.Open);
            List<Double> lats = (List<Double>)bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"longs.ser",
                                      FileMode.Open);
            List<Double> longs = (List<Double>)bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"dircs.ser",
                              FileMode.Open);
            List<Int32> dircs = (List<Int32>)bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"froms.ser",
                              FileMode.Open);
            List<String> fromStreets = (List<String>)bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"osts.ser",
                              FileMode.Open);
            List<String> osts = (List<String>)bformatter.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < sensorIDs.Count; i++)
                freewayLinkLocDic.Add(sensorIDs[i], new FreewaySensorInfo(lats[i], longs[i], osts[i], dircs[i], fromStreets[i]));
        }
        #endregion
         public FreewaySensorInfo GetFreewaySensInfo(int linkID)
        {
            if (freewayLinkLocDic.ContainsKey(linkID))
                return freewayLinkLocDic[linkID];
            Console.Error.WriteLine("link_id " + linkID + " does not exist in my latlong hash table");
            return null;
        }
        public static BootUp GetInstance()
        {

            if (instance == null)
                instance = new BootUp(string.Empty);
            return instance;
        }
        public static BootUp GetInstance(String DBAddress)
        {
            if (instance == null)
                instance = new BootUp(DBAddress);
            return instance;
        }
    }
}
