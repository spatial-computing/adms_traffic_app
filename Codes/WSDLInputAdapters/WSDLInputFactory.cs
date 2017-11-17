/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Globalization;
using BaseTrafficInputAdapters;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using WSDLInputAdapters;

namespace SocketInputAdapter
{
    public class WSDLInputFactory : IInputAdapterFactory<RIITSInputConfig>
    {
        #region IInputAdapterFactory<RIITSInputConfig> Members

        public InputAdapterBase Create(RIITSInputConfig configInfo, EventShape eventShape, CepEventType cepEventType)
        {
            InputAdapterBase adapter = default(InputAdapterBase);

            if (eventShape == EventShape.Point)
            {
                adapter = new WSDLPointInput(configInfo, cepEventType);
            }
            else if (eventShape == EventShape.Interval)
            {
                throw new NotImplementedException();
            }
            else if (eventShape == EventShape.Edge)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                                  "WSDLInputFactory cannot instantiate adapter with event shape {0}", eventShape));
            }

            return adapter;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}