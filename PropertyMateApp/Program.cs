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
                    logger.Fatal(exception, "A fatal unhandled exception occured during execution.");
                    Environment.Exit(1);
                }
                else
                {
                    throw;
                }
            }

            

            
                
                /*
                    if (string.IsNullOrWhiteSpace(string.Empty))
                    {
                        logger.Information("Data update is ignored, as no data path was provided");
                    }
                    else
                    {
                        var dataFilePath = Path.GetFullPath(options.DataPath);
                        if (!File.Exists(dataFilePath))
                        {
                            logger.Error("Data file not found: {path}", dataFilePath);
                            Environment.Exit(1);
                        }

                        

                        //// var request = WebRequest.CreateHttp(new Uri(new Uri(options.TargetServerUrl), "data/_bulk"));
                        //// request.Credentials = CredentialCache.DefaultCredentials;
                        //// request.Method = "POST";
                        //// var dstStream = request.GetRequestStream();


                        List<dynamic> Documents = new List<dynamic>();

                        using (var mappedDataFile = MemoryMappedFile.CreateFromFile(dataFilePath, FileMode.Open))
                        {
                            using (var srcStream = mappedDataFile.CreateViewStream())
                            {

                                //// using (var writer = new StreamWriter(dstStream))
                                {
                                    //// writer.WriteLine("{ \"index\" : { \"_index\":\"data\", \"_type\": \"geo\" } }");
                                    using (var reader = new StreamReader(srcStream))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var instance = JObject.Parse(reader.ReadLine());
                                            var val = instance.Property("_id").Value.ToString();
                                            instance.Property("_id").Remove();
                                            logger.Verbose("Indexing {data}", instance.ToString(Formatting.None));
                                            client.Index(instance, x => x.Index("data").Type("geo").Id(val));

                                            //// Documents.Add(instance);

                                            //// var processed = instance.ToString(Formatting.None);
                                            //// writer.WriteLine(processed);
                                        }



                                        //// using (var dstStream = request.GetRequestStream())
                                        //// {
                                        ////     srcStream.CopyTo(dstStream);
                                        //// }

                                    }
                                }
                            }


                            //Populate Documents

                            ///// var descriptor = new BulkDescriptor();
                            ///// foreach (var doc in Documents)
                            ///// {
                            /////     descriptor
                            /////         .Index<object>(i => i
                            /////             .Index("data")
                            /////             .Type("geo")
                            /////             .Document(doc));
                            ///// }
                            ///// 
                            ///// client.Bulk(descriptor);


                            //// request.GetResponse().Dispose();
                        }
                    }

                //// using (WebApp.Start<Startup>(url))
                //// {
                ////     logger.Information("Running on {url}", url);
                ////     logger.Information("Press enter to exit");
                ////     Console.ReadLine();
                //// }*/
            
        }
    }
}
