// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemcacheFunctions.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the MemcacheFunctions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Memcached
{
    using System;
    using System.Configuration;
    using System.Text;
    using System.Web;

    using LIB.Tools.Utils;

    using global::Memcached.ClientLibrary;

    /// <summary>
    /// The memory cache functions.
    /// </summary>
    public class MemcacheFunctions
    {
        #region properties
        /// <summary>
        /// Gets the current serialize.
        /// </summary>
        public static string CurrentSerialize
        {
            get
            {
                if (null != (new HttpContextWrapper(HttpContext.Current)).Cache)
                {
                    var obj = CacheManager.GetCachedIISItem("CurrentSerialize");
                    if (null != obj && string.Empty != obj.ToString())
                    {
                        return obj.ToString();
                    }
                }

                var currentSerialize = ConfigurationManager.AppSettings["CurrentSerialize"];

                if (!string.IsNullOrEmpty(currentSerialize))
                {
                    return currentSerialize;
                }

                return MemcachedClient.BINARY_SERIALIZE; // default .net serializer BinaryFormatter()
            }
        }
        #endregion
        
        #region delete cached item

        /// <summary>
        /// The delete cached item.
        /// </summary>
        /// <param name="cacheId">
        /// The p cache id.
        /// </param>
        /// <param name="subKey">
        /// The p sub key.
        /// </param>
        /// <param name="groupNames">
        /// The p group names.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool DeleteCachedItem(string cacheId, string subKey = "", string groupNames = MemcacheGroup.GroupAll)
        {
            GetSockPool();
            var mc = new MemcachedClient();

            string keyForTrace;
            string key = GetCacheKey(cacheId, subKey, groupNames, out keyForTrace);

            return mc.Delete(key);
        }
        #endregion

        #region static methods

        /// <summary>
        /// The get sock pool.
        /// </summary>
        /// <returns>
        /// The <see cref="SockIOPool"/>.
        /// </returns>
        public static SockIOPool GetSockPool()
        {
            var serverList = GetServerList();

            // initialize the pool for memcache servers
            var pool = SockIOPool.GetInstance();
            if (pool.Initialized || 0 == serverList.Length)
            { 
                return pool;
            }

            pool.SetServers(serverList);

            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;

            pool.SocketConnectTimeout = 100;
            pool.SocketTimeout = 300;

            pool.MaintenanceSleep = 30;
            pool.Failover = true;

            pool.Nagle = false;

            try
            {
                pool.Initialize();
            }
            catch (Exception)
            {
            }

            return pool;
        }

        /// <summary>
        /// The memory flush.
        /// </summary>
        public static void MemFlush()
        {
            GetSockPool();
            var mc = new MemcachedClient(MemcacheFunctions.CurrentSerialize) { EnableCompression = false };
            mc.FlushAll();
        }

        /// <summary>
        /// The show memory cache statistics.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ShowMemcacheStatistics()
        {
            GetSockPool();

            var mc = new MemcachedClient(CurrentSerialize) { EnableCompression = false };

            System.Collections.IDictionary stats = mc.Stats();
            if (null == stats)
            {
                return string.Empty;
            }

            var statInfo = new StringBuilder(1000);
            foreach (string key1 in stats.Keys)
            {
                statInfo.Append(key1);
                statInfo.Append("<BR />");
                var values = (System.Collections.Hashtable)stats[key1];
                foreach (string key2 in values.Keys)
                {
                    statInfo.Append(key2 + ":" + values[key2]);
                    statInfo.Append("<BR />");
                }
                statInfo.Append("<BR />");
            }

            return statInfo.ToString();
        }
        
        /// <summary>
        /// The get cached item.
        /// </summary>
        /// <param name="cacheId">
        /// The p cache id.
        /// </param>
        /// <param name="groupNames">
        /// The p group names.
        /// </param>
        /// <param name="subKey">
        /// The p sub key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetCachedItem(string cacheId, string groupNames = MemcacheGroup.GroupAll, string subKey = "")
        {
            GetSockPool();
            var mc = new MemcachedClient(CurrentSerialize);

            string keyForTrace;
            string key = GetCacheKey(cacheId, subKey, groupNames, out keyForTrace);

            var cachedItem = mc.Get(key);

            return cachedItem;
        }
        
        /// <summary>
        /// The set cached item.
        /// </summary>
        /// <param name="cacheId">
        ///     The p cache id.
        /// </param>
        /// <param name="value">
        ///     The p value.
        /// </param>
        /// <param name="groupNames">
        ///     The p group names.
        /// </param>
        /// <param name="subKey">
        ///     The p sub key.
        /// </param>
        public static void SetCachedItem(string cacheId, object value, string groupNames = MemcacheGroup.GroupAll, string subKey = "")
        {
            SetCachedItem(cacheId, subKey, value, groupNames, DateTime.Now.AddHours(24));
        }

        /// <summary>
        /// Caches an item in Memory cache
        /// </summary>
        /// <param name="cacheId">
        /// stringId that identifies the current item.
        /// </param>
        /// <param name="subKey">
        /// stringId that identifies the group of the current item.
        /// </param>
        /// <param name="value">
        /// value to cache.
        /// </param>
        /// <param name="groupNames">
        /// comma separated list of parent groups.
        /// </param>
        /// <param name="expiry">
        /// cache expiry date, local time.
        /// </param>
        public static void SetCachedItem(
            string cacheId,
            string subKey,
            object value,
            string groupNames,
            DateTime expiry)
        {
            GetSockPool();
            var mc = new MemcachedClient(CurrentSerialize);

            string keyForTrace;
            string key = GetCacheKey(cacheId, subKey, groupNames, out keyForTrace);
            key = key.Replace(' ', '_'); 

            mc.Set(key, value, expiry);
        }

        /// <summary>
        /// The get cache key.
        /// </summary>
        /// <param name="cacheId">
        /// The p cache id.
        /// </param>
        /// <param name="subKey">
        /// The p sub key.
        /// </param>
        /// <param name="groupNames">
        /// The p group names.
        /// </param>
        /// <param name="keyForTrace">
        /// The p key for trace.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCacheKey(string cacheId, string subKey, string groupNames, out string keyForTrace)
        {
            var groupNamesWorker = groupNames.Split(",".ToCharArray());
            var stringkey = new StringBuilder(500);

            stringkey.Append(cacheId + "_" + MemcacheGroup.GetVersion(cacheId) + "_" + subKey);
            foreach (var t in groupNamesWorker)
            {
                stringkey.Append("_" + t + "_" + MemcacheGroup.GetVersion(t));
            }

            var key = stringkey.ToString().Replace(' ', '_');
            keyForTrace = key;

            // result in MD5 Hash
            if (key.Length > 250)
            {
                key = General.GetMd5Hash(key);
                keyForTrace = "(Converted to MD5) " + keyForTrace;
            }

            return key;
        }

        /// <summary>
        /// The change server list.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="newList">
        /// The new list.
        /// </param>
        public static void ChangeServerList(System.Web.UI.Page page, string newList)
        {
            var pool = SockIOPool.GetInstance();
            if (pool.Initialized)
            {
                pool.Shutdown();
            }

            // if no exception - then we have valid list
            ParseServerList(newList);

            if (null != page)
            {
                CacheManager.SetCachedIISItem("MemcachedServers", newList, page.Cache);
            }
        }

        /// <summary>
        /// The get server list.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public static string[] GetServerList()
        {
            // get server list from 
            // a) IIS cache
            // b) web.config

            string serverList = (string)CacheManager.GetCachedIISItem("MemcachedServers");
          
            if (string.IsNullOrEmpty(serverList))
            {
                serverList = ConfigurationManager.AppSettings["MemcachedServers"] ?? string.Empty;
                CacheManager.SetCachedIISItem("MemcachedServers", serverList);
            }

            // if we are here, we have memcached servers. Parse
            return ParseServerList(serverList);
        }

        /// <summary>
        /// The parse server list.
        /// </summary>
        /// <param name="serverList">
        /// The server list.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] ParseServerList(string serverList)
        {
            var arrServers = serverList.Split(",".ToCharArray());
            var allServers = new System.Collections.ArrayList(10);
            foreach (var srv in arrServers)
            {
                var tmp = srv.Split(":".ToCharArray());
                try
                {
                    System.Net.IPAddress.Parse(tmp[0]);
                }
                catch
                {
                    continue;
                }

                // if we are here - we have valid server address
                allServers.Add(srv);
            }

            arrServers = new string[allServers.Count];
            for (var i = 0; i < arrServers.Length; i++)
            {
                arrServers[i] = (string)allServers[i];
            }

            return arrServers;
        }

        #endregion
    }
}