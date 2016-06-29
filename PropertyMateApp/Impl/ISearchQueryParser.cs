namespace PropertyMateApp.Impl
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface ISearchQueryParser
    {
        [ContractAnnotation("query:null=>false; =>false,text:null; =>true,text:notnull")]
        bool TryParse([CanBeNull] dynamic query, [CanBeNull] out string text, out int count, [NotNull, ItemNotNull] out IEnumerable<string> errors);
    }
}