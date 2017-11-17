using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.ComponentModel;
using System.IO;

namespace ExceptionReporter
{
    public class ExceptionEmailReporter
    {
        bool mailSent = false;
        public string msg = "";
        public int lineNo = -1;
        public string FileName = "";
        public ExceptionEmailReporter(string m, int l, string fileName)
        {
            msg = m;
            lineNo = l;
            FileName = fileName;
        }

        public void SendEmailThread()
        {
            try
            {
                var reportThread = new Thread(SendEmail);
                Console.WriteLine(msg);
                reportThread.Start();
            }
            catch (Exception e)
            {

                try
                {
                    StreamWriter tempWriter = new StreamWriter("Network Failure_Email", true);
                    tempWriter.WriteLine(DateTime.Now.ToString() + ": " + e.Message);
                    tempWriter.Close();
                    tempWriter.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendEmail()
        {

            try
            {
            
                // Command line argument must the the SMTP host.
                var client = new SmtpClient("smtp.gmail.com", 587)
                                 {
                                     Credentials = new NetworkCredential("StreamInsight323@gmail.com", "infolab323"),
                                     EnableSsl = true
                                 };

                MailAddress from = new MailAddress("StreamInsight323@gmail.com", "StreamInsight Infolab",
                                                   System.Text.Encoding.UTF8);

                // Set destinations for the e-mail message.
                MailAddress to = new MailAddress("demiryur@usc.edu");
                // Specify the message content.


                MailMessage message = new MailMessage(from, to);
                //message.To.Add(new MailAddress("aakdogan@usc.edu"));
                //message.To.Add(new MailAddress("demiryur@usc.edu"));
                message.To.Add(new MailAddress("madapura@usc.edu"));
                message.To.Add(new MailAddress("jianfali@usc.edu"));
                message.Body =
                    "Hi, Infolabers: \n This is an automatic msg to notify you that the StreamInsight server has catched the following exception:";

                message.Body += Environment.NewLine + "\nException Msg:" + msg;
                message.Body += Environment.NewLine + "\nSource Code File:" + FileName;
                message.Body += Environment.NewLine + "\n Line Number:" + lineNo.ToString();
                // Include some non-ASCII characters in body and subject.
                string footnote = "\nBest,\n StreamInsight Server";

                message.Body += Environment.NewLine + footnote;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = "StreamInsight Server Exception Notification for ADMS (From New Server)";
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                // Set the method that is called back when the send operation ends.
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                // The userState can be any object that allows your callback 
                // method to identify this send operation.

                // For this example, the userToken is a string constant.
                string userState = "test message1";
                client.SendAsync(message, userState);

                Console.WriteLine("Sending Exception message... ");
                string answer = Console.ReadLine();

                while (!mailSent)
                {
                    Thread.Sleep(1000);
                }

                // If the user canceled the send, and mail hasn't been sent yet,
                // then cancel the pending operation.
                //if (answer.StartsWith("c") && mailSent == false)
                //{
                //    client.SendAsyncCancel();
                //}
                // Clean up.
                message.Dispose();
            }
            catch(Exception)
            {

            }
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }
    }
}
