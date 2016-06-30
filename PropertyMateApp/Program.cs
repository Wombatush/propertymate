namespace PropertyMateApp
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Autofac;
    using CommandLine;
    using JetBrains.Annotations;
    using PropertyMateApp.Impl;
    using PropertyMateApp.Infrastructure;
    using PropertyMateApp.Properties;
    using Serilog;
    using Serilog.Core;
    
    [UsedImplicitly]
    class Program
    {
        public const string CompanyName = "Ultima Labs";
        public const string ApplicationName = "Property Mate App";

        private static readonly Version Version = Assembly.GetEntryAssembly().GetName().Version;

        static void Main(string[] args)
        {
            var logger = default(ILogger);
            var options = new Options();
            var settings = Settings.Default;

            try
            {
                if (!Parser.Default.ParseArguments(args, options))
                {
                    Environment.Exit(1);
                }

                Console.WriteLine($"{ApplicationName} {Version}");
                Console.WriteLine($"Copyright © {CompanyName} 2016");
                Console.WriteLine();

                var logLevelSwitch = new LoggingLevelSwitch { MinimumLevel = options.LogEventLevel };
                var logConfiguration = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(logLevelSwitch)
                    .WriteTo.ColoredConsole();

                var runtimeOptions = new RuntimeOptions(options, settings);

                if (!string.IsNullOrWhiteSpace(runtimeOptions.LoggingServerUrl))
                {
                    logConfiguration = logConfiguration
                        .WriteTo.Seq(runtimeOptions.LoggingServerUrl);
                }

                Log.Logger = logConfiguration.CreateLogger();
                logger = Log.Logger.ForContext<Program>();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterInstance(options);
                containerBuilder.RegisterInstance(settings);
                containerBuilder.RegisterInstance(runtimeOptions);
                containerBuilder.RegisterModule<AppModule>();
                using (containerBuilder.Build())
                {
                    var awaiter = new AutoResetEvent(false);

                    Console.CancelKeyPress += (s, e) =>
                    {
                        e.Cancel = true;
                        awaiter.Set();
                    };

                    logger.Information("Service is now started, press Ctrl+C for exit");

                    awaiter.WaitOne();
                }
            }
            catch (Exception exception)
            {
                if (logger != null)
                {
                    logger.Fatal(exception, "A fatal unhandled exception occured during execution");
                    Environment.Exit(1);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
