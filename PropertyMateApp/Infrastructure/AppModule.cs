namespace PropertyMateApp.Infrastructure
{
    using System;
    using Autofac;
    using PropertyMateApp.Impl;
    using PropertyMateApp.Infrastructure.Starters;
    using PropertyMateApp.Properties;
    using Serilog;

    internal sealed class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new RuntimeOptions(
                    c.Resolve<Options>(),
                    c.Resolve<Settings>()))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => new DatabaseRestoreService())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .Register(c => new QueryParser())
                .As<ISuggestQueryParser>()
                .As<ISearchQueryParser>()
                .SingleInstance();

            builder
                .Register(c => new SuggestQueryHandler(
                    c.Resolve<IRuntimeOptions>()))
                .As<ISuggestQueryHandler>()
                .SingleInstance();

            builder
                .Register(c => new SearchQueryHandler(
                    c.Resolve<IRuntimeOptions>()))
                .As<ISearchQueryHandler>()
                .SingleInstance();

            builder
                .Register(c => new ApiBootstrapper(c.Resolve<ILifetimeScope>()))
                .SingleInstance();

            builder.Register(c => new WebAppStarter(
                    c.Resolve<IRuntimeOptions>(),
                    c.Resolve<Func<ApiBootstrapper>>()))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => new DataRestorationStarter(
                    c.Resolve<IRuntimeOptions>(),
                    c.Resolve<IDatabaseRestoreService>()))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => Log.Logger)
                .AsImplementedInterfaces()
                .OnRelease(c => Log.CloseAndFlush())
                .AutoActivate()
                .AsSelf()
                .SingleInstance();
        }
    }
}
