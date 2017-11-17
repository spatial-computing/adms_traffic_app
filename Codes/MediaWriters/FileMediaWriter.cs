/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.IO;
using ExceptionReporter;

namespace MediaWriters
{
    public class FileMediaWriter : MediaWriter
    {
        private bool backUpEnabled = false;
        private string filePath;
        private string myBackUpPath; //maybe deleted. just for demo purposes
        public FileMediaWriter(string filePath)
        {
            this.filePath = filePath; 
            
            //for replay purposes. not a good programming way.
            if (filePath.Contains("Arterial"))
                myBackUpPath= @"C:\OneWeekForBarak\ArterialLADOT\"; //@"C:\ProducedForColin\ArterialLADOT\";
            else
            {
                if (filePath.Contains("Highway"))
                    myBackUpPath = @"C:\OneWeekForBarak\FreewayD7\";//@"C:\ProducedForColin\FreewayD7\";
                                      
            }
            
        }

        private int tempReplayCounter = 0;
        public bool Write(Object message)
        {
            String msg = (String) message;
            TextWriter tw=null;
            try
            {
                tw =
                    new StreamWriter(filePath);

                tw.Write(msg);

                tw.Close();

                // for replay purposes for demos. 
                #region replay purposes
                if( backUpEnabled)
                {
                    TextWriter myOwnCopy = new StreamWriter(myBackUpPath + tempReplayCounter + ".xml");
                    tempReplayCounter++;
                    myOwnCopy.Write(msg);
                    myOwnCopy.Close();
                }
                #endregion

                return true;
            }
            catch(IOException exp)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(exp.Message, 60, "FileMediaWriter.cs");
                reporter.SendEmailThread();
                return false;
            }
        }
    }
}
