namespace PropertyMateApp.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Nest;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class SearchQueryHandler : ISearchQueryHandler
    {
        private readonly ElasticClient client;

        public SearchQueryHandler([NotNull] IRuntimeOptions options)
        {
            var node = new Uri(options.TargetServerUrl);
            var settings = new ConnectionSettings(node)
                .DefaultIndex("data");
            client = new ElasticClient(settings);
        }

        public IEnumerable<dynamic> Search(string text, int count)
        {
            var response = client.Search<JObject>(descriptor =>
            {
                return descriptor
                    .Type("geo")
                    .From(0)
                    .Size(count)
                    .Sort(s => s.Descending(SortSpecialField.Score))
                    .MinScore(1.0)
                    .Query(q => q.QueryString(s => s.Fields(f => f.Field("full_address_line")).Query(text)));
            });
            if (response.IsValid)
            {
                return response.Documents;
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}