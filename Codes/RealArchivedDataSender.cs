/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Utils;

namespace TrafficApp
{
    public class RealArchivedDataSender
    {
        public RealArchivedDataSender()
        {
            var sender = new Thread(send);
            sender.Start();
        }

        private static void send()
        {
            // send 2 hours of real traffic data for 08/30/2010
            for (int i = 200; i < 379; i++)
            {
                var tcpclnt = new TcpClient();

                tcpclnt.Connect("127.0.0.1", 11111);
                TextReader reader = new StreamReader(Constants.RealTrafficDataPath + i + ".xml");
                String data = reader.ReadToEnd();

                var writer = new StreamWriter(tcpclnt.GetStream());

                //ASCIIEncoding asen = new ASCIIEncoding();
                //byte[] ba = asen.GetBytes(data);
                writer.Write(data); //(ba, 0, ba.Length);
                Console.WriteLine("Real Archived Traffic Data: Data Sent");
                Thread.Sleep(1000*15);
                //System.Threading.Thread.SpinWait(1000*10);
                writer.Close();
            }
        }
    }
}