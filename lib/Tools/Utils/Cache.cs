// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cache.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Cache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System.Collections;
    using System.Linq;
    using System.Web;
    using System.Web.Caching;
    
    using System;

    /// <summary>
    /// The cache.
    /// </summary>
    public class CacheManager
    {
        #region Memcache
        
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
       /* public static object GetCachedItem(string cacheId, string groupNames = MemcacheGroup.GroupAll, string subKey = "")
        {
            return MemcacheFunctions.GetCachedItem(cacheId, groupNames, subKey);
        }*/
        
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
      /*  public static void SetCachedItem(string cacheId, object value, string groupNames = MemcacheGroup.GroupAll, string subKey = "")
        {
            MemcacheFunctions.SetCachedItem(cacheId, value, groupNames, subKey);
        }*/

        #endregion

        #region IIS cache
        /// <summary>
        /// The set cached item.
        /// </summary>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="item">
        /// The o item.
        /// </param>
        /// <param name="cache">
        /// The o cache.
        /// </param>
        /// <param name="cachePrefix">
        /// The cache prefix.
        /// </param>
        public static void SetCachedIISItem(
            string cacheId, object item, Cache cache = null, string cachePrefix = "")
        {
            if (null == cache)
            {
                if (null == new HttpContextWrapper(HttpContext.Current) || null == new HttpContextWrapper(HttpContext.Current).Cache)
                {
                    return;
                }

                cache = new HttpContextWrapper(HttpContext.Current).Cache;
            }

            var cacheItem = cachePrefix + cacheId;
            if (cache[cacheItem] != null)
                cache[cacheItem] = item;
            else
            {
                cache.Add(cacheItem, item, null, System.DateTime.Now.AddHours(23), Cache.NoSlidingExpiration, CacheItemPriority.Normal, new CacheItemRemovedCallback(ReportRemovedCallback));
            }
        }

        public static void ReportRemovedCallback(String key, object value,
            CacheItemRemovedReason removedReason)
        {
        }

        /// <summary>
        /// The get cached item.
        /// </summary>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="cache">
        /// The o cache.
        /// </param>
        /// <param name="cachePrefix">
        /// The cache prefix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetCachedIISItem(string cacheId, Cache cache = null, string cachePrefix = "")
        {
            if (null == cache)
            {
                if (null == new HttpContextWrapper(HttpContext.Current) || null == new HttpContextWrapper(HttpContext.Current).Cache)
                {
                    return null;
                }

                cache = new HttpContextWrapper(HttpContext.Current).Cache;
            }

            var cacheItem = cachePrefix + cacheId;
            var cachedItem = cache[cacheItem];
            return cachedItem;
        }

        /// <summary>
        /// The remove cached item.
        /// </summary>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="cachePrefix">
        /// The cache prefix.
        /// </param>
        /// <param name="cache">
        /// The o cache.
        /// </param>
        public static void RemoveCachedIISItem(string cacheId, string cachePrefix = "", Cache cache = null)
        {
            if (null == cache)
            {
                if (null == new HttpContextWrapper(HttpContext.Current) || null == new HttpContextWrapper(HttpContext.Current).Cache)
                {
                    return;
                }

                cache = new HttpContextWrapper(HttpContext.Current).Cache;
            }

            var cacheItem = cachePrefix + cacheId;
            cache.Remove(cacheItem);
        }

        /// <summary>
        /// The remove cached items by partial key.
        /// </summary>
        /// <param name="partialKey">
        /// The p partial key.
        /// </param>
        /// <param name="cache">
        /// The p cache.
        /// </param>
        public static void RemoveCachedIISItemsByPartialKey(string partialKey, Cache cache = null)
        {
            if (null == cache)
            {
                if (null == new HttpContextWrapper(HttpContext.Current) || null == new HttpContextWrapper(HttpContext.Current).Cache)
                {
                    return;
                }

                cache = new HttpContextWrapper(HttpContext.Current).Cache;
            }

            bool toBreak;
            do
            {
                toBreak = true;
                foreach (var objItem in cache.Cast<DictionaryEntry>().Where(objItem => -1 != ((string)objItem.Key).ToLower().IndexOf(partialKey, System.StringComparison.Ordinal)))
                {
                    cache.Remove((string)objItem.Key);
                    toBreak = false;
                    break;
                }
            }
            while (!toBreak);
        }
        #endregion
    }
}