using LIB.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Cache
{
    public class CacheKeyProcessor
    {
        public static string BuildKey(List<string> keys)
        {
            string key = "";
            foreach (var subkey in keys)
            {
                var subkeyval = CacheProcessor.GetCachedIISItem(subkey);
                if (subkeyval != null && !string.IsNullOrEmpty((string)subkeyval))
                {
                    key += "_" + CacheProcessor.GetCachedIISItem(subkey).ToString();
                }
                else
                {
                    var sval = subkey + "_" + Guid.NewGuid().ToString();
                    CacheProcessor.SetCachedIISItem(subkey, sval);
                    key += "_" + sval;
                }
            }
            return key;
        }
        public static void ResetCahceForKey(string key)
        {
            var sval = key + "_" + Guid.NewGuid().ToString();
            CacheProcessor.SetCachedIISItem(key, sval);
        }

        public static void ResetCahceForKey(List<string> keys)
        {
            foreach (var subkey in keys)
            {
                var sval = subkey + "_" + Guid.NewGuid().ToString();
                CacheProcessor.SetCachedIISItem(subkey, sval);
            }
        }
    }
}
