/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;

namespace BaseOutputAdapter
{
    public class RawString
    {
        public String Str;

        public RawString(String str)
        {
            Str = str;
        }
    }

    public class RawStringOutput
    {
        public DateTimeOffset StartTime;
        public String Str;

        public RawStringOutput(String str, DateTimeOffset startTime)
        {
            Str = str;
            StartTime = startTime;
        }
    }

    
    //todo: check if we ever use the starttime field (don't mistake it with event start time.
    public class AverageSpeedOutputElement
    {
        public int Speed;
        public DateTimeOffset StartTime;

        public AverageSpeedOutputElement(Double speed, DateTimeOffset startTime)
        {
            Speed = (int) speed;
            StartTime = startTime;
        }
    }

    public class PredictionSpeedOutputElement
    {
        public int Speed;
        public DateTimeOffset StartTime;

        public PredictionSpeedOutputElement(Double speed, DateTimeOffset startTime)
        {
            Speed = (int) speed;
            StartTime = startTime;
        }
    }
}