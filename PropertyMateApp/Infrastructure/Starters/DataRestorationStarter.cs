namespace PropertyMateApp.Infrastructure.Starters
{
    using Autofac;
    using JetBrains.Annotations;
    using PropertyMateApp.Impl;
    using Serilog;

    internal sealed class DataRestorationStarter : IStartable
    {
        private static readonly ILogger Logger = Log.ForContext<DataRestorationStarter>();

        private readonly IRuntimeOptions options;
        private readonly IDatabaseRestoreService databaseRestoreService;
        
        public DataRestorationStarter(
            [NotNull] IRuntimeOptions options,
            [NotNull] IDatabaseRestoreService databaseRestoreService)
        {
            this.options = options;
            this.databaseRestoreService = databaseRestoreService;
        }

        public void Start()
        {
            Logger.Information("Preparing to import data");

            databaseRestoreService.Restore(options.DataPath, options.TargetServerUrl);
        }
    }
}
