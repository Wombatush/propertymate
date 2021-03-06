﻿namespace PropertyMateApp.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Nancy;
    using PropertyMateApp.Impl;
    using Serilog;
    using Serilog.Events;

    [UsedImplicitly]
    public sealed class ApiModule : NancyModule
    {
        private static readonly ILogger Logger = Log.ForContext<ApiModule>();

        public ApiModule(
            [NotNull] ISuggestQueryParser suggestQueryParser,
            [NotNull] ISuggestQueryHandler suggestQueryHandler,
            [NotNull] ISearchQueryParser searchQueryParser,
            [NotNull] ISearchQueryHandler searchQueryHandler)
        {
            Logger.Debug("Starting module {module}", nameof(ApiModule));

            Get["/suggest"] = _ =>
                {
                    Logger.Verbose("Processing {method} method for the path {path}", Request.Method, Request.Path);

                    using (Logger.BeginTimedOperation("suggest", level: LogEventLevel.Verbose))
                    {
                        string text;
                        int count;
                        IEnumerable<string> errors;
                        if (!suggestQueryParser.TryParse(Request.Query, out text, out count, out errors))
                        {
                            return Response.AsJson(errors, HttpStatusCode.BadRequest);
                        }

                        return Response.AsJson(new { matches = suggestQueryHandler.Suggest(text, count) });
                    }
                };


            Get["/query"] = _ => 
            {
                Logger.Verbose("Processing {method} method for the path {path}", Request.Method, Request.Path);

                using (Logger.BeginTimedOperation("query", level: LogEventLevel.Verbose))
                {
                    string text;
                    int count;
                    IEnumerable<string> errors;
                    if (!searchQueryParser.TryParse(Request.Query, out text, out count, out errors))
                    {
                        return Response.AsJson(errors, HttpStatusCode.BadRequest);
                    }

                    return Response.AsJson(new { matches = searchQueryHandler.Search(text, count) });
                }
            };
        }
    }
}