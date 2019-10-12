
namespace WmiHelper.Internal
{
    internal static class DefaultCache<T>
    {
        internal static readonly T[] Array = new T[0];
        internal static readonly T Value = default(T);
    }
}
