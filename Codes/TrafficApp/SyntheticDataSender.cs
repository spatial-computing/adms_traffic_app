/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using EventTypes;

namespace TrafficApp
{
    public class SyntheticDataSender
    {
        private static readonly Random rnd = new Random();
        private static List<TrafficSensorReading> allEvents;

        public SyntheticDataSender()
        {
            var sender = new Thread(send);
            sender.Start();
        }

        private static void send()
        {
            allEvents = ParseXML(Program.InputDataPath + @"oneMinuteOfTrafficData.xml");
            //Thread.Sleep(1000 * 3);


            while (true)
            {
                var tcpclnt = new TcpClient();

                tcpclnt.Connect("127.0.0.1", 11111);

                String data = "Ready," + rnd.Next(1, 9) + "," + createData();
                var writer = new StreamWriter(tcpclnt.GetStream());

                var asen = new ASCIIEncoding();
                //byte[] ba = asen.GetBytes(data);
                writer.Write(data); //(ba, 0, ba.Length);
                Console.WriteLine("SyntheticDataSender: Data Sent");
                Thread.Sleep(1000*30);
                //System.Threading.Thread.SpinWait(1000*10);
                writer.Close();
            }
        }

        private static string createData()
        {
            String result =
                @"<?xml version=""1.0"" encoding=""UTF-8""?><informationResponse xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""RIITS_ISP_Schema.xsd""><messageHeader><sender><agencyName>Caltrans-D7</agencyName></sender><messageID>1264914916452</messageID><responseTo>87654321</responseTo><timeStamp><date>";
            DateTime now = DateTime.Now;
            result += now.ToString("yyyyMMdd");
            result += "</date><time>";
            result += now.ToString("HHmmss");
            result += "00";
            result += "</time></timeStamp></messageHeader><responseGroups><responseGroup><head><updateTime><date>";
            now -= TimeSpan.FromMinutes(1);
            result += now.ToString("yyyyMMdd");
            result += "</date><time>";
            result += now.ToString("HHmmss");
            result += "00";
            result += "</time></updateTime></head><links>";

            foreach (TrafficSensorReading sensor in allEvents)
            {
                result += "<link><head><id>";
                result += sensor.SensorId;
                result += "</id></head><occupancy>";
                result += sensor.Occupancy;
                result += "</occupancy><speed>";
                result += rnd.Next(Math.Max(sensor.Speed - 10, 0), sensor.Speed + 10);
                result += "</speed><localLinkTrafficInformation><volume>";
                result += sensor.Volume;
                result += "</volume><hovSpeed>";
                result += sensor.HovSpeed;
                result += "</hovSpeed><linkDataStatus>";
                result += sensor.LinkDataStatus;
                result += "</linkDataStatus></localLinkTrafficInformation></link>";
            }
            result += "</links>";
            return result;
        }

        private static List<TrafficSensorReading> ParseXML(String fileName)
        {
            int eventIndex = 0;
            var allEvents = new List<TrafficSensorReading>();
            var xDoc = new XmlDocument();
            xDoc.Load(fileName);
            XmlNodeList name = xDoc.GetElementsByTagName("id");
            XmlNodeList occupancy = xDoc.GetElementsByTagName("occupancy");
            XmlNodeList speed = xDoc.GetElementsByTagName("speed");
            XmlNodeList volume = xDoc.GetElementsByTagName("volume");
            XmlNodeList hovSpeed = xDoc.GetElementsByTagName("hovSpeed");
            XmlNodeList linkDataStatus = xDoc.GetElementsByTagName("linkDataStatus");
            Console.WriteLine("started loading");
            for (int i = 0; i < name.Count; i++)
            {
                string linkNumber = name[i].InnerText;
                int occ = Int32.Parse(occupancy[i].InnerText);
                String status = linkDataStatus[i].InnerText;
                int sp = Int32.Parse(speed[i].InnerText);
                int hov = Int32.Parse(hovSpeed[i].InnerText);
                int vol = Int32.Parse(volume[i].InnerText);

                allEvents.Add(new TrafficSensorReading(hov, status, occ, linkNumber, sp, vol));
            }
            Console.WriteLine("finished loading");
            return allEvents;
        }
    }
}