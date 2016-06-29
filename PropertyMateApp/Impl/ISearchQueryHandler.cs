namespace PropertyMateApp.Impl
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface ISearchQueryHandler
    {
        IEnumerable<dynamic> Search([NotNull] string text, int count);
    }
}