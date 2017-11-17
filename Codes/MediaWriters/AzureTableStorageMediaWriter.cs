/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using EventTypes;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using ExceptionReporter;

namespace MediaWriters
{
    public class AzureTableStorageMediaWriter<T> : MediaWriter  where T : TableServiceEntity
    {
        private string storageConnectionString;
        CloudStorageAccount Account;
        CloudTableClient TableClient;
        private const int maxInsertsPerBatch = 100;//100 Jalal: commented because some data sources (ie. traveltimedata, busgps data, and rail) send duplicate records.
        // I informed the delcan and IMSC folks through email on 07/04/2011. Penny or folks going to work on this project in future may do a workaround to 
        // minimize cloud storage costs by sending records in batch to the cloud (ie. using 100 instead of 1).
        private string tableName;
        public AzureTableStorageMediaWriter(string tableName)
        {
            this.tableName = tableName.ToLower();
            storageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
            
            Account = CloudStorageAccount.Parse(storageConnectionString);

            TableClient = Account.CreateCloudTableClient();
            TableClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
           // CreateTablesIfNotExists();
        }

        public bool Write(Object message)
        {
            try
            {
                List<Object> list = (List<Object>)message;
                bool result = true;
                int i = 0;
                HashSet<string> rowkeys = new HashSet<string>();

                for (i = 0; i < list.Count; i++) {

                    //if (tableName == "busgpsdata")
                        //rowkeys.Add(list.ElementAt(i));
                }

                for (i = 0; i < list.Count / maxInsertsPerBatch; i++)
                {
                    InsertBatchEntity(tableName, list.GetRange(i * maxInsertsPerBatch, maxInsertsPerBatch));
                }

                // Last chunk of the list.
                if (i * maxInsertsPerBatch < list.Count)
                    InsertBatchEntity(tableName, list.GetRange(i * maxInsertsPerBatch, list.Count - i * maxInsertsPerBatch));

                return result;
            }
            catch (Exception e)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 67, "AzureTableStorageMediaWriter.cs");
                reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(e.Message, 67, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();

                return false;
            }

        }

        // Return true on success, false if not found, throw exception on error.
        private bool InsertBatchEntity(string tableName, List< Object> list)
        {
            try
            {
                if (list == null || list.Count == 0)
                    return true;
                TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
                
                foreach (T entity in list)
                {
                    tableServiceContext.AddObject(tableName, entity);
                }
                
                tableServiceContext.SaveChanges(SaveChangesOptions.Batch);

                return true;
            }
            catch (DataServiceRequestException EX)
            {
              //  Console.WriteLine("Exception while writing to azure table "+ tableName);
               // ExceptionEmailReporter reporter = new ExceptionEmailReporter(EX.StackTrace, 87, "AzureTableStorageMediaWriter.cs");
             //  reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(EX.StackTrace, 99, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();

                return false;
            }
            catch (StorageClientException ex)
            {
                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 94, "AzureTableStorageMediaWriter.cs");
                //reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message, 111, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();

                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
            catch(Exception ex)
            {
                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 105, "AzureTableStorageMediaWriter.cs");
                //reporter.SendEmailThread();
                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message, 127, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();

                return false;
            }
        }   

        // Return true on success, false if already exists, throw exception on error.
        private bool CreateTablesIfNotExists()
        {
            try
            {
                TableClient.CreateTable(tableName);

               
                return true;
            }
            catch (StorageClientException ex)
            {
                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 123, "AzureTableStorageMediaWriter.cs");
                //reporter.SendEmailThread();
                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message,149, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();
                if ((int)ex.StatusCode == 409)
                {
                    return false;
                }

                throw;
            }
            catch(Exception ex)
            {
                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 134, "AzureTableStorageMediaWriter.cs");
                //reporter.SendEmailThread();
                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message, 163, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();
                return false;   
            }
            finally
            {
                // test for jalal
                //FreewaySensorEntityTuple temp = JalalTest();
            }
        }

        //private FreewaySensorEntityTuple JalalTest()
        //{
        //    FreewaySensorEntityTuple result;
        //    GetEntity<FreewaySensorEntityTuple>("highwaycongestiondata",
        //                 SensorEntityTuple.CalcPartition(new DateTime(2011, 3, 1), "Caltrans-D7", "FREEWAY"),
        //                 SensorEntityTuple.CalcRowKey(716499, new DateTime(2011, 3, 22, 20, 29, 21)), out result);
        //    return result;

        //}

        // Retrieve an entity.
        // Return true on success, false if not found, throw exception on error.

        private bool GetEntity<T>(string tableName, string partitionKey, string rowKey, out T entity) where T : TableServiceEntity
        {
            entity = null;

            try
            {
                TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
                IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                          where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                          select e);

                entity = entities.FirstOrDefault();

                return true;
            }
            catch (DataServiceRequestException ex)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 207, "AzureTableStorageMediaWriter.cs");
                reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message, 207, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();

                return false;
            }
            catch (StorageClientException ex)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(ex.Message, 218, "AzureTableStorageMediaWriter.cs");
                reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(tableName);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(ex.Message, 218, "AzureTableStorageMediaWriter.cs", datatype, "Azure");
                reporter2.SendDatabaseExceptionThread();
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }
    }
}
