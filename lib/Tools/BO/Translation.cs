// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translation.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The translation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.BO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using LIB.BusinessObjects;
    using LIB.Tools.Utils;

    /// <summary>
    /// The translation.
    /// </summary>
    [Serializable]
    public class Translation : ItemBase
    {

        #region Languages Properties

        /// <summary>
        /// The supported languages.
        /// </summary>
        private static Dictionary<long, ItemBase> supportedLanguages = null;

        /// <summary>
        /// Gets the supported languages.
        /// </summary>
        public static Dictionary<long, ItemBase> SupportedLanguages
        {
            get
            {
                if (supportedLanguages == null)
                {
                    try
                    {
                        supportedLanguages = (new Language()).Populate();
                    }
                    catch (Exception ex)
                    {
                        // supportedLanguages is static so it can be that just after check supportedLanguages - supportedLanguages.ContainsKey - key already exists
                        General.TraceWarn(ex.ToString());
                    }
                }

                return supportedLanguages;
            }
        }

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        public static Dictionary<long, ItemBase> AvailableLanguages
        {
            get
            {
                return SupportedLanguages != null ? supportedLanguages.Values.Where(item => ((Language)item).ShortName != System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToDictionary(item => item.Id) : null;
            }
        }

        #endregion

        #region Translations Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        #endregion

        /// <summary>
        /// The get translations_to_cache.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>     .
        /// </returns>
        public static Dictionary<string, Dictionary<string, Translation>> GetTranslationsToCache(SqlConnection conn)
        {
            const string StrSql = "Translation_Populate";

            var sqlCommand = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var translations = new Dictionary<string,Dictionary<string,Translation>>();

            using (var rdr = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var translationItem = (Translation)(new Translation()).FromDataRow(rdr);
                    if (translations.ContainsKey(translationItem.Alias))
                    {
                        if (translations[translationItem.Alias].ContainsKey(translationItem.Language))
                        {
                            translations[translationItem.Alias][translationItem.Language] = translationItem;
                        }
                        else
                        {
                            translations[translationItem.Alias].Add(translationItem.Language, translationItem);
                        }
                    }
                    else
                    {
                        var lng = new Dictionary<string, Translation> { { translationItem.Language, translationItem } };
                        translations.Add(translationItem.Alias, lng);
                    }
                }

                rdr.Close();
            }

           // CacheManager.SetCachedItem(CacheKeyConstants.Translations, translations, MemcacheGroup.GroupTranslations);

            return translations;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="translationItem">
        /// The translation item.
        /// </param>
        public static void Update(SqlConnection conn, Translation translationItem)
        {
            if (string.IsNullOrEmpty(translationItem.Alias))
            {
                return;
            }

            const string StrSql = "Translation_Update";

            var cmd = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@Alias", SqlDbType.NVarChar, 50) { Value = translationItem.Alias };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@Language", SqlDbType.NVarChar, 10) { Value = translationItem.Language };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@Text", SqlDbType.NVarChar, -1) { Value = translationItem.Text };
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();

            Dictionary<string, Dictionary<string, Translation>> translations;

            object translationArray = null;// CacheManager.GetCachedItem(CacheKeyConstants.Translations, MemcacheGroup.GroupTranslations) as Dictionary<string, Dictionary<string, Translation>>;

            if (null == translationArray)
            {
                translations = new Dictionary<string, Dictionary<string, Translation>>();
                var lng = new Dictionary<string, Translation> { { translationItem.Language, translationItem } };
                translations.Add(translationItem.Alias, lng);
            }
            else
            {
                translations = (Dictionary<string, Dictionary<string, Translation>>)translationArray;
                if (translations.ContainsKey(translationItem.Alias))
                {
                    if (translations[translationItem.Alias].ContainsKey(translationItem.Language))
                    {
                        translations[translationItem.Alias][translationItem.Language] = translationItem;
                    }
                    else
                    {
                        translations[translationItem.Alias].Add(translationItem.Language, translationItem);
                    }
                }
                else
                {
                    var lng = new Dictionary<string, Translation> { { translationItem.Language, translationItem } };
                    translations.Add(translationItem.Alias, lng);
                }
            }

           // CacheManager.SetCachedItem(CacheKeyConstants.Translations, translations, MemcacheGroup.GroupTranslations);

            Translate.StaticTranslations = translations;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        public static void Delete(SqlConnection conn)
        {
            const string StrSql = "Translation_Delete";

            var cmd = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            cmd.ExecuteNonQuery();

            var translations = new Dictionary<string,Dictionary<string,Translation>>();
            
           // CacheManager.SetCachedItem(CacheKeyConstants.Translations, translations, MemcacheGroup.GroupTranslations);

            Translate.StaticTranslations = translations;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="translationArray">
        /// The translation array.
        /// </param>
        public static void Update(SqlConnection conn, Translation[] translationArray)
        {
            foreach (var tr in translationArray)
            {
                Update(conn, tr);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="translationItem">
        /// The translation item.
        /// </param>
        /// <param name="pathToResource">
        /// The path to resource.
        /// </param>
        public static void Update(Translation translationItem, string pathToResource)
        {
            var resFile = new ResXUnified(pathToResource);

            resFile[translationItem.Language][translationItem.Alias] = translationItem.Text;

            resFile.Save();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="translationArray">
        /// The translation array.
        /// </param>
        /// <param name="pathToResource">
        /// The path to resource.
        /// </param>
        public static void Update(Translation[] translationArray, string pathToResource)
        {
            var resFile = new ResXUnified(pathToResource);

            foreach (Translation tr in translationArray)
            {
                resFile[tr.Language][tr.Alias] = tr.Text;
            }

            resFile.Save();
        }
    }
}