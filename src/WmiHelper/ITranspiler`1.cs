
namespace WmiHelper
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface ITranspiler<T>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        IDictionary<string, object> Create();

        [EditorBrowsable(EditorBrowsableState.Never)]
        T Convert(IDictionary<string, object> dict);
    }
}
