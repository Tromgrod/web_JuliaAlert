using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using System.ComponentModel;
using System.IO;
using System.Resources;
using System.Globalization;

namespace LIB.Tools.Localization
{
    public enum ResourceDataSource { dsCode, dsXml }

    public enum ExportImportMethod
    {
        [Description("csv")]
        csv,
        [Description("xml")]
        xml,
        [Description("resx")]
        resx
    }

    public static class ExportImportMethodHelper
    {
        public static string ToDescription(this Enum value)
        {
            string Description = string.Empty;
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                Description = attributes[0].Description;
            }
            return Description;
        }
    }

    public class ResourceData
    {
        private string key = string.Empty;
        private ResourceDataSource _resourceDataSource;
        protected Hashtable data = new Hashtable();
        private ResourceManager l_resourceManager = null;

        public ResourceData(string key)
        {
            _resourceDataSource = ResourceDataSource.dsCode;
            this.key = key;
            InitValues();
        }

        public ResourceData(XDocument resources)
        {
            _resourceDataSource = ResourceDataSource.dsXml;
            this.key = resources.Root.Attribute("key").Value;
            this.data.Clear();

            var query = from c in resources.Root.Elements("r")
                        where c.Attribute("id") != null && c.Attribute("v") != null
                        select c;
            foreach (var r in query)
            {
                string key = r.Attribute("id").Value;
                if (!data.Contains(key))
                {
                    data.Add(key, r.Attribute("v").Value);
                }
            }
        }

        public ResourceData(ResourceManager resources, string key)
        {
            l_resourceManager = resources;
            this.key = key;
        }

        public virtual void InitValues()
        {
        }

        public void UpdateFrom(ResourceData source)
        {
            IDictionaryEnumerator en;
            if (source.l_resourceManager != null)
            {
                en = source.l_resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
            }
            else
            {
                en = source.data.GetEnumerator();
            }
            while (en.MoveNext())
            {
                if (data.Contains(en.Key))
                {
                    data[en.Key] = en.Value;
                }
                else
                {
                    if (l_resourceManager != null)
                    {
                        data.Add(en.Key, en.Value);
                    }
                }
            }
        }

        public string ExportToCsv()
        {
            StringBuilder sb = new StringBuilder();
            IDictionaryEnumerator en;
            if (l_resourceManager != null)
            {
                en = l_resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
            }
            else
            {
                en = new SortedList(data).GetEnumerator();
            }
            while (en.MoveNext())
            {
                sb.AppendLine((string)en.Key + ",\"" + (string)en.Value + '"');
            }
            return sb.ToString(); ;
        }

        public XmlDocument ExportToXml()
        {
            XmlDocument res = new XmlDocument();
            XmlElement de = res.CreateElement("resources");
            res.AppendChild(de);
            IDictionaryEnumerator en;
            if (l_resourceManager != null)
            {
                en = l_resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
            }
            else
            {
                en = new SortedList(data).GetEnumerator();
            }
            while (en.MoveNext())
            {
                XmlElement r = res.CreateElement("r");
                r.SetAttribute("key", (string)en.Key);
                r.SetAttribute("value", (string)en.Value);
                de.AppendChild(r);
            }
            return res;
        }

        public MemoryStream ExportToResx()
        {
            var res = new MemoryStream();
            var resWriter = new ResXResourceWriter(res);
            IDictionaryEnumerator en;
            if (l_resourceManager != null)
            {
                en = l_resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
            }
            else
            {
                en = new SortedList(data).GetEnumerator();
            }
            try
            {
                while (en.MoveNext())
                {
                    resWriter.AddResource((string)en.Key, (string)en.Value);
                }
            }
            catch
            {
                // duplicates?
                Console.WriteLine((string)en.Key);
            }
            resWriter.Generate();
            return res;
        }

        public string GetValue(string key)
        {
            string res = string.Empty;
            if (data.Contains(key))
            {
                res = data[key] as string;
            }
            else
            {
                if (l_resourceManager != null)
                {
                    try
                    {
                        res = l_resourceManager.GetString(key);
                    }
                    catch(Exception ex)
                    {
                        //TBD handle errors
                    }
                }
            }
            return res;
        }

        public void SetValue(string key, string value)
        {
            data[key] = value;
        }

        public string[] CodeCategories { get; set; }
        public string[] PaceInvTables { get; set; }
        public string CodeCategoriesMask { get; set; }

        public string Key { get { return key; } }
        public ResourceDataSource DataSource { get { return _resourceDataSource; } }
    }
}
