// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System;
    using System.Configuration;

    /// <summary>
    /// The config.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// The primary connection id.
        /// </summary>
        public const string PrimaryConnectionId = "SqlConn";

        /// <summary>
        /// The get web config value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="fromRoot">
        /// The from Root.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConfigValue(string key, bool fromRoot = true)
        {
            Configuration rootWebConfig = null;
            if (fromRoot)
            {
                rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            }

            return GetConfigValue(key, rootWebConfig);
        }

        /// <summary>
        /// The get web config value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="rootWebConfig">
        /// The root web config.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConfigValue(string key, Configuration rootWebConfig)
        {
            return rootWebConfig != null && rootWebConfig.AppSettings.Settings[key]!=null ? rootWebConfig.AppSettings.Settings[key].Value : ConfigurationManager.AppSettings[key];
        }
        
        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="fromRoot">
        /// The from root.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConnectionString(string key, bool fromRoot=true)
        {
            Configuration rootWebConfig = null;
            try
            {
                if (fromRoot)
                {
                    rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
                }
            }
            catch(Exception ex)
            {

            }

            return GetConnectionString(key, rootWebConfig);
        }

        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="rootWebConfig">
        /// The root web config.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConnectionString(string key, Configuration rootWebConfig)
        {
            var settings = rootWebConfig != null
                               ? rootWebConfig.ConnectionStrings.ConnectionStrings[key]
                               : ConfigurationManager.ConnectionStrings[key];
            return settings != null ? settings.ConnectionString : "No connection string";
        }
    }
}