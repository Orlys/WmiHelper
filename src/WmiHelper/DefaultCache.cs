
namespace WmiHelper
{
    internal static class DefaultCache<T>
    {
        internal static readonly T[] Array = System.Array.Empty<T>();
        internal static readonly T Value = default(T);
    }
}
