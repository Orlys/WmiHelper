
namespace Orlys.WmiHelper.Internal
{
    using System;
    using System.Text;
    using System.Collections;

    internal static class WmiDebugHelper
    {
        internal static Func<int, string> Formatter(int count)
        {
            if (count == 1)
                return (i) => null;

            var c = " | ";
            if (count <= 9)
                return (i) => i.ToString("D1") + c;
            if (count <= 99)
                return (i) => i.ToString("D2") + c;
            if (count <= 999)
                return (i) => i.ToString("D3") + c;
            if (count <= 9999)
                return (i) => i.ToString("D4") + c;

            return (i) => i.ToString("D5") + c;
        }

        internal static string DebugString(object value)
        {
            if (value is IList list)
            {
                const string COMMA = ",";
                switch (list.Count)
                {
                    case 0: return string.Empty;
                    case 1: return list[0]?.ToString();
                    case 2: return list[0]?.ToString() + COMMA + list[1]?.ToString();
                    case 3: return list[0]?.ToString() + COMMA + list[1]?.ToString() + COMMA + list[2]?.ToString();
                    default:
                        {
                            var sb = new StringBuilder(list[0]?.ToString());
                            for (int i = 1; i < list.Count; i++)
                            {
                                sb.Append(COMMA).Append(list[i]?.ToString());

                            }
                            return sb.ToString();
                        }

                }

            }
            else
            {
                return value?.ToString() ?? string.Empty;
            }

        }

    }
}
