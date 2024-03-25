using System;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Collections;

namespace LIB.BusinessObjects
{
    class CacheItem : ItemBase
    {
        #region Constructors
        public CacheItem()
            : base(0) { }

        public CacheItem(long id)
            : base(id) { }
        #endregion

        public override Dictionary<long, ItemBase> Populate(ItemBase item = null,
                                                        SqlConnection conn = null,
                                                        bool sortByName = false,
                                                        string AdvancedFilter = "",
                                                        bool ShowCanceled = false,
                                                        User sUser = null,
                                                        bool ignoreQueryFilter = false)
        => Populate(null);

        public override Dictionary<long, ItemBase> Populate(List<SortParameter> SortParameters)
        {
            var cache= new HttpContextWrapper(HttpContext.Current).Cache;

            var ChacheItems = new Dictionary<long, ItemBase>();
            var ind = 0;

            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            List<CacheItem> list = new List<CacheItem>();
            while (enumerator.MoveNext())
            {
                string key = (string)enumerator.Key;
                string value = enumerator.Value.ToString();
                var CacheItem = new CacheItem(ind)
                {
                    Key = key,
                    Value = value.Length > 200 ? value.Substring(0, 200) : value
                };
                CacheItem.DateExpire = GetCacheUtcExpiryDateTime(key, cache);
                CacheItem.DateCreated = CacheItem.DateExpire.AddHours(-1);
                CacheItem.CreatedBy = new User() { Login = "IIS Cache" };
                if (key.IndexOf("System.Web.Optimization.Bundle") == -1
                    && key.IndexOf("__AppStartPage__") == -1
                    && key.IndexOf(":ViewCacheEntry:WebLib.Themes.ThemeableRazorViewEngine") == -1)
                {
                    list.Add(CacheItem);
                    ind++;
                }
            }
            if (SortParameters!=null){
                var sort = SortParameters.First();
                if(sort.Field== "Key")
                {
                    if (sort.Direction == "asc")
                        list.Sort((x, y) => x.Key.CompareTo(y.Key));
                    else
                        list.Sort((x, y) => y.Key.CompareTo(x.Key));
                }
                if (sort.Field == "Value")
                {
                    if (sort.Direction == "asc")
                        list.Sort((x, y) => x.Value.CompareTo(y.Value));
                    else
                        list.Sort((x, y) => y.Value.CompareTo(x.Value));
                }
                if (sort.Field == "DateExpire")
                {
                    if (sort.Direction == "asc")
                        list.Sort((x, y) => x.DateExpire.CompareTo(y.DateExpire));
                    else
                        list.Sort((x, y) => y.DateExpire.CompareTo(x.DateExpire));
                }
                if (sort.Field == "DateCreated")
                {
                    if (sort.Direction == "asc")
                        list.Sort((x, y) => x.DateCreated.CompareTo(y.DateCreated));
                    else
                        list.Sort((x, y) => y.DateCreated.CompareTo(x.DateCreated));
                }
                if (sort.Field == "CreatedBy")
                {
                    if (sort.Direction == "asc")
                        list.Sort((x, y) => x.CreatedBy.Login.CompareTo(y.CreatedBy.Login));
                    else
                        list.Sort((x, y) => y.CreatedBy.Login.CompareTo(x.CreatedBy.Login));
                }
            }
            foreach (var lCacheItem in list)
            {
                ChacheItems.Add(lCacheItem.Id, lCacheItem);
            }
            return ChacheItems;
        }
        public bool Delete(Dictionary<long, ItemBase> dictionary, out string Reason, string Comment = "", SqlConnection connection = null, User user = null)
        {
            Reason = "";
            var cache = new HttpContextWrapper(HttpContext.Current).Cache;
            foreach (var item in dictionary.Values)
            {
                var cacheitem = (CacheItem)item;
                cache.Remove(cacheitem.Key);
            }
            return true;
        }
        private DateTime GetCacheUtcExpiryDateTime(string cacheKey, System.Web.Caching.Cache cache)
        {
            if (cache != null && cache.GetType() != null)
            {
                var cacheGet = cache.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic);
                if (cacheGet != null)
                {
                    var cacheEntry = cache.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(cache, new object[] { cacheKey, 1 });
                    PropertyInfo utcExpiresProperty = cacheEntry.GetType().GetProperty("UtcExpires", BindingFlags.NonPublic | BindingFlags.Instance);
                    DateTime utcExpiresValue = (DateTime)utcExpiresProperty.GetValue(cacheEntry, null);

                    return utcExpiresValue;
                }
            }
            return DateTime.Now;
        }

        public override string GetName()
        {
            return Key;
        }

        public override Object GetId()
        {
            return Key;
        }

        public override void SetId(object Id)
        {
            Key = Id.ToString();
        }

        #region Properties
        [Template(Mode = Template.Name)]
        public string Key { get; set; }

        [Template(Mode = Template.Name)]
        public string Value { get; set; }
        
        [Template(Mode = Template.DateTime)]
        public DateTime DateExpire { get; set; }
        #endregion
    }
}