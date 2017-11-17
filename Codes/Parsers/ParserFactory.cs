/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the ParsersTypes for Bus, Rail, RMS, TravelTime
 * Date: 04/18/2011
 */


using System;
using System.Globalization;

namespace Parsers
{
    public class ParserFactory
    {
        public static BaseFileParser Create(RIITSDataTypes fetchType, ReadType recordType, string agency, CultureInfo culture)
        {
            BaseFileParser result = default(BaseFileParser);
            //if (recordType == ReadType.Raw)
            //    result = new RawFileParser(GetFetcher(fetchType),agency);
            //else // reading record by record
            {
                switch (fetchType)
                {
                    case (RIITSDataTypes.Freeway):
                        result = new FreewayParser(agency);
                        break;
                    case (RIITSDataTypes.Arterial):
                        result = new ArterialParser(agency);
                        break;
                    case (RIITSDataTypes.Bus):
                        result = new BusParser(agency);
                        break;
                    case (RIITSDataTypes.Rail):
                        result = new RailParser(agency);
                        break;
                    case (RIITSDataTypes.Ramp):
                        result = new RampParser(agency);
                        break;
                    case (RIITSDataTypes.Traveltimes):
                        result = new TravelTimeParser(agency);
                        break;
                    case (RIITSDataTypes.Event):
                        if (agency == RIITSFreewayAgency.Caltrans_D7 || 
                            agency == RIITSFreewayAgency.Caltrans_D8 || 
                            agency == RIITSFreewayAgency.Caltrans_D12)
                            result = new D7EventParser(agency, culture);
                        else if (agency == RIITSEventAgency.Regional_LA)
                            result = new RegionalLAEventParser(agency, culture);
                        else
                            result = new CHPEventParser(agency, culture);
                        break;
                    case (RIITSDataTypes.CMS):
                        result = new CmsParser(agency);
                        break;
                }
            }
            return result;
        }

        
    }
}