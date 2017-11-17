// Type: Microsoft.WindowsAzure.StorageClient.TableServiceEntity
// Assembly: Microsoft.WindowsAzure.StorageClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Program Files\Windows Azure SDK\v1.4\ref\Microsoft.WindowsAzure.StorageClient.dll

using System;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.StorageClient
{
    [DataServiceKey(new string[] {"PartitionKey", "RowKey"})]
    [CLSCompliant(false)]
    public abstract class TableServiceEntity
    {
        protected TableServiceEntity(string partitionKey, string rowKey);
        protected TableServiceEntity();
        public DateTime Timestamp { get; set; }
        public virtual string PartitionKey { get; set; }
        public virtual string RowKey { get; set; }
    }
}
