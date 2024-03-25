// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Document.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Document type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using LIB.Tools.BO;
    using LIB.AdvancedProperties;
    using LIB.Tools.Utils;

    /// <summary>
    /// The document.
    /// </summary>
    [Serializable]
    [Bo(LogRevisions = false)]
    public class Document : ItemBase
    {
        public Document()
            : base(0) { }

        public Document(long id)
            : base(id) { }

        public string FileName { get; set; }

        public string Name { get; set; }

        [Db(_Ignore = true)]
        private string _RelativePath { get; set; }

        public string Ext { get; set; }

        [Db(_Ignore = true)]
        public string FileWeb
        {
            get
            {
                if (!string.IsNullOrEmpty(_RelativePath))
                    return URLHelper.GetUrl(_RelativePath);

                else if (!string.IsNullOrEmpty(FileName))
                    return URLHelper.GetUrl(Config.GetConfigValue("UploadURL") + "/Documents/" + FileName + "." + Ext);

                else if(!string.IsNullOrEmpty(Name))
                    return URLHelper.GetUrl(Config.GetConfigValue("UploadURL") + "/Documents/" + Name + "." + Ext);

                return "";
            }
        }

        [Db(_Ignore = true)]
        public string File
        {
            get
            {
                if (!string.IsNullOrEmpty(_RelativePath))
                    return AppDomain.CurrentDomain.BaseDirectory + _RelativePath;

                else if (!string.IsNullOrEmpty(FileName))
                    return $"{AppDomain.CurrentDomain.BaseDirectory}/{Config.GetConfigValue("UploadURL")}/Documents/{FileName}.{Ext}";

                else if (!string.IsNullOrEmpty(Name))
                    return $"{AppDomain.CurrentDomain.BaseDirectory}/{Config.GetConfigValue("UploadURL")}/Documents/{Name}.{Ext}";

                return "";
            }
        }
    }
}