namespace PropertyMateApp.Impl
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using PropertyMateApp.Infrastructure;
    using PropertyMateApp.Properties;
    using Serilog;

    internal sealed class RuntimeOptions : IRuntimeOptions
    {
        private readonly ILogger logger = Log.ForContext<RuntimeOptions>();

        private const string LastResortBaseUrl = "http://localhost:8585";
        private const string LastResortTargetUrl = "http://localhost:9200";

        private readonly Lazy<string> dataPath;
        private readonly Lazy<string> baseServerUrl;
        private readonly Lazy<string> loggingServerUrl;
        private readonly Lazy<string> targetServerUrl;
        
        public RuntimeOptions(
            [NotNull] Options options,
            [NotNull] Settings settings)
        {
            dataPath = new Lazy<string>(() =>
            {
                var path = options.DataPath;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = settings.DataPath;
                }

                if (string.IsNullOrWhiteSpace(path))
                {
                    logger.Warning("No data path was specified, neither as a -d/--data arguments, nor in application configuration file. No data restoration will be performed");
                    path = string.Empty;
                }
                else
                {
                    path = Path.GetFullPath(path);
                }

                logger.Debug("Using {path} as a data path", path);

                return path;
            });

            baseServerUrl = new Lazy<string>(() =>
            {
                var baseUrl = options.BaseServerUrl;
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    baseUrl = settings.BaseServerUrl;
                }

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    logger.Warning("No base server url was specified, neither as a -b/--base arguments, nor in application configuration file. Defaulting to {url}", LastResortBaseUrl);
                    baseUrl = LastResortBaseUrl;
                }

                logger.Debug("Using {url} as a base server", baseUrl);

                return baseUrl;
            });

            loggingServerUrl = new Lazy<string>(() =>
            {
                var loggingUrl = options.LoggingServerUrl;
                if (string.IsNullOrWhiteSpace(loggingUrl))
                {
                    loggingUrl = settings.LoggingServerUrl;
                }

                if (string.IsNullOrWhiteSpace(loggingUrl))
                {
                    logger.Warning("No logging (seq) server url was specified, neither as a -s/--seq arguments, nor in application configuration file. No Seq logging will be used");
                    loggingUrl = string.Empty;
                }

                logger.Debug("Using {url} as a logging (seq) server", loggingUrl);

                return loggingUrl;
            });

            targetServerUrl = new Lazy<string>(() =>
            {
                var targetUrl = options.TargetServerUrl;
                if (string.IsNullOrWhiteSpace(targetUrl))
                {
                    targetUrl = settings.TargetServerUrl;
                }

                if (string.IsNullOrWhiteSpace(targetUrl))
                {
                    logger.Warning("No target (elastic) server url was specified, neither as a -t/--target arguments, nor in application configuration file. Defaulting to {url}", LastResortTargetUrl);
                    targetUrl = LastResortTargetUrl;
                }

                logger.Debug("Using {url} as a target (elastic) server", targetUrl);

                return targetUrl;
            });
        }

        public string DataPath => dataPath.Value;

        public string BaseServerUrl => baseServerUrl.Value;

        public string LoggingServerUrl => loggingServerUrl.Value;

        public string TargetServerUrl => targetServerUrl.Value;
    }
}