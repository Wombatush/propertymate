namespace PropertyMateApp.Impl
{
    using System.Collections.Generic;

    internal sealed class QueryParser : 
        ISuggestQueryParser,    // Implementations are equal, 
        ISearchQueryParser      // but interfaces are seggregated on purpose
    {
        public bool TryParse(dynamic query, out string text, out int count, out IEnumerable<string> errors)
        {
            var failures = new List<string>();
            if (query != null)
            {
                text = query["text"].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    failures.Add("Query parameter 'text' was either not specified or contained blank text");
                }

                if (!int.TryParse(query["count"].Value?.ToString(), out count))
                {
                    failures.Add("Query parameter 'count' was either not specified or contained invalid integer");
                }
            }
            else
            {
                text = default(string);
                count = default(int);
                failures.Add("The original query was not recognized");
            }

            errors = failures;

            return failures.Count == 0;
        }
    }
}