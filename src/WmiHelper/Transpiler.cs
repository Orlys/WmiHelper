
namespace WmiHelper
{
    using System.Collections.Generic;
    using System.Dynamic;

    public static class Transpiler
    {
        public readonly static ITranspiler<dynamic> Dynamic
            = new Transpiler<dynamic>(() => new ExpandoObject());

        public readonly static ITranspiler<IReadOnlyDictionary<string, object>> ReadOnlyDictionary
            = new Transpiler<IReadOnlyDictionary<string, object>>(() => new Dictionary<string, object>());
    }
}
