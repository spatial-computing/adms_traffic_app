/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

namespace Parser
{
    internal class XmlUtil
    {
        private static string myUser = "PhD2";//"udem";
        private static string myPass = "PhD2336";//"udem553";
        private static string myAgency = "USC InfoLab";

        internal static string getUser()
        {
            return myUser;
        }

        internal static string getPass()
        {
            return myPass;
        }

        internal static string getMsgReq(string requestType, string issuingAgency, string verbosity)
        {
            string retrn = "" +
                           "<informationRequest>" +
                           "<messageHeader>" +
                           "<sender>" +
                           "<agencyName>" + myAgency + "</agencyName>" +
                           "</sender>" +
                           // your message id here
                           "<messageID>87654321</messageID>" +
                           "<timeStamp>" +
                           // your date time here
                           "<date>20040826</date>" +
                           "<time>16071800</time>" +
                           "</timeStamp>" +
                           "</messageHeader>" +
                           "<filter>" +
                           "<dataTypes>" +
                           "<localInformationRequestType>" +
                           "<requestType>" + requestType + "</requestType>" +
                           "</localInformationRequestType>" +
                           "</dataTypes>" +
                           "<issueAgencies>" +
                           "<issueAgency>" + issuingAgency + "</issueAgency>" +
                           "</issueAgencies>" +
                           "</filter>" +
                           "<verbosity>" + verbosity + "</verbosity>" +
                           "</informationRequest>";
            return retrn;
        }
    }
}