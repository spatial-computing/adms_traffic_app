using System;
using BaseOutputAdapter;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace XMLOutputAdapter
{
    public class SocketTrafficOutputConfig : TrafficOutputConfig
    {
        public string IPAddress { get; set; }
        public int PortNumber { get; set; }
    }

    public class SocketOutputFactory : IOutputAdapterFactory<SocketTrafficOutputConfig>
    {
        #region IOutputAdapterFactory<SocketTrafficOutputConfig> Members

        public OutputAdapterBase Create(SocketTrafficOutputConfig configInfo, EventShape eventShape,
                                        CepEventType cepEventType)
        {
            OutputAdapterBase adapter = default(OutputAdapterBase);

            switch (eventShape)
            {
                case EventShape.Point:
                    if (configInfo.OutputType.Equals(typeof (ArterialSensorSpeedOutputElement).ToString()))
                        adapter = new SocketPointArterialSensorSpeedOutput(configInfo, cepEventType);
                    else if (configInfo.OutputType.Equals(typeof (FreewaySensorSpeedOutputElement).ToString()))
                        adapter = new SocketPointFreewaySensorSpeedOutput(configInfo, cepEventType);
                    break;
                case EventShape.Interval:

                    throw new NotImplementedException();
                    break;
                case EventShape.Edge:
                    throw new NotImplementedException();
                    break;
                default:
                    break;
            }

            return adapter;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}