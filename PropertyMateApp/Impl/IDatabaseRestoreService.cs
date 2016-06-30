namespace PropertyMateApp.Impl
{
    using JetBrains.Annotations;

    internal interface IDatabaseRestoreService
    {
        void Restore([NotNull] string backupFile, [NotNull] string targetUrl);
    }
}
