// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemcacheGroup.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the MemcacheGroup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Memcached
{
    using System;

    using LIB.Tools.Utils;

    using global::Memcached.ClientLibrary;

    /// <summary>
    /// The memory cache group.
    /// </summary>
    public class MemcacheGroup
    {
        /// <summary>
        /// The group all.
        /// </summary>
        public const string GroupAll = "ALL";

        /// <summary>
        /// The group translations.
        /// </summary>
        public const string GroupTranslations = "TRANSLATIONS";

        /// <summary>
        /// The get version.
        /// </summary>
        /// <param name="groupName">
        /// The p group name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetVersion(string groupName)
        {
            if (null == groupName)
            {
                return string.Empty;
            }
            groupName = groupName.Replace(' ', '_');

            if (groupName.Length > 250)
            {
                groupName = General.GetMd5Hash(groupName);
            }

            var mc = new MemcachedClient(MemcacheFunctions.CurrentSerialize) { EnableCompression = false };
            var str = (string)mc.Get(groupName);
            if (string.IsNullOrEmpty(str))
            {
                str = Guid.NewGuid().ToString();
                mc.Set(groupName, str, DateTime.Now.AddHours(24));
            }

            return str;
        }


        /// <summary>
        /// The change versions.
        /// </summary>
        /// <param name="groupNames">
        /// The p group names.
        /// </param>
        public static void ChangeVersions(string groupNames)
        {
            MemcacheFunctions.GetSockPool();
            var mc = new MemcachedClient(MemcacheFunctions.CurrentSerialize) { EnableCompression = false };

            var groupNamesWorker = groupNames.Split(",".ToCharArray());
            for (var i = 0; i < groupNamesWorker.Length; i++)
            {
                var str = Guid.NewGuid().ToString();
                if (groupNamesWorker[i].Length > 250)
                {
                    groupNamesWorker[i] = General.GetMd5Hash(groupNamesWorker[i]);
                }

                mc.Set(groupNamesWorker[i], str, DateTime.Now.AddHours(24));
            }
        }
    }
}