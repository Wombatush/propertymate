namespace PropertyMateApp.Impl
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface ISearchQueryHandler
    {
        [NotNull, ItemNotNull]
        IEnumerable<dynamic> Search([NotNull] string text, int count);
    }
}