
namespace Orlys.WmiHelper.Internal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class InternalTranspiler<T> : ITranspiler<T>
    {
        private readonly Func<IDictionary<string, object>> _builder;

        internal InternalTranspiler(Func<IDictionary<string, object>> builder)
        {
            this._builder = builder;
        }

        IDictionary<string, object> ITranspiler<T>.Create()
        {
            return this._builder();
        }

        T ITranspiler<T>.Convert(IDictionary<string, object> dict)
        {
            return (T)dict;
        }
    }
}
