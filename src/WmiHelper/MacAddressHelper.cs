
namespace WmiHelper
{
    using System.Net.NetworkInformation;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static class MacAddressHelper
    {
        private readonly static Regex s_pattern1;
        private readonly static Regex s_pattern2;

        static MacAddressHelper()
        {
            var token = Enumerable.Repeat("[0-9A-F]{2}", 5);
            s_pattern1 = new Regex(string.Join("-", token));
            s_pattern2 = new Regex(string.Join(":", token));
        }
        public static bool TryParse(string s, out PhysicalAddress physicalAddress)
        {
            var k = s.ToUpper();
            physicalAddress = null;
            if (s_pattern1.IsMatch(k))
            {
                physicalAddress = PhysicalAddress.Parse(k);
                return true;
            }
            if (s_pattern2.IsMatch(k))
            {
                physicalAddress = PhysicalAddress.Parse(k.Replace(":", null));
                return true;
            }
            return false;
        }
    }
}
