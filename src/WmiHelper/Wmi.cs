
namespace WmiHelper
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Management;
    using System.Diagnostics;
    using System.Threading;
    using WmiHelper.Internal;

    /// <summary>
    /// Gets the system info, see https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/computer-system-hardware-classes
    /// </summary>
    public static class Wmi
    {
        private static bool TryConvert(CimType cimType, bool isArray, object value, out object result)
        {
            var flag = false;
            result = null;
            switch (cimType)
            {
                case CimType.SInt8:
                    flag = CimTypeConverter.TryConvert<sbyte>(value, isArray, out result);
                    break;
                case CimType.UInt8:
                    flag = CimTypeConverter.TryConvert<byte>(value, isArray, out result);
                    break;
                case CimType.Reference:
                case CimType.SInt16:
                    flag = CimTypeConverter.TryConvert<short>(value, isArray, out result);
                    break;
                case CimType.UInt16:
                    flag = CimTypeConverter.TryConvert<ushort>(value, isArray, out result);
                    break;
                case CimType.SInt32:
                    flag = CimTypeConverter.TryConvert<int>(value, isArray, out result);
                    break;
                case CimType.UInt32:
                    flag = CimTypeConverter.TryConvert<uint>(value, isArray, out result);
                    break;
                case CimType.SInt64:
                    flag = CimTypeConverter.TryConvert<long>(value, isArray, out result);
                    break;
                case CimType.UInt64:
                    flag = CimTypeConverter.TryConvert<ulong>(value, isArray, out result);
                    break;
                case CimType.Real32:
                    flag = CimTypeConverter.TryConvert<float>(value, isArray, out result);
                    break;
                case CimType.Real64:
                    flag = CimTypeConverter.TryConvert<double>(value, isArray, out result);
                    break;
                case CimType.Boolean:
                    flag = CimTypeConverter.TryConvert<bool>(value, isArray, out result);
                    break;
                case CimType.Char16:
                    flag = CimTypeConverter.TryConvert<char>(value, isArray, out result);
                    break;

                // -----
                case CimType.None:
                case CimType.Object:
                    flag = CimTypeConverter.TryConvert<object>(value, isArray, out result);
                    break;

                // -----
                case CimType.String:
                    flag = CimTypeConverter.TryConvertString(value, isArray, out result);
                    break;
                case CimType.DateTime:
                    flag = CimTypeConverter.TryConvertDateTime(value, isArray, out result);
                    break;
            }

            return flag;
        }
        
        public static async Task<IReadOnlyList<T>> GetAsync<T>(WmiSubject subject, ITranspiler<T> transpiler, bool localOnly = true, CancellationToken token = default(CancellationToken))
        {
            return await Task.Run(() => Get(subject, transpiler, localOnly), token);
        }

        /// <summary>
        /// Gets the Windows Management Instrumentation (WMI) objects by subject.
        /// </summary>0
        /// <typeparam name="T">Type of callback.</typeparam>
        /// <param name="subject">The subject which you want to get.</param>
        /// <param name="transpiler">Uses the <seealso cref="Transpiler.Dynamic"/> or <seealso cref="Transpiler.ReadOnlyDictionary"/> to get the value.</param>
        /// <param name="localOnly">finds only local fields.</param>
        /// <returns></returns>
        public static IReadOnlyList<T> Get<T>(WmiSubject subject, ITranspiler<T> transpiler, bool localOnly = true)
        {
            var collection = new ManagementObjectSearcher($"select * from {subject}").Get();
#if DEBUG
            Debug.WriteLine("# " + subject);
            var fmt = WmiDebugHelper.Formatter(collection.Count);
            var i = 0;
#endif
            var list = new List<T>(collection.Count);
            foreach (ManagementObject mo in collection)
            {
#if DEBUG
                i++;
#endif
                var dict = transpiler.Create();
                try
                {
                    foreach (var pd in mo.Properties)
                    {
                        if ((localOnly && pd.IsLocal) || !localOnly)
                        {
                            var name = pd.Name;
                            if (!TryConvert(pd.Type, pd.IsArray, pd.Value, out var value))
                                continue;
                            if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                                continue;
#if DEBUG
                            Debug.WriteLine($"[({value.GetType().Name}){fmt(i)}{name}]: { WmiDebugHelper.DebugString(value) } ");
#endif
                            dict.Add(name, value);
                        }

                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.WriteLine(e);
#endif
                    continue;
                }
                list.Add(transpiler.Convert(dict));
            }

            return list;
        }


    }
}
