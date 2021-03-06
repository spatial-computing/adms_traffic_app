﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Xml;
using Parser;
using Parsers.CongestionService1;

namespace Parsers
{
    public abstract class InventoryParser : BaseFileParser
    {
        public InventoryParser(string agency, string datatype)
            : base(agency, "links", datatype)
        {
            expectedFieldNum = 9;
            del = FetchData;
            
        }

        public Sensor ReadASensorInfo()
        {
            return new Sensor(agency, ReadARecord());
        }


        public override List<string> ReadARecord()
        {
            var result = new List<string>(11);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "link":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "id":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "onStreetInfo":
                        textReader.Read(); // onstreet
                        textReader.Read(); //name

                        if (!textReader.Value.Contains("\'"))
                            result.Add(textReader.Value);
                        else
                            result.Add(textReader.Value.Replace('\'', ' '));
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /onstreet
                        break;
                    case "fromStreetInfo":
                        textReader.Read(); // fromstreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /fromstreet
                        break;

                    case "toStreetInfo":

                        textReader.Read(); // tostreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /toStreet
                        break;
                    /*
                    case "onStreetInfo":
                        textReader.Read(); // onstreet
                        textReader.Read(); //name


                        if (!textReader.Value.Contains("\'"))
                            result.Add(textReader.Value);
                        else
                        {
                            string processedString = "";
                            string[] item = textReader.Value.Split('\'');
                            for (int i = 0; i < item.Length - 1; i++)
                            {
                                processedString += item[i] + "\'\'";
                            }
                            processedString += item[item.Length - 1];
                            result.Add(processedString);
                        }
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /onstreet
                        break;
                    case "fromStreetInfo":
                        textReader.Read(); // fromstreet
                        textReader.Read(); //name

                        if (!textReader.Value.Contains("\'"))
                            result.Add(textReader.Value);
                        else
                        {
                            string processedString = "";
                            string[] item = textReader.Value.Split('\'');
                            for (int i = 0; i < item.Length - 1; i++)
                            {
                                processedString += item[i] + '\'' + '\'';
                            }
                            processedString += item[item.Length - 1];
                            result.Add(processedString);
                        }

                        //result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /fromstreet
                        break;

                    case "toStreetInfo":

                        textReader.Read(); // tostreet
                        textReader.Read(); //name


                        if (!textReader.Value.Contains("\'"))
                            result.Add(textReader.Value);
                        else
                        {
                            string processedString = "";
                            string[] item = textReader.Value.Split('\'');
                            for (int i = 0; i < item.Length - 1; i++)
                            {
                                processedString += item[i] + "\'\'";
                            }
                            processedString += item[item.Length - 1];
                            result.Add(processedString);
                        }

                        //result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /toStreet
                        break;*/
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

                    case "laneCnt":
                        textReader.Read(); // LaneCnt
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /LaneCnt
                        break;
                    case "type":
                        textReader.Read(); // type
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /type
                        break;
                }
            }


            return result;
        }

        public abstract string FetchData();
    }
}