namespace PropertyMateApp.Infrastructure.Starters
{
    using System;
    using Autofac;
    using JetBrains.Annotations;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Hosting;
    using Owin;
    using PropertyMateApp.Impl;
    using Serilog;

    internal sealed class WebAppStarter : IStartable, IDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<WebAppStarter>();

        private readonly IRuntimeOptions options;

        private readonly Func<ApiBootstrapper> bootstrapperFactory;

        private IDisposable disposable;
        
        public WebAppStarter(
            [NotNull] IRuntimeOptions options,
            [NotNull] Func<ApiBootstrapper> bootstrapperFactory)
        {
            this.options = options;
            this.bootstrapperFactory = bootstrapperFactory;
        }

        public void Start()
        {
            Logger.Information("Starting web app");

            disposable?.Dispose();
            disposable = WebApp.Start(options.BaseServerUrl, app =>
            {
                Logger.Debug("Configuring web app");
                app.UseCors(CorsOptions.AllowAll);
                app.UseNancy(options =>
                {
                    options.Bootstrapper = bootstrapperFactory();
                });
            });
        }

        public void Dispose()
        {
            Logger.Debug("Disposing web app");

            disposable?.Dispose();
        }
    }
}
