
namespace WmiHelper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Collections;
    using System.Net;
    using System.Net.NetworkInformation;

    internal static class CimTypeConverter
    {
        private static DateTime ConvertDateTimeStringToDateTime(string fmt)
        {
            var pattern = "yyyyMMddHHmmss.ffffff+000";
            if (DateTime.TryParseExact(fmt, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            pattern = pattern.Remove(pattern.Length - 4);
            fmt = fmt.Remove(fmt.Length - 4);

            if (DateTime.TryParseExact(fmt, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;

            throw new NotSupportedException();
        }
        internal static bool TryConvertDateTime(object raw, bool isArray, out object result)
        {
            if (isArray)
            {
                if (raw is string[] array)
                {
                    result = new DateTime[array.Length];
                    for (int i = 0; i < ((DateTime[])result).Length; i++)
                    {
                        ((DateTime[])result)[i] = ConvertDateTimeStringToDateTime(array[i]);
                    }
                    return true;
                }
                result = DefaultCache<DateTime>.Array;
                return false;
            }
            if (raw is string value)
            {
                result = ConvertDateTimeStringToDateTime(value);
                return true;
            }
            result = DefaultCache<DateTime>.Value;
            return false;
        }

        internal static bool TryConvert<T>(object raw, bool isArray, out object result)
        {
            if (isArray)
            {
                if (raw is T[] array)
                {
                    result = array;
                    return true;
                }
                result = DefaultCache<T>.Array;
                return false;

            }
            if (raw is T value)
            {
                result = value;
                return true;
            }

            result = DefaultCache<T>.Value;
            return false;
        }

        private static bool IsGuid(string s, out Guid guid)
        {
            return Guid.TryParse(s, out guid);
        }
        private static bool IsWmiSubject(string s, out WmiSubject subject)
        {
            var k = Enum.GetNames(typeof(WmiSubject));
            for (int i = 0; i < k.Length; i++)
            {
                var c = k[i];
                if(string.Equals(c,s, StringComparison.CurrentCultureIgnoreCase))
                    return Enum.TryParse(s, true, out subject);
            }
            subject = (WmiSubject) (- 1);
            return false;

        }
        private static bool IsIPAddress(string s, out IPAddress address)
        {
            if (s.Contains(".") || s.Contains(":"))
            {
                return IPAddress.TryParse(s, out address);
            }
            address = null;
            return false;
        }

        private static bool IsMacAddress(string s, out PhysicalAddress mac)
        {
            return MacAddressHelper.TryParse(s, out mac);
        }

        internal static bool TryConvertString(object raw, bool isArray, out object result)
        {
            if (isArray)
            {
                if (raw is string[] array)
                {
                    var arrayList = new ArrayList();
                    var guidCount = 0;
                    var wmiSubjectCount = 0;
                    var ipAddressCount = 0;
                    var macAddressCount = 0;
                    var normalCount = 0;
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array[i];

                        if (IsWmiSubject(item, out var subject))
                        {
                            wmiSubjectCount++;
                            arrayList.Add(subject);
                        }
                        else if (IsGuid(item, out var guid))
                        {
                            guidCount++;
                            arrayList.Add(guid);
                        }
                        else if (IsIPAddress(item, out var ip))
                        {
                            ipAddressCount++;
                            arrayList.Add(ip);
                        }
                        else if (IsMacAddress(item, out var mac))
                        {
                            macAddressCount++;
                            arrayList.Add(mac);
                        }
                        else
                        {
                            normalCount++;
                            arrayList.Add(item);
                        }
                    }

                    if (normalCount == array.Length)
                    {
                        result = array;
                    }
                    else if (guidCount == array.Length)
                    {
                        result = arrayList.ToArray(typeof(Guid));
                    }
                    else if (wmiSubjectCount == array.Length)
                    {
                        result = arrayList.ToArray(typeof(WmiSubject));
                    }
                    else if (ipAddressCount == array.Length)
                    {
                        result = arrayList.ToArray(typeof(IPAddress));
                    }
                    else if (macAddressCount == array.Length)
                    {
                        result = arrayList.ToArray(typeof(PhysicalAddress));
                    }
                    else
                    {
                        result = arrayList.ToArray();
                    }

                    return true;
                }
                result = DefaultCache<string>.Array;
                return false;

            }
            if (raw is string value)
            {
                if (IsWmiSubject(value, out var subject))
                    result = subject;
                else if (IsGuid(value, out var guid))
                    result = guid;
                else if (IsIPAddress(value, out var ip))
                    result = ip;
                else if (IsMacAddress(value, out var mac))
                    result = mac;
                else
                    result = value;
                return true;
            }

            result = DefaultCache<string>.Value;
            return false;
        }
    }
}
