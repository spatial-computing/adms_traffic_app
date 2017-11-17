/**
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
    public abstract class LookupCms : LookupTable<CmsOutputElement>
    {
        private static BootUp memory = BootUp.GetInstance();
        public override Object GetRecord(CmsOutputElement ev, string agency)
        {

            CmsDevice device = new CmsDevice(agency); //= memory.GetRailRouteInfo(ev.routeId);
            var values = GetFields(device, ev);

            return values;
        }

        protected abstract Object GetFields(CmsDevice br, CmsOutputElement ev);
    }


    public class LookupCmsDb : LookupCms
    {
        protected override Object GetFields(CmsDevice br, CmsOutputElement ev)
        {
            return new List<Object>
                       {
                           ev.Id,
                           ev.DeviceStatus,
                           ev.State,
                           ev.Date,
                           ev.Time,
                           ev.Phase1Line1,
                           ev.Phase1Line2,
                           ev.Phase1Line3,
                           ev.Phase2Line1,
                           ev.Phase2Line2,
                           ev.Phase2Line3,
                           BootUp.maxCmsConfigID,//ev.ConfigId,
                           ev.StartTime.LocalDateTime.ToString("yyyyMMdd HHmmss")
                          
                       };
            throw new NotImplementedException();
       
        }

        private static String LongToDate(long iDate)
        {
            if (iDate == 10101000000 || iDate == 0)
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
                    return DateTime.ParseExact("00010101000000", "yyyyMMddHHmmss", null).ToString("yyyyMMdd HH:mm:ss");
                }
                return time.ToString("yyyyMMdd HH:mm:ss");
            }
        }

    }

    public class LookUpCmsAzureTable : LookupCms
    {
        protected override object GetFields(CmsDevice br, CmsOutputElement ev)
        {
            return new CmsEntityTuple(br,ev);
        }
    }

}
