namespace PropertyMateApp.Infrastructure
{
    using Autofac;
    using JetBrains.Annotations;
    using Nancy.Bootstrappers.Autofac;

    internal sealed class ApiBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope container;

        public ApiBootstrapper([NotNull] ILifetimeScope container)
        {
            this.container = container;
        }

        [NotNull]
        protected override ILifetimeScope GetApplicationContainer() => container;
    }
}