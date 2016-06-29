namespace PropertyMateApp.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Threading.Tasks;
    using Nest;
    using Newtonsoft.Json.Linq;
    using Serilog;

    internal sealed class DatabaseRestoreService : IDatabaseRestoreService
    {
        private const int NumItemsInBulkGroup = 1000;

        private static readonly ILogger Logger = Log.ForContext<DatabaseRestoreService>();
        
        public void Restore(string backupFile, string databaseUrl)
        {
            if (!string.IsNullOrWhiteSpace(backupFile))
            {
                if (!File.Exists(backupFile))
                {
                    Logger.Error("No data will be restored, file not found: {path}", backupFile);
                    return;
                }

                var node = new Uri(databaseUrl);
                var settings = new ConnectionSettings(node);
                var client = new ElasticClient(settings);
                var al = client.CatIndices();
                if (al.Records.Any(x => string.Equals(x.Index, "data", StringComparison.InvariantCultureIgnoreCase)))
                {
                    client.DeleteIndex(new DeleteIndexRequest("data"));
                }

                client.CreateIndex(new CreateIndexRequest("data"));

                using (Logger.BeginTimedOperation("Bulk data indexing"))
                using (var file = MemoryMappedFile.CreateFromFile(backupFile, FileMode.Open))
                using (var view = file.CreateViewStream())
                using (var reader = new StreamReader(view))
                {
                    var total = 0;
                    var tasks = new List<Task>();
                    var buffer = new List<string>(NumItemsInBulkGroup);
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        buffer.Add(line);

                        if (buffer.Count < NumItemsInBulkGroup)
                        {
                            continue;
                        }

                        total += buffer.Count;
                        var capture = buffer;
                        buffer = new List<string>();

                        tasks.Add(Task.Factory.StartNew(() => Push(client, capture)));
                    }

                    if (buffer.Count > 0)
                    {
                        total += buffer.Count;
                        tasks.Add(Task.Factory.StartNew(() => Push(client, buffer)));
                    }

                    Task.WaitAll(tasks.ToArray());

                    Logger.Information("Total {count} records has been indexed", total);
                }
            }
        }

        private void Push(ElasticClient client, List<string> buffer)
        {
            Logger.Verbose("A buffer of {count} records is about to be sent for bulk indexing", buffer.Count);

            var descriptor = new BulkDescriptor();
            foreach (var line in buffer)
            {
                var instance = JObject.Parse(line);
                var identifier = instance.Property("_id");
                var id = identifier.Value.ToString();
                identifier.Remove();
                
                descriptor
                    .Index<JObject>(i => i
                        .Index("data")
                        .Type("geo")
                        .Id(id)
                        .Document(instance));
            }
            
            var response = client.Bulk(descriptor);
            if (response.Errors)
            {
                Logger.Debug("Builk insert failed: {errors}", response.ItemsWithErrors);
            }
        }
    }
}