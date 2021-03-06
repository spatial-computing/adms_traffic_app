﻿/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Added the Azure Lookup Table
 * Date: 06/09/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using MyMemory;
using OutputTypes;
using Parsers;
using Utils;

namespace MyMemory
{
    public abstract class LookupEvent : LookupTable<EventEntityTuple>
    {
        private static BootUp memory = BootUp.GetInstance();
        public override Object GetRecord(EventEntityTuple ev, string agency)
        {
            // there is no configuration for events! because they are unpredicted events! no configuration is available. If it were, we would follow the same process like other look up tables
           var values = GetFields(ev);

            return values;
        }

        protected abstract Object GetFields( EventEntityTuple ev);
    }


    public class LookupEventDB : LookupEvent
    {
        protected override Object GetFields( EventEntityTuple ev)
        {
            return new List<Object>
                       {
                           ev.EventId,
                           ev.Agency,
                           ev.OnStreet,
                           ev.FromStreet,
                           ev.ToStreet,
                           ev.Latitude,
                           ev.Longitude,
                           ev.Direction,
                           ev.AdminCity,
                           ev.AdminPostmile,
                           ev.TypeEvent,
                           ev.Severity,
                           ev.Description,
                           ev.AffectedLaneCnt,
                           ev.AffectedLaneType,
                           ev.VecType9220,
                           ev.VecType9227,
                           ev.VecType9228,
                           ev.VecType9290,
                           ev.FatalityCnt,
                           ev.PossibleInjCnt,
                           LongToDate(ev.iStartTime),
                           LongToDate(ev.iClearTime),
                           ev.IssuingUser,
                           ev.ContactName,
                           ev.ContactPhone,
                           ev.HighwayPatrol,
                           ev.CountyFire,
                           ev.CountySheriff,
                           ev.FireDepartment,
                           ev.Ambulance,
                           ev.Coroner,
                           ev.Mait,
                           ev.Hazmat,
                           ev.FreewayServicePatrol,
                           ev.CaltransMaintenance,
                           ev.CaltransTMT,
                           ev.CountySheriffTSB,
                           ev.Other,
                           ev.OtherText,
                           ev.CommentInternalContent,
                           ev.CommentInternalText,
                           ev.CommentExternalContent,
                           ev.CommentExternalText,
                           LongToDate(ev.iActualStartTime),
                           LongToDate(ev.iActualEndTime),
                           ev.EventStatus,
                           ev.datetime.ToString("yyyyMMdd HH:mm:ss")
                       };
            throw new NotImplementedException();
            //return new List<Object>
            //                     {
            //                         BootUp.maxRampMeterConfigID, //ev.ConfigId,
            //                        ev.StartTime.LocalDateTime.ToString("yyyyMMdd HH:mm:ss"),
            //                        ev.travelId,
            //                        (double)Utilities.KMH2MPH(ev.speed),
            //                        ev.travelTime//,
            //                        //br.Agency
            //                     };
        }
        private static String LongToDate(long iDate)
        {
            if (iDate == 10101000000)
            {
                return DateTime.ParseExact("00010101000000", "yyyyMMddHHmmss", null).ToString("yyyyMMdd HH:mm:ss");
            }
            else
            {
                DateTime time = default(DateTime);
                try
                {
                    time = DateTime.ParseExact(iDate.ToString(), "yyyyMMddHHmmss", null);
                }
                catch (Exception)
                {

                }
                return time.ToString("yyyyMMdd HH:mm:ss");
            }
        }

    }

    public class LookUpEventAzureTable : LookupEvent
    {
        protected override object GetFields(EventEntityTuple ev)
        {
            return ev;

        }
    }

}
