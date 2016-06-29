namespace PropertyMateApp.Impl
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface ISuggestQueryHandler
    {
        IEnumerable<dynamic> Suggest([NotNull] string text, int count);
    }
}