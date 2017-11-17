/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/09/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Parsers
{
    public class D7EventParser : EventParser
    {
        public D7EventParser(string agency,CultureInfo _culture) : base(agency, _culture)
        {
            del = FetchData;
        }

        public override List<string> ReadARecord()
        {
            var result = new List<string>(expectedFieldNum);

            bool goOn = true;
            bool ProcessedClearTime = false;
            bool hasClearTime = false;

            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "event":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "id":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "issuingAgency":
                        textReader.Read(); //issuingAgency
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /issuingAgency
                        break;
                    case "onStreetInfo":
                        textReader.Read(); // onstreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /onstreet
                        break;
                    case "fromStreetInfo":
                        textReader.Read(); // fromstreet
                        textReader.Read(); //name
                        string[] temp = textReader.Value.Split('/');
                        if(temp.Length>1)
                            result.Add(temp[0]+" "+ temp[1]);
                        else
                        {
                            result.Add(textReader.Value);
                        }
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /fromstreet
                        break;

                    case "toStreetInfo":

                        textReader.Read(); // tostreet
                        textReader.Read(); //name
                        if(textReader.Value.Length==0)
                            result.Add(" ");
                        else
                            result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /toStreet
                        break;
                    case "latitude":
                        textReader.Read(); // latitude
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /latitude
                        break;
                    case "longitude":
                        textReader.Read(); // longitude
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /longitude
                        break;

                    case "direction":
                        textReader.Read(); // direction
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /direction
                        break;

                    case "city":
                        textReader.Read(); // city
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /city

                        break;
                    case "postmile":
                        textReader.Read(); // postmile
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /postmile
                        break;
                    case "typeEvent":
                        textReader.Read(); // typeEvent
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /typeEvent
                        break;
                    case "severity":
                        textReader.Read(); // severity
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /severity
                        break;

                    case "description":

                        textReader.Read(); // description
                        textReader.Read(); //text
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /text
                        textReader.Read(); // /description
                        break;
                    case "laneCnt":
                        textReader.Read(); // LaneCnt
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /LaneCnt
                        textReader.Read(); // type
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /type
                        break;
                    case "vehicleType":
                        textReader.Read(); //vehicleType
                        string tempType = textReader.Value;
                        if (textReader.Value.Length == 0)
                            break;  // just an end element, so just jump over it.
                        
                        textReader.Read(); // /vehicleType
                        textReader.Read(); // count
                        textReader.Read(); // value itself ??
                        string tempCnt = textReader.Value;
                        if (textReader.Value.Length == 0)
                            break; // no count value. so just jump.
                        textReader.Read(); // 
                        textReader.Read(); // /count
                        PopulateVehicleType(tempType, tempCnt);
                        break;
                    case "injuries":
                        // if the starting tag of injuries, then enter the values for vehicle types.
                        if (textReader.NodeType != XmlNodeType.EndElement)
                        {
                            result.Add(_9227.ToString());
                            result.Add(_9220.ToString());
                            result.Add(_9228.ToString());
                            result.Add(_9290.ToString());
                        }

                        break;

                    case "injury":
                        textReader.Read(); // injury
                        textReader.Read(); // injuryLevel
                        
                        string tempInj = textReader.Value;
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /injuryLevel
                        textReader.Read(); //count
                        textReader.Read(); // value
                        string cnt = textReader.Value;
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /count
                        PopulateInjury(tempInj, cnt);
                        textReader.Read(); // injury

                        break;

                    case "startTime":
                        // first insert the injury counts.
                        result.Add(fatalityCount.ToString());
                        result.Add(possibleInjCount.ToString());

                        textReader.Read(); // startTime
                        textReader.Read(); // date
                        DateTime date;
                        DateTime.TryParseExact(textReader.Value, "yyyyMMdd", _culture, DateTimeStyles.None, out date);
                        

                        if(textReader.Value == "00000000")
                        {
                            ProcessedClearTime = true;
                        }

                        textReader.Read(); // /date
                        textReader.Read(); // time
                        textReader.Read(); // value
                        DateTime time;


                        DateTime.TryParseExact(textReader.Value, "HHmmssff", _culture, DateTimeStyles.None, out time);
                        DateTime finalDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                        textReader.Read(); // /time

                        result.Add(finalDateTime.ToString());

                        if (ProcessedClearTime)
                            result.Add(finalDateTime.ToString());

                        textReader.Read(); // /startTime
                        break;
                    case "clearTime":
                        hasClearTime = true;
                        textReader.Read(); // clearTime
                        textReader.Read(); // date
                        DateTime clearDate;
                        DateTime.TryParseExact(textReader.Value, "yyyyMMdd", _culture, DateTimeStyles.None, out clearDate);
                        textReader.Read(); // /date

                        textReader.Read(); // time
                        textReader.Read(); // value
                        DateTime clearTime;
                        DateTime.TryParseExact(textReader.Value, "HHmmssff", _culture, DateTimeStyles.None, out clearTime);
                        DateTime clearFinalTime = new DateTime(clearDate.Year, clearDate.Month, clearDate.Day, clearTime.Hour, clearTime.Minute, clearTime.Second);
                        textReader.Read(); // /time

                        if (!ProcessedClearTime)
                            result.Add(clearFinalTime.ToString());

                        textReader.Read(); // /clearTime

                        break;
                    case "issuingUser":

                        if (!hasClearTime && !ProcessedClearTime)
                        {  // add an empty field for clearTime. sometimes events don't have a clearTime. However, as all events are stored in one shape, we consider all of them having a superset of fields.
                            result.Add(DateTime.MinValue.ToString());
                        }

                        textReader.Read(); // issuingUser
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /issuingUser
                        break;
                    case "contactName":
                        textReader.Read(); // contactName
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /contactName
                        break;
                    case "contactPhone":
                        textReader.Read(); // contactPhone
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /contactPhone
                        break;
                    case "highwayPatrol":
                        textReader.Read(); // highwayPatrol
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /highwayPatrol
                        break;
                    case "countyFire":
                        textReader.Read(); // countyFire
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /countyFire
                        break;
                    case "countySheriff":
                        textReader.Read(); // countySheriff
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /countySheriff
                        break;
                    case "fireDepartment":
                        textReader.Read(); // fireDepartment
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /fireDepartment
                        break;
                    case "ambulance":
                        textReader.Read(); // ambulance
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /ambulance
                        break;
                    case "coroner":
                        textReader.Read(); // coroner
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /coroner
                        break;
                    case "mait":
                        textReader.Read(); // mait
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /mait
                        break;
                    case "hazmat":
                        textReader.Read(); // hazmat
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /hazmat
                        break;
                    case "freewayServicePatrol":
                        textReader.Read(); // freewayServicePatrol
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /freewayServicePatrol
                        break;
                    case "caltransMaintenance":
                        textReader.Read(); // caltransMaintenance
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /caltransMaintenance
                        break;
                    case "caltransTMT":
                        textReader.Read(); // caltransTMT
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /caltransTMT
                        break;
                    case "countySheriffTSB":
                        textReader.Read(); // countySheriffTSB
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /countySheriffTSB
                        break;
                    case "other":
                        textReader.Read(); // other
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /value
                        break;
                    
                    case "otherText":
                        textReader.Read(); //otherText
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /value
                        break;
                    case "commentInternal":
                        textReader.Read(); // commentInternal
                        textReader.Read(); // content 
                        if (textReader.Value.Length >= MaxContentLength)
                            result.Add(textReader.Value.Substring(0, MaxContentLength));
                        else
                            result.Add(textReader.Value);

                        if (textReader.Value.Length > 0) 
                            textReader.Read(); // move from value
                        textReader.Read(); // /content 
                        textReader.Read(); // text

                        if (textReader.Value.Length >= MaxContentLength)
                            result.Add(textReader.Value.Substring(0, MaxContentLength));
                        else
                          result.Add(textReader.Value);

                      if (textReader.Value.Length > 0) 
                            textReader.Read(); // move from value
                        textReader.Read(); // /text
                        
                        break;
                    case "commentExternal":
                        textReader.Read(); // commentInternal
                        textReader.Read(); // content 

                        string content = EliminateAnd(textReader.Value);

                        //result.Add(EliminateAnd(textReader.Value));
                        if (textReader.Value.Length > 0)
                         textReader.Read(); // move from value
                        textReader.Read(); // /content 
                        textReader.Read(); // text

                        string text = EliminateAnd(textReader.Value);
                        if (content == text && content.Length / 2>1)
                        {
                            result.Add(content.Substring(0, content.Length/2));
                            result.Add(content.Substring(content.Length/2, content.Length/2-1));
                        }
                        else
                        {
                            result.Add(content);
                            result.Add(text);
                        }
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // move from value
                        textReader.Read(); // /text
                        break;
                                         
                    case "eventStatus":
                        // Adding two dummy fields. D7 events don't include these two fields. However, there are other events which do include them. We store all events in a unified shape in the DB.
                        result.Add(DateTime.MinValue.ToString()); //ActualStartTime
                        result.Add(DateTime.MinValue.ToString()); //ActualEndTime.                        
                        textReader.Read(); // eventStatus
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // value
                        break;
                }

            }
            
            /*StreamWriter wr = new StreamWriter("Event_D7.txt");
            for (int i = 0; i < result.Count; i++ )
            {
                wr.WriteLine((string)result[i]);
            }
            wr.Close();
             * */
            return result;

        }

       
        public override string FetchData()
        {
            return WSDLConnector("event", agency, "real-time");
        }

        public string EliminateAnd(string value)
        {
            string[] item = value.Split('&');
            string result = "";
            for(int i =0; i<item.Length; i++)
            {
                result += item[i];
            }

            if (result.Length >= MaxContentLength)
                result=result.Substring(0, MaxContentLength);

            return result;
        }
    }
}
