// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translation.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the StaticTranslations type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    using LIB.Tools.AdminArea;

    /// <summary>
    /// The static translations.
    /// </summary>
    [Serializable]
    public class Translation : ItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        public Translation()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Translation(long id)
            : base(id)
        {
        }

        #region properties

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /*[Common(Order = 0, EditTemplate = EditTemplates.SimpleInput, DisplayName = "Admin_ID",
            ControlClass = CssClass.Wide),
         Access(DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced)]*/
        public string Alias { get; set; }

        #endregion

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="staticTranslations">
        /// The static translations.
        /// </param>
        /// <param name="pathtoResource">
        /// The path to resource.
        /// </param>
        public void Insert(Translation staticTranslations, string pathtoResource)
        {
            var resFile = new ResXUnified(pathtoResource);
            
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(staticTranslations.GetType());
            var properties = pss.GetSearchProperties(pdc);
            var isdefault = true;

            foreach (AdvancedProperty property in properties)
            {
                if (!string.IsNullOrEmpty(property.Service.LanguageAbbr))
                {
                    var translation = property.PropertyDescriptor.GetValue(staticTranslations);
                    if (translation != null)
                    {
                        if (isdefault)
                        {
                            resFile["Default"][staticTranslations.Alias] = translation.ToString();
                        }

                        resFile[property.Service.LanguageAbbr][staticTranslations.Alias] = translation.ToString();
                    }
                    isdefault = false;
                }
            }

            resFile.Save();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="staticTranslations">
        /// The static translations.
        /// </param>
        /// <param name="pathtoResource">
        /// The path to resource.
        /// </param>
        public void Update(Translation staticTranslations, string pathtoResource)
        {
            var resFile = new ResXUnified(pathtoResource);
            
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(staticTranslations.GetType());
            var properties = pss.GetSearchProperties(pdc);
            var isdefault = true;

            foreach (AdvancedProperty property in properties)
            {
                if (!string.IsNullOrEmpty(property.Service.LanguageAbbr))
                {
                    var translation = property.PropertyDescriptor.GetValue(staticTranslations);
                    if (translation != null)
                    {
                        if (isdefault)
                        {
                            resFile["Default"][staticTranslations.Alias] = translation.ToString();
                        }

                        resFile[property.Service.LanguageAbbr][staticTranslations.Alias] = translation.ToString();
                        isdefault = false;
                    }
                }
            }
            
            resFile.Save();
        }     

        /// <summary>
        /// The get translations for tools.
        /// </summary>
        /// <param name="staticTranslations">
        /// The static translations.
        /// </param>
        /// <param name="pathtoResource">
        /// The path to resource.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<long, ItemBase> GetStaticTranslationsForTools(Translation staticTranslations, string pathtoResource)
        {
            var staticTranslationsWorker = new Dictionary<long, ItemBase>();

            var resFile = new ResXUnified(pathtoResource);

            var dt = resFile.ToDataTable();

            int idSec = 1;
            
            foreach (DataRow dr in dt.Rows)
            {
                var st = (Translation)staticTranslations.GetType().InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);

                st.Id = idSec;
                st.Alias = dr["Key"].ToString();

                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(st.GetType());
                var properties = pss.GetSearchProperties(pdc);
                var isLikeAlias = false;

                foreach (var property in properties.Cast<AdvancedProperty>().Where(property => !string.IsNullOrEmpty(property.Service.LanguageAbbr)))
                {
                    property.PropertyDescriptor.SetValue(st, dr[property.Service.LanguageAbbr].ToString());

                    if (Strings.Like(dr[property.Service.LanguageAbbr].ToString(), staticTranslations.Alias))
                    {
                        isLikeAlias = true;
                    }
                }

                if (!string.IsNullOrEmpty(staticTranslations.Alias))
                {
                    if (Strings.Like(st.Alias, staticTranslations.Alias) || isLikeAlias)
                    {
                        staticTranslationsWorker.Add(idSec, st);
                        idSec++;
                    }
                }
                else
                {
                    staticTranslationsWorker.Add(idSec, st);
                    idSec++;
                }
            }

            return staticTranslationsWorker;
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        public override void Insert(ItemBase item, string Comment = "", SqlConnection connection = null, User user = null)
        {
            Insert((Translation)item, (new HttpContextWrapper(HttpContext.Current)).Server.MapPath("~/App_GlobalResources/strings.resx"));
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "", SqlConnection connection = null)
        {
            Update((Translation)item, (new HttpContextWrapper(HttpContext.Current)).Server.MapPath("~/App_GlobalResources/strings.resx"));
        }
    }
}