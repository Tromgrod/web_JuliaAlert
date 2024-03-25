// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResXUnified.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the ResXUnified type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Resources;

    /// <summary>
    /// Represent a unified dictionnary of a bunch of ResX files.  For example
    /// Default.aspx.resx, Default.aspx.fr-CA.resx will be merged in an easy
    /// to access in memory dictionnary.  You can then change the values in
    /// memory and then save the changes back to disk later.
    /// </summary>
    [Serializable]
    public class ResXUnified
    {
        /// <summary>
        /// Files that changed since the last save
        /// </summary>
        private readonly List<string> changed = new List<string>();

        /// <summary>
        /// The base file name.
        /// </summary>
        private readonly string baseFileName;

        /// <summary>
        /// The base path.
        /// </summary>
        private readonly string basePath;

        /// <summary>
        /// The lck.
        /// </summary>
        private readonly object lck = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResXUnified"/> class. 
        /// Provide the path to the resx file name to load.
        /// The class will automatically find related files.
        /// </summary>
        /// <param name="filePath">
        /// </param>
        public ResXUnified(string filePath)
        {
            List<string> siblings = this.FindResXSiblings(filePath);

            foreach (string sibling in siblings)
            {
                this.Languages.Add(FindCultureInFilename(sibling), this.ReadResX(sibling));
            }

            this.baseFileName = GetBaseName(filePath);

            this.basePath = Path.GetDirectoryName(filePath);
        }

        // Public Methods
        #region Public Methods

        /// <summary>
        /// This is how to access the data.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public ResXUnifiedIndexer this[string language]
        {
            get
            {
                lock (this.lck)
                {
                    if (this.Languages.ContainsKey(language))
                    {
                        return new ResXUnifiedIndexer(this, language);
                    }

                    return new ResXUnifiedIndexer();
                }
            }
        }

        /// <summary>
        /// Get a list of all the languages from this unified resx oject.
        /// </summary>
        /// <returns></returns>
        public SortedList<string, string> GetLanguages()
        {
            var keys = new SortedList<string, string>();
            lock (this.lck)
            {
                foreach (var key in this.Languages.Keys)
                {
                    keys.Add(key, key);
                }
            }

            return keys;
        }

        /// <summary>
        /// Returns a DataTable wich is easy to diplay in a GridView.
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable(bool removeEmpty)
        {
            var table = new DataTable();
            table.Columns.Add("Key");

            lock (this.lck)
            {
                foreach (var lang in this.Languages.Keys)
                {
                    table.Columns.Add(lang);
                }

                foreach (var key in this.UnifiedKeys)
                {
                    var isEmpty = false;
                    if (removeEmpty)
                    {
                        isEmpty = this.IsKeyEmpty(key);
                    }

                    if (!isEmpty)
                    {
                        var row = table.NewRow();
                        table.Rows.Add(row);
                        row["Key"] = key;

                        foreach (var lang in this.Languages.Keys)
                        {
                            row[lang] = this[lang][key];
                        }
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// The to data table.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable ToDataTable()
        {
            return this.ToDataTable(false);
        }

        /// <summary>
        /// Determine if the specified key have any value associated with it in any language.
        /// If it finds a value, it returns false, otherwise it returns true.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool IsKeyEmpty(string key)
        {
            return this.Languages.Keys.All(lang => string.IsNullOrEmpty(this[lang][key]));
        }

        /// <summary>
        /// The add language.
        /// </summary>
        /// <param name="lang">
        /// The lang.
        /// </param>
        public void AddLanguage(string lang)
        {
            lock (this.Languages)
            {
                this.Languages.Add(lang, new Dictionary<string, string>());
                this.changed.Add(lang);
            }
        }

        /// <summary>
        /// Save changes to disk made to this object.
        /// Files will be saved on the correct files.
        /// </summary>
        public void Save()
        {
            lock (this.lck)
            {
                foreach (string lang in this.Languages.Keys)
                {
                    this.WriteResX(this.GetFileName(lang), this.Languages[lang]);
                }

                this.changed.Clear();
            }
        }
        #endregion

        // Utility Methods
        #region Utility Methods

        /// <summary>
        /// Check in the same directory as the file and find related resx
        /// files that we can add to the unified object.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected List<string> FindResXSiblings(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);

            string fileName = GetBaseName(filePath);

            if (directoryName != null)
            {
                var dir = new DirectoryInfo(directoryName);
                var files = dir.GetFiles("*.resx", SearchOption.TopDirectoryOnly);

                return (from file in files where file.Name.StartsWith(fileName) select file.FullName).ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Open a ResX file and extract it's data.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected Dictionary<string, string> ReadResX(string filename)
        {
            var extracted = new Dictionary<string, string>();

            try
            {
                using (var reader = new ResXResourceReader(filename))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        extracted.Add((string)entry.Key, (string)entry.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem loading ResX: " + filename + "." + ex.Message);
            }

            return extracted;
        }

        /// <summary>
        /// Write a ResX file to disk
        /// </summary>
        /// <param name="fileName">
        /// </param>
        /// <param name="dict">
        /// The dict.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool WriteResX(string fileName, Dictionary<string, string> dict)
        {
            try
            {
                using (var writer = new ResXResourceWriter(fileName))
                {
                    foreach (var key in dict.Keys)
                    {
                        writer.AddResource(key, dict[key]);
                    }
                    writer.Generate();
                }
            }
            catch
            {
                try
                {
                    using (var writer = new ResXResourceWriter(fileName))
                    {
                        foreach (string key in dict.Keys)
                        {
                            writer.AddResource(key, dict[key]);
                        }

                        writer.Generate();
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Simply find the culture string from the resx file name.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string FindCultureInFilename(string filename)
        {
            string file = filename.Substring(filename.IndexOf(GetBaseName(filename), StringComparison.Ordinal) + GetBaseName(filename).Length + 1); // remove base name plus the dot. 
            string[] split = file.Split('.');

            if (split.Length == 2)
            { 
                return split[0];
            }

            if (split.Length > 2)
            {
                throw new Exception(
                    "Invalid base resx name. Filenames other than aspx/ascx/ashx/asmx/master are assumed not to contain any periods.");
            }

            return "Default";
        }

        /// <summary>
        /// Returns the complete path of the resx of a language for the current resx we are editing
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        protected string GetFileName(string language)
        {
            if (language != "Default")
            {
                return Path.Combine(this.basePath, this.baseFileName + "." + language + ".resx");
            }

            return Path.Combine(this.basePath, this.baseFileName + ".resx");
        }

        /// <summary>
        /// Gets the path of a resx file and then returns the base
        /// file name for this resx.  For example for Default.aspx.fr-CA.resx,
        /// the base file name would be Default.aspx.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetBaseName(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension == null || !extension.ToLower().EndsWith("resx"))
            {
                return filePath;
            }
            var file = Path.GetFileName(filePath);

            var split = file.Split('.');


            // assumption: filenames don't have . in them except if IsAspNetFile... 
            if (IsAspNetFile(file))
            {
                return split[0] + "." + split[1];
            }

            return split[0];
        }

        /// <summary>
        /// The is asp net file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsAspNetFile(string file)
        {
            return file.ToLower().Contains(".ascx.") || file.ToLower().Contains(".aspx.") || file.ToLower().Contains(".master.") || file.ToLower().Contains(".asmx.") || file.ToLower().Contains(".ashx.");
        }

        /// <summary>
        /// The get res x in directory.
        /// </summary>
        /// <param name="basePath">
        /// The base path.
        /// </param>
        /// <returns>
        /// The <see cref="SortedList"/>.
        /// </returns>
        public static SortedList<string, string> GetResXInDirectory(string basePath)
        {
            return GetResXInDirectory(basePath, null);
        }

        /// <summary>
        /// Returns a list of all the resx files contained recursivelly in the basePath directory passed.
        /// The
        /// </summary>
        /// <param name="basePath">Where do you want to search for .resx</param>
        /// <param name="display">A predicate function to modify the string to be displayed.  Can be null.</param>
        /// <returns></returns>
        public static SortedList<string, string> GetResXInDirectory(string basePath, GenericPredicate<string, string> display)
        {
            var dir = new DirectoryInfo(basePath);
            var files = dir.GetFiles("*.resx", SearchOption.AllDirectories);

            var dict = new SortedList<string, string>();

            foreach (var file in files)
            {
                var baseName = ResXUnified.GetBaseName(file.FullName);
                var path = Path.GetDirectoryName(file.FullName);

                var displayName = display == null ? path : display(path, basePath);
                if (displayName != null)
                {
                    displayName = Path.Combine(displayName, baseName);
                }

                if (displayName != null && !dict.ContainsKey(displayName))
                {
                    if (!dict.ContainsKey(displayName))
                    {
                        dict.Add(displayName, file.FullName);
                    }
                }
            }

            return dict;
        }

        #endregion

        // Properties
        #region Properties

        /// <summary>
        /// The languages.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Languages = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// The unified kkeys.
        /// </summary>
        private readonly List<string> unifiedKkeys = new List<string>();

        /// <summary>
        /// Gets the unified keys.
        /// </summary>
        public List<string> UnifiedKeys
        {
            get
            {
                foreach (var lang in this.Languages.Keys)
                {
                    foreach (var key in this.Languages[lang].Keys)
                    {
                        if (!this.unifiedKkeys.Contains(key))
                        {
                            this.unifiedKkeys.Add(key);
                        }
                    }
                }

                return this.unifiedKkeys;
            }
        }

        #endregion

        // Indexer Class
        #region Indexer Class

        /// <summary>
        /// The res x unified indexer.
        /// </summary>
        public class ResXUnifiedIndexer
        {
            /// <summary>
            /// The resx.
            /// </summary>
            private readonly ResXUnified resx;

            /// <summary>
            /// The language.
            /// </summary>
            private readonly string language;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResXUnifiedIndexer"/> class.
            /// </summary>
            public ResXUnifiedIndexer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ResXUnifiedIndexer"/> class.
            /// </summary>
            /// <param name="resx">
            /// The resx.
            /// </param>
            /// <param name="language">
            /// The language.
            /// </param>
            public ResXUnifiedIndexer(ResXUnified resx, string language)
            {
                this.resx = resx;
                this.language = language;
            }

            /// <summary>
            /// The this.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string this[string key]
            {
                get
                {
                    if (this.resx != null && this.resx.Languages[this.language].ContainsKey(key))
                    {
                        return this.resx.Languages[this.language][key];
                    }

                    return string.Empty;
                }

                set
                {
                    if (this.resx != null && !string.IsNullOrEmpty(key) && this.resx.Languages[this.language].ContainsKey(key))
                    {
                        // Change it only if the two values are different
                        if (this.resx.Languages[this.language][key] != value)
                        {
                            // Mark the file as changed
                            this.resx.changed.Add(this.language);
                            this.resx.Languages[this.language][key] = value;
                        }
                    }
                    else if (this.resx != null && !string.IsNullOrEmpty(key) && !this.resx.Languages[this.language].ContainsKey(key))
                    {
                        // Mark the file as changed
                        this.resx.changed.Add(this.language);
                        this.resx.Languages[this.language].Add(key, value);
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// The generic predicate.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <typeparam name="R">
    /// </typeparam>
    public delegate R GenericPredicate<T, R>(params T[] obj);
}