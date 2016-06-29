namespace PropertyMateApp.Impl
{
    using JetBrains.Annotations;

    internal interface IRuntimeOptions
    {
        [NotNull]
        string DataPath { get; }

        [NotNull]
        string BaseServerUrl { get; }

        [NotNull]
        string LoggingServerUrl { get; }

        [NotNull]
        string TargetServerUrl { get; }
    }
}
