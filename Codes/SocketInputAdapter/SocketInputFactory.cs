/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Globalization;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace SocketInputAdapter
{
    public class SocketInputFactory : IInputAdapterFactory<SocketInputConfig>
    {
        #region IInputAdapterFactory<SocketInputConfig> Members

        public InputAdapterBase Create(SocketInputConfig configInfo, EventShape eventShape, CepEventType cepEventType)
        {
            InputAdapterBase adapter = default(InputAdapterBase);

            if (eventShape == EventShape.Point)
            {
                adapter = new SocketPointInput(configInfo, cepEventType);
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
                                  "SocketInputFactory cannot instantiate adapter with event shape {0}", eventShape));
            }

            return adapter;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}