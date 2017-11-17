using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.ComplexEventProcessing;

namespace XMLOutputAdapter
{
    public class SocketPointArterialSensorSpeedOutput : XmlPointArterialSensorSpeedOutput
    {
        private readonly String ip;
        private readonly int port;

        public SocketPointArterialSensorSpeedOutput(SocketTrafficOutputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
            ip = configInfo.IPAddress;
            port = configInfo.PortNumber;
        }

        protected override void GenerateOutput()
        {
            var tcpclnt = new TcpClient();

            tcpclnt.Connect(ip, port);

            var writer = new StreamWriter(tcpclnt.GetStream());

            var asen = new ASCIIEncoding();
            //byte[] ba = asen.GetBytes(data);
            writer.Write(myCreateOutputMessage()); //(ba, 0, ba.Length);
            Console.WriteLine("Query 1 wrote to file " + Config.OutputFileName);
            // Question: Instead of writing "Query 1 wro...", how can I find the query/application that ran this adapter? 
            writer.Close();
        }
    }
}