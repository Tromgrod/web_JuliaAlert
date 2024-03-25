using System;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.AdvancedProperties;

namespace LIB.BusinessObjects
{
    [Serializable]
    [Bo(LogRevisions = false)]
    public class Graphic : ItemBase
    {
        public Graphic()
            : base(0) { }

        public Graphic(long id)
            : base(id) { }

        #region properties
        public string Name { get; set; }

        public string Ext { get; set; }

        public string BOName { get; set; }

        [Db(_Ignore = true)]
        public string RelativePath { get; set; }

        [Db(_Ignore = true)]
        public string FullThumbnail
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativePath))
                    return URLHelper.GetUrl(RelativePath);

                if (!string.IsNullOrEmpty(Name))
                    return URLHelper.GetUrl(Config.GetConfigValue("UploadURL") + "/" + BOName + "/" + Name + "_original." + Ext);

                return "";
            }
        }

        [Db(_Ignore = true)]
        public string AdminThumbnail
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativePath))
                    return URLHelper.GetUrl(RelativePath);

                if (!string.IsNullOrEmpty(Name))
                    return URLHelper.GetUrl(Config.GetConfigValue("UploadURL") + "/" + BOName + "/" + Name + "_adminthumb." + Ext);

                return "";
            }
        }

        [Db(_Ignore = true)]
        public string Thumbnail
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativePath))
                    return URLHelper.GetUrl(RelativePath);

                if (!string.IsNullOrEmpty(Name))
                    return URLHelper.GetUrl(Config.GetConfigValue("UploadURL") + "/" + BOName + "/" + Name + "." + Ext);

                return "";
            }
        }

        [Db(_Ignore = true)]
        public string AppSrc
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativePath))
                    return AppDomain.CurrentDomain.BaseDirectory + RelativePath;

                if (!string.IsNullOrEmpty(Name))
                    return AppDomain.CurrentDomain.BaseDirectory + Config.GetConfigValue("UploadURL") + "/" + BOName + "/" + Name + ".jpeg";

                return "";
            }
        }
        #endregion

        public static Graphic ToolsPlaceHolder = new Graphic() { RelativePath = @"Images\placeholders\ToolsPlaceHolder.png" };
        public static Graphic LogoInvoice = new Graphic() { RelativePath = @"Images\print\logo_invoice.png" };
        public static Graphic LogoInvoiceBarcode = new Graphic() { RelativePath = @"Images\print\logo_invoice_barcobe.png" };
    }
}