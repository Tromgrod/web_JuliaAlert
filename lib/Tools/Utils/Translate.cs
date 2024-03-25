// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translate.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Translate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Web;

    using LIB.Tools.BO;
    using LIB.Tools.Localization;
    using Translation = LIB.Tools.BO.Translation;

    /// <summary>
    /// The translate.
    /// </summary>
    public class Translate
    {
        /// <summary>
        /// The words.
        /// </summary>
        private static Dictionary<string, string> words;

        /// <summary>
        /// Gets or sets the static translations.
        /// </summary>
        public static Dictionary<string, Dictionary<string, Translation>> StaticTranslations { get; set; }

        /// <summary>
        /// The new translation.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        public static void NewTranslation(SqlConnection conn, string alias, string text, string language)
        {
            var translationItem = new Translation { Alias = alias, Text = text, Language = language };
            Translation.Update(conn, translationItem);
        }

        /// <summary>
        /// The new translation.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="pathToResource">
        /// The path to resource.
        /// </param>
        public static void NewTranslation(string alias, string text, string language, string pathToResource)
        {
            var resFile = new ResXUnified(pathToResource);
            
            resFile["Default"][alias] = text;

            resFile[language][alias] = text;

            resFile.Save();
        }

        /// <summary>
        /// The get translations from data base.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, Dictionary<string, Translation>> GetTranslationsFromDb()
        {
            General.TraceWarn("GetTransaltionsFromDB called");
            var conn = DataBase.ConnectionFromContext(); ;
            var translations = Translation.GetTranslationsToCache(conn);
            return translations;
        }

        /// <summary>
        /// The get translated value from data base.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTranslatedValueFromDb(string alias)
        {
            return GetTranslatedValueFromDb(alias, alias);
        }

        /// <summary>
        /// The get translated value from data base.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="defaultvalue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTranslatedValueFromDb(string alias, string defaultvalue)
        {
            var translated = string.Empty;

            var translations = GetTranslationsArray();

            /*if (translations.ContainsKey(alias))
            {
                if (translations[alias].ContainsKey(System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName))
                {
                    translated = translations[alias][System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName].Text;
                }
            }*/

            if (string.IsNullOrEmpty(translated))
            {
                translated = string.Empty; 
            }

            return !string.IsNullOrEmpty(translated) ? translated : defaultvalue;
        }
        /// <summary>
        /// The get translated value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTranslatedValue(string alias, string ResourceFile)
        {
            return GetTranslatedValue(alias, ResourceFile, alias);
        }

        public static Dictionary<string, ResourceData> Resources = new Dictionary<string, ResourceData>();
        /// <summary>
        /// The get translated value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="defaultvalue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTranslatedValue(string alias, string ResourceFile, string defaultvalue)
        {
            string translated = string.Empty;

            //try
            {
                ResourceData data = null;
                if (!Resources.ContainsKey(ResourceFile/* + Thread.CurrentThread.CurrentUICulture.Name*/))
                {
                    var TranslationsAssembly = Assembly.Load("Translations");
                    data = new ResourceData(new ResourceManager("Translations." + ResourceFile, TranslationsAssembly), "Translations");
                    if (!Resources.ContainsKey(ResourceFile/* + Thread.CurrentThread.CurrentUICulture.Name*/))
                        Resources.Add(ResourceFile/* + Thread.CurrentThread.CurrentUICulture.Name*/, data);
                }

                if (data == null)
                    data = Resources[ResourceFile/* + Thread.CurrentThread.CurrentUICulture.Name*/];

                translated = data.GetValue(alias);
                /*  Assembly initialAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "Translations");

                  ResourceManager ResManager = new ResourceManager(ResourceFile, initialAssembly);

                  translated = ResManager.GetString(alias);*/

                if (string.IsNullOrEmpty(translated))
                {
                    translated = string.Empty;
                }
            }
            /*catch
            {
            }*/

            return !string.IsNullOrEmpty(translated) ? translated : defaultvalue;
        }

        /// <summary>
        /// The get translated value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="ci">
        /// The ci.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTranslatedValue(string alias, CultureInfo ci)
        {
            var translated = string.Empty;

            var translations = GetTranslationsArray();

            if (translations.ContainsKey(alias))
            {
                if (translations[alias].ContainsKey(ci.TwoLetterISOLanguageName))
                {
                    translated = translations[alias][ci.TwoLetterISOLanguageName].Text;
                }
            }

            if (string.IsNullOrEmpty(translated))
            {
                translated = string.Empty; // Resources.DB.ResourceManager.GetString(alias, ci);
            }

            return translated;
        }


        /// <summary>
        /// The modify string to alias.
        /// </summary>
        /// <param name="txt">
        /// The txt.
        /// </param>
        /// <param name="tablename">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ModifyStringToAlias(string txt, string tablename)
        {
            return tablename + "_" + Translit(txt, 10);
        }

        /// <summary>
        /// The modify string to alias.
        /// </summary>
        /// <param name="txt">
        /// The txt.
        /// </param>
        /// <param name="tablename">
        /// The table name.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ModifyStringToAlias(string txt, string tablename, long id)
        {
            return tablename + "_" + Translit(txt, 10) + "_" + id.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The transliteral.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Translit(string value, int count)
        {
            if (words == null)
            {
                GenerateWords();
            }

            if (value.Length > count)
            {
                value = value.Substring(0, count);
            }

            value = words.Aggregate(value, (current, pair) => current.Replace(pair.Key, pair.Value));

            var result = value.Where((t, i) => char.IsLetter(value, i) || IsWord(t)).Aggregate(string.Empty, (current, t) => current + t);

            return new HttpContextWrapper(HttpContext.Current).Server.UrlEncode(result);
        }

        /// <summary>
        /// The get translations array.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        private static Dictionary<string, Dictionary<string, Translation>> GetTranslationsArray()
        {
            Dictionary<string, Dictionary<string, Translation>> translations;

            if (StaticTranslations != null)
            {
                translations = StaticTranslations;
            }
            else
            {
                object translationArray = null;// CacheManager.GetCachedItem(CacheKeyConstants.Translations, MemcacheGroup.GroupTranslations) as Dictionary<string, Dictionary<string, Translation>>;

                //translations = translationArray ?? GetTranslationsFromDb();
                translations = null;

                StaticTranslations = translations;
            }

            return translations;
        }

        /// <summary>
        /// The is word.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWord(char character)
        {
            return words.Values.Any(val => val == character.ToString());
        }

        /// <summary>
        /// The generate words.
        /// </summary>
        public static void GenerateWords()
        {
            words = new Dictionary<string, string>
                        {
                            { "à", "a" },
                            { "á", "b" },
                            { "â", "v" },
                            { "ã", "g" },
                            { "ä", "d" },
                            { "å", "e" },
                            { "¸", "yo" },
                            { "æ", "zh" },
                            { "ç", "z" },
                            { "è", "i" },
                            { "é", "j" },
                            { "ê", "k" },
                            { "ë", "l" },
                            { "ì", "m" },
                            { "í", "n" },
                            { "î", "o" },
                            { "ï", "p" },
                            { "ð", "r" },
                            { "ñ", "s" },
                            { "ò", "t" },
                            { "ó", "u" },
                            { "ô", "f" },
                            { "õ", "h" },
                            { "ö", "c" },
                            { "÷", "ch" },
                            { "ø", "sh" },
                            { "ù", "sch" },
                            { "ú", "j" },
                            { "û", "i" },
                            { "ü", "j" },
                            { "ý", "e" },
                            { "þ", "yu" },
                            { "ÿ", "ya" },
                            { "À", "A" },
                            { "Á", "B" },
                            { "Â", "V" },
                            { "Ã", "G" },
                            { "Ä", "D" },
                            { "Å", "E" },
                            { "¨", "Yo" },
                            { "Æ", "Zh" },
                            { "Ç", "Z" },
                            { "È", "I" },
                            { "É", "J" },
                            { "Ê", "K" },
                            { "Ë", "L" },
                            { "Ì", "M" },
                            { "Í", "N" },
                            { "Î", "O" },
                            { "Ï", "P" },
                            { "Ð", "R" },
                            { "Ñ", "S" },
                            { "Ò", "T" },
                            { "Ó", "U" },
                            { "Ô", "F" },
                            { "Õ", "H" },
                            { "Ö", "C" },
                            { "×", "Ch" },
                            { "Ø", "Sh" },
                            { "Ù", "Sch" },
                            { "Ú", "J" },
                            { "Û", "I" },
                            { "Ü", "J" },
                            { "Ý", "E" },
                            { "Þ", "Yu" },
                            { "ß", "Ya" },
                            { " ", "_" },
                            { "-", "_" },
                            { "/", "_" },
                            { ".", "_" },
                            { ",", "_" }
                        };
        }
    }
}