﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using EventTypes;
using Microsoft.ComplexEventProcessing;
using MyMemory;
using Parsers;
using Utils;

namespace ADMS
{
    public class PCA
    {
        private static PCA instance;
        private readonly Dictionary<String, PCACoefLoader> allCoefs;
        private readonly BootUp memory;

        private PCA()
        {
            memory = BootUp.GetInstance();
            int[] rows = {
                             6546, 7093, 0, 0, //I-5
                             0, 0, 7042, 7957, // I-10
                             0, 0, 5971, 6659, //I-210
                             6222, 6767, 0, 0, // I-405
                             5073, 5597, 0, 0 // SR-101
                         };

            allCoefs = new Dictionary<string, PCACoefLoader>(rows.Length/2);

            String[] highways = {"I-5", "I-10", "I-210", "I-405", "SR-101"};
            var pcNum = new int[highways.Length*4];

            for (int i = 0; i < pcNum.Length; i++)
                pcNum[i] = 15;
            int dimensions = 901;
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] == 0)
                    continue;
                int direction = i%4;
                int highwayIndex = i/4;
                String key =
                    Constants.GetPreprocessedTableString(Constants.GetUnprocessedTableString(highways[highwayIndex],
                                                                                             direction));
                String coefPath = Constants.PcaCoefPath;
                String indexPath = Constants.PcaIndecesPath;
                String pcFile = coefPath + Constants.GetPCFileName(key);
                String muFile = coefPath + Constants.GetMuString(key);
                String transFile = coefPath + Constants.GetTransFormedDataString(key);
                String allDaysFile = indexPath + Constants.GetAllDaysString(key);
                String sensorDayFile = indexPath + Constants.GetSensorDaysString(key);
                String sensorsTableFile = indexPath + Constants.GetSensorTableString(key);
                allCoefs.Add(key,
                             new PCACoefLoader(rows[i], dimensions, pcNum[i], pcFile, transFile, muFile, allDaysFile,
                                               sensorDayFile, sensorsTableFile));
            }
        }

        public static PCA GetInstance()
        {
            if (instance == null)
                instance = new PCA();
            return instance;
        }

        // Although we have the data for all other dates and they are loaded in memory, for our replay at demo, we are directly returning the proper day as historic value
        public double HistoricValueForSeptember21st(string sensorId, DateTime timeOffset)
        {
            double result = PCACoefLoader.NO_VALUE;


            PCACoefLoader loader = GetLoader(sensorId);

            if (loader != null)
            {
                result = loader.GetSpeedForSeptember21st(sensorId, timeOffset);
            }

            return result;
        }

        public int GetMostSimilar(List<string> sensorIDs, List<IntervalEvent<doubleClass>> toList)
        {
            int Default_For_Unknown = 9;
            int result = 0;
            if (sensorIDs == null || sensorIDs.Count == 0)
                return Default_For_Unknown; // just a default value todo: needs return correct exception or so.

            Sensor highway = null;
            int i = 0;
            string tempSensorId = "";
            while (highway == null)
            {
                tempSensorId = sensorIDs[i];
                BootUp.FreewayLinkLocDic.TryGetValue(tempSensorId.ToString(), out highway);
                
                i++;
            }
            PCACoefLoader loader = GetLoader(tempSensorId);
            if (loader == null)
                return Default_For_Unknown;
            int numOfDays = loader.GetNumOfDays();

            var distances = new double[numOfDays];
            var NumberOfFoundValues = new int[numOfDays];
            foreach (var ev in toList)
            {
                int timeIndex = PCACoefLoader.GetTimeIndex(ev.StartTime.LocalDateTime);
                for (int dayIndex = 0; dayIndex < numOfDays; dayIndex ++)
                {
                    double tempAverage = FindAverage(sensorIDs, dayIndex, timeIndex);
                    if (tempAverage != 0)
                        NumberOfFoundValues[dayIndex]++;
                    // we successfully calculated a value for that minute. Usually the final value should be equal to the number of events.
                    double tempDist = Math.Abs(tempAverage - ev.Payload.Speed);
                    distances[dayIndex] += tempDist;
                }
            }
            double leastValue = distances[0]/NumberOfFoundValues[0];
            for (int dayIndex = 0; dayIndex < numOfDays; dayIndex++)
            {
                if (leastValue > distances[dayIndex]/NumberOfFoundValues[dayIndex])
                {
                    result = dayIndex;
                    leastValue = distances[dayIndex]/NumberOfFoundValues[dayIndex];
                }
            }
            return result;
        }

        private PCACoefLoader GetLoader(string sensorId)
        {
            PCACoefLoader loader = null;
            Sensor highway = null;
            highway= memory.CheckFreewaySensor(sensorId);
            if (highway != null)
            {
                String key = Constants.GetPreprocessedTableString(Constants.GetUnprocessedTableString(highway.OnStreet,
                                                                                                      highway.Direction));
                allCoefs.TryGetValue(key, out loader);
            }
            return loader;
        }

        public double FindAverage(List<string> sensorIDs, int dayIndex, int timeIndex)
        {
            int sensorsWithValue = 0;
            double totalResult = 0;
            foreach (string sensorId in sensorIDs)
            {
                double resultElement = PCACoefLoader.NO_VALUE;
                PCACoefLoader loader = GetLoader(sensorId);
                if (loader != null)
                {
                    resultElement = loader.GetSpeed(sensorId, dayIndex, timeIndex);
                    if (resultElement != PCACoefLoader.NO_VALUE)
                    {
                        totalResult += resultElement;
                        sensorsWithValue++;
                    }
                }
            }
            if (totalResult > 0)
                return totalResult/sensorsWithValue;
            else
            {
                return 0; // no value exists in database for these sensors. returning 0.
            }
        }
    }
}