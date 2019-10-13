
namespace Orlys.WmiHelper
{
    using System.Collections.Generic;
    using System.Dynamic;
    using Orlys.WmiHelper.Internal;

    public static class Transpiler
    {
        public readonly static ITranspiler<dynamic> Dynamic
            = new InternalTranspiler<dynamic>(() => new ExpandoObject());
         
        public readonly static ITranspiler<IReadOnlyDictionary<string, object>> ReadOnlyDictionary
            = new InternalTranspiler<IReadOnlyDictionary<string,object>>(() => new Dictionary<string,object>());
    }
}
