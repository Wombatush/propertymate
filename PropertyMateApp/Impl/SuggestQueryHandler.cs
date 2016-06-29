namespace PropertyMateApp.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Nest;

    internal sealed class SuggestQueryHandler : ISuggestQueryHandler
    {
        private readonly ElasticClient client;

        public SuggestQueryHandler([NotNull] IRuntimeOptions options)
        {
            var node = new Uri(options.TargetServerUrl);
            var settings = new ConnectionSettings(node)
                .DefaultIndex("data");
            client = new ElasticClient(settings);
        }

        public IEnumerable<dynamic> Suggest(string text, int count)
        {
            var response = client.Search<dynamic>(descriptor =>
            {
                return descriptor
                    .Type("geo")
                    .From(0)
                    .Size(count)
                    .Sort(s => s.Descending(SortSpecialField.Score))
                    //// .MinScore(1.0)
                    //// .Query(q => q.QueryString(s => s.Fields(f => f.Field("full_address_line")).Query(text)));
                    .Query(q => q.Prefix("full_address_line", text));
            });
            if (response.IsValid)
            {
                return response.Documents
                    .Select(x => new
                    {
                        id = x.address_detail_pid,
                        text = x.full_address_line,
                    });
            }

            /* https://www.elastic.co/guide/en/elasticsearch/guide/current/_index_time_search_as_you_type.html */

            return Enumerable.Empty<dynamic>();
        }
    }
}