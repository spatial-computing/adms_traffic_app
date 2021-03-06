﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace ADMS
{
    public class PCACoefLoader
    {
        public static readonly int NO_VALUE = -1;
        private readonly int[] allDays;
        private readonly int dimensions;
        private readonly int numOfDays;
        private readonly int pcNum;
        private readonly int rowNum;
        private readonly int[] sensorDayTable;
        private readonly Dictionary<string, int> sensorTable;
        private double[] MUs;
        private double[][] PCs;
        private double[][] transformedData;

        public PCACoefLoader(int rows, int dimensions, int PCs, String pcFile, String transFile, String muFile,
                             String allDayFile, String sensorDayFile, String sensorTableFile)
        {
            rowNum = rows;
            pcNum = PCs;
            this.dimensions = dimensions;
            LoadMU(muFile);
            LoadPCs(pcFile);
            LoadTransformedData(transFile);
            sensorDayTable = LoadIntegerArray(sensorDayFile);
            allDays = LoadIntegerArray(allDayFile);
            numOfDays = allDays.Length;
            sensorTable = LoadSensors(sensorTableFile);
        }

        public int GetNumOfDays()
        {
            return numOfDays;
        }

        public double GetSpeedForSeptember21st(string sensorId, DateTime offset)
        {
            //todo: needs a mechanism to find the specified date.
            double result = NO_VALUE;
            int dayIndex = 4; // for Monday Sep 21 2009 which is equivalent to Monday August 30 2010

            return GetSpeed(sensorId, dayIndex, GetTimeIndex(offset));
        }

        public static int GetTimeIndex(DateTime time)
        {
            return (time.Hour - 6)*60 + time.Minute; // minutes from 6 am.
        }

        public double GetSpeed(string sensorId, int dayIndex, int timeIndex)
        {
            double result = NO_VALUE;
            if (dayIndex >= numOfDays)
                return result;

            Int32 sensorIndex = -1;
            sensorTable.TryGetValue(sensorId, out sensorIndex);
            //todo: what if the sensorId is not there? (for now we check it before calling this function)
            int tempIndex = sensorIndex*numOfDays + dayIndex;
            int realIndex = sensorDayTable[tempIndex];

            if (realIndex != -1) // if there is a record for that sensor-day
                result = RetData(realIndex, timeIndex);

            return result;
        }

        private Dictionary<string, int> LoadSensors(String sensorTableFile)
        {
            var result = new Dictionary<string, int>();
            TextReader reader = new StreamReader(sensorTableFile);

            string line = reader.ReadLine();
            for (int i = 0; line != null; line = reader.ReadLine(), i++)
                result.Add(line, i);

            reader.Close();
            return result;
        }

        private int[] LoadIntegerArray(String sensorDayFile)
        {
            var tempList = new List<int>();

            TextReader reader = new StreamReader(sensorDayFile);

            string line = reader.ReadLine();
            while (line != null)
            {
                tempList.Add(Int32.Parse(line));
                line = reader.ReadLine();
            }

            reader.Close();
            return tempList.ToArray();
        }

        private void LoadMU(String muFile)
        {
            MUs = new double[dimensions];
            try
            {
                TextReader reader = new StreamReader(muFile);

                for (int i = 0; i < dimensions; i++)
                    MUs[i] = Double.Parse(reader.ReadLine());
                reader.Close();
            }
            catch (FileNotFoundException e1)
            {
                Console.WriteLine("No such file: " + muFile);
                MUs = null;
            }
        }

        private void LoadPCs(String pcFile)
        {
            TextReader reader = new StreamReader(pcFile);
            PCs = new double[pcNum][];
            for (int i = 0; i < pcNum; i++)
            {
                PCs[i] = new double[dimensions];
                for (int j = 0; j < dimensions; j++)
                {
                    PCs[i][j] = Double.Parse(reader.ReadLine());
                }
            }

            reader.Close();
        }

        private void LoadTransformedData(String transFile)
        {
            transformedData = new double[rowNum][];
            TextReader reader = new StreamReader(transFile);
            for (int i = 0; i < rowNum; i++)
            {
                transformedData[i] = new double[pcNum];
                for (int j = 0; j < pcNum; j++)
                    transformedData[i][j] = Double.Parse(reader.ReadLine());
            }

            reader.Close();
        }

        // retrieve the real data for a point
        public double RetData(int row, int column)
        {
            if (column > 900)
                column = 900; // we don't have PCAs for after 9pm.
            if (column < 0)
                column = 0; // we don't have PCAs for before 6am.
            double result = 0;
            for (int i = 0; i < pcNum; i++)
                result += transformedData[row][i]*PCs[i][column];
            result += MUs[column];
            return result;
        }
    }
}