
namespace Orlys.WmiHelper
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Management;
    using System.Threading;
    using Orlys.WmiHelper.Internal;

    /// <summary>
    /// Gets the system info, see https://docs.microsoft.com/en-us/windows/desktop/cimwin32prov/computer-system-hardware-classes
    /// </summary>
    public static class Wmi
    {
        public static async Task<IReadOnlyList<T>> GetAsync<T>(WmiSubject subject, ITranspiler<T> transpiler, bool localOnly = true, TimeSpan? timeout = null, CancellationToken token = default(CancellationToken))
        {
            return await Task.Factory.StartNew(() => Get(subject, transpiler, localOnly, timeout), token, TaskCreationOptions.LongRunning, Task.Factory.Scheduler);
        }

        /// <summary>
        /// Gets the Windows Management Instrumentation (WMI) objects by subject.
        /// </summary>0
        /// <typeparam name="T">Type of callback.</typeparam>
        /// <param name="subject">The subject which you want to get.</param>
        /// <param name="transpiler">Uses the <seealso cref="Transpiler.Dynamic"/> or <seealso cref="Transpiler.ReadOnlyDictionary"/> to get the value.</param>
        /// <param name="localOnly">finds only local fields.</param>
        /// <returns></returns>
        public static IReadOnlyList<T> Get<T>(WmiSubject subject, ITranspiler<T> transpiler, bool localOnly = true, TimeSpan? timeout = null)
        {
            var searcher = new ManagementObjectSearcher($"select * from {subject}");
            if (timeout is TimeSpan ts)
                searcher.Options.Timeout = ts;

            var collection = default(ManagementObjectCollection);
            try
            {
                collection = searcher.Get(); 
                var list = new List<T>(collection.Count);

                foreach (ManagementObject mo in collection)
                { 
                    var dict = transpiler.Create();
                    try
                    {
                        foreach (var pd in mo.Properties)
                        {
                            if ((localOnly && pd.IsLocal) || !localOnly)
                            {
                                var name = pd.Name;
                                if (!CimTypeConverter.TryConvert(pd.Type, pd.IsArray, pd.Value, out var value))
                                    continue;
                                // todo: makes filter optional.
                                if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                                {
                                    continue;
                                }
                                dict.Add(name, value);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    list.Add(transpiler.Convert(dict));
                }
                return list;

            }
            catch
            {
                return DefaultCache<T>.Array;
            }
            finally
            {
                (collection as IDisposable)?.Dispose();
            }
        } 
    }
}
