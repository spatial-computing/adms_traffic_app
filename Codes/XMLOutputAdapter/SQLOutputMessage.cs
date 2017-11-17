/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using MyMemory;
using OutputTypes;

namespace XMLOutputAdapter
{
    public class SQLOutputMessage<T> : IOutputMessage<T> where T : IOutputType
    {
        private SQLMessageConfig config;
        private LookupTable<T> lookupTable;
        private List<char> tags;
        public SQLOutputMessage(SQLMessageConfig configInfo, LookupTable<T> table)
        {
            config= configInfo;
            lookupTable = table;
            tags = CreateTags(config);
        }

        private List<char> CreateTags(SQLMessageConfig config)
        {
            List<char> result = new List<char>(config.OutputFieldOrders.Count);
            for( int i= 0; i< config.OutputFieldOrders.Count;i++)
            {
                result.Add(default(char));
                string type = config.OutputFieldOrders[i].GetType().ToString();
                if ( type == typeof(string).ToString() || type == typeof(String).ToString())
                    result[i] = '\'';
                
            }
            return result;
        }

        public Object CreateMessage(List<T> buffer)
        {
            string UNION_ALL= " UNION ALL ";
            string SELECT = " SELECT ";
            string SqlStr = //"GO \n" +
                            "insert into " + config.TableName + "( ";
            for( int i = 0; i< config.OutputFieldOrders.Count; i++)
                SqlStr += config.OutputFieldOrders[i] + ",";
            SqlStr= SqlStr.Remove(SqlStr.Length - 1); // remove last comma
            SqlStr += ")";

            //speed, lat, lng, jj, direction, onstreet, fromStreet)"
            int count = 0;
            foreach (T e in buffer)
            {

                var values = (List<object>)lookupTable.GetRecord(e, config.Agency);
                if (values == null) // sensor information not found in our databases.
                    continue;
                        
                SqlStr+= SELECT;
                int i;
                for (i = 0; i < values.Count-1; i++)
                    SqlStr+=  tags[i] + values[i].ToString() + tags[i] + ",";

                SqlStr += tags[i] + values[i].ToString() + tags[i];
                SqlStr += UNION_ALL;

                count++;
            }
            SqlStr = SqlStr.Remove(SqlStr.Count() - UNION_ALL.Count(), UNION_ALL.Count());
            //SqlStr += " GO ";


            //TextWriter tw = new StreamWriter("C:/inetpub/wwwroot/WebApplication1/HighwayTraffic.xml");
            return SqlStr;
        }
        }
    }

