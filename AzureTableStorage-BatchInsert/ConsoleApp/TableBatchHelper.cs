using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class TableBatchHelper<T> where T : ITableEntity
    {
        const int batchMaxSize = 100;

        public static IEnumerable<TableBatchOperation> GetBatches(IEnumerable<T> items)
        {
            var list = new List<TableBatchOperation>();
            var partitionGroups = items.GroupBy(arg => arg.PartitionKey).ToArray();
            foreach (var group in partitionGroups)
            {
                T[] groupList = group.ToArray();
                int offSet = batchMaxSize;
                T[] entities = groupList.Take(offSet).ToArray();
                while (entities.Any())
                {
                    var tableBatchOperation = new TableBatchOperation();
                    foreach (var entity in entities)
                    {
                        tableBatchOperation.Add(TableOperation.InsertOrReplace(entity));
                    }
                    list.Add(tableBatchOperation);
                    entities = groupList.Skip(offSet).Take(batchMaxSize).ToArray();
                    offSet += batchMaxSize;
                }
            }
            return list;
        }

        public static async Task InsertOrReplaceBatch(CloudTable table, IEnumerable<T> items)
        {
            var batches = GetBatches(items);
            await Task.WhenAll(batches.Select(table.ExecuteBatchAsync));
        }
    }
}
