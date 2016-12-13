using ConsoleApp.entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("VerifyVariableEntity");
            table.CreateIfNotExists();

            //generate test items for inserting in batch
            var entities = Enumerable.Range(1, 100).Select(i => new VerifyVariableEntity()
            {
                ConsumerId = String.Format("{0}", i),
                Score = String.Format("{0}", i * 2 + 2),
                PartitionKey = String.Format("{0}", i),
                RowKey = String.Format("{0}", i * 2 + 2)
            });

            //group by PartitionKey to generate the TableBatchOperation for inserting
            var batches = TableBatchHelper<VerifyVariableEntity>.GetBatches(entities);

            //exexute batch in parallel
            Parallel.ForEach(batches, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5
            }, (batchOperation) =>
            {
                try
                {
                    table.ExecuteBatch(batchOperation);
                    Console.WriteLine("Writing {0} records", batchOperation.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ExecuteBatch throw a exception:" + ex.Message);
                }
            });
            Console.WriteLine("Done!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
