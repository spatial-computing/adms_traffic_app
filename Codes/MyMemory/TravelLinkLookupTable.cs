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
    public abstract class LookupTravelLink : LookupTable<TravelLinksOutputElement>
    {
        private static BootUp memory = BootUp.GetInstance();
        public override Object GetRecord(TravelLinksOutputElement ev, string agency)
        {
            travelLinks meter = new travelLinks(ev.travelId, 0, "", "", 0, 0, 0, "", 0, 0, "", 0, 0, agency);
            //travelLinks meter = memory.GetTravelLinkInfo(ev.travelId);
            //if (meter == null)
            //    return null;

            var values = GetFields(meter, ev);

            return values;
        }

        protected abstract Object GetFields(travelLinks br, TravelLinksOutputElement ev);
    }


    public class LookupTravelLinkDB : LookupTravelLink
    {
        protected override Object GetFields(travelLinks br, TravelLinksOutputElement ev)
        {
            return new List<Object>
                                 {
                                     //Before the debug time, we used the maxRampMeterConfig_id as the config_id in travel time
                                     //The historical data needs to be cleaned
                                     //Debugged 02/07/2013 11:19AM By Penny
                                     //BootUp.maxRampMeterConfigID, //ev.ConfigId,
                                    BootUp.maxTravelTimeConfigID,
                                    ev.StartTime.LocalDateTime.ToString("yyyyMMdd HH:mm:ss"),
                                    ev.travelId,
                                    (double)Utilities.KMH2MPH(ev.speed),
                                    ev.travelTime//,
                                    //br.Agency
                                 };
        }
    }

    public class LookUpTravelLinkAzureTable : LookupTravelLink
    {
        protected override object GetFields(travelLinks br, TravelLinksOutputElement ev)
        {
            return new TravelTimeEntityTuple(br, ev);

        }
    }

}
