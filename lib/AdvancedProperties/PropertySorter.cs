namespace LIB.AdvancedProperties
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;    

    public class PropertySorter : ExpandableObjectConverter
    {
        #region Public Methods

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public AdvancedProperties GetProperties(PropertyDescriptorCollection pdc)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                orderedProperties.Add(GetProperty(pd));
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetProperties(PropertyDescriptorCollection pdc, User usr, DisplayMode displayMode)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && template != null && template.Access != null)
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && template != null && template.Access != null)
                {
                    access = template.Access;
                }

                if (access != null && ((access.DisplayMode & displayMode) == displayMode)
                    && (access.EditableFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.EditableFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetExcellProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (access != null && ((access.DisplayMode & DisplayMode.Excell) == DisplayMode.Excell)
                    && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.VisibleFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetAvailableProperties(PropertyDescriptorCollection pdc, User usr, List<string> FiedList=null)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (access != null && (access.DisplayMode != DisplayMode.None && access.DisplayMode != DisplayMode.Search)
                    && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.VisibleFor))
                    && (FiedList==null || FiedList.Any(s=>s== GetProperty(pd).PropertyName)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (access != null && ((access.DisplayMode & DisplayMode.Simple) == DisplayMode.Simple)
                    && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.VisibleFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetFormSaveProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (access != null && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.EditableFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetDbProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var db = (Db)pd.Attributes[typeof(Db)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (db != null && (template != null && template.Access != null))
                {
                    db.CopyUserFields(template.Db);
                }
                else if (db == null && (template != null && template.Db != null))
                {
                    db = template.Db;
                }

                var access = (Access)pd.Attributes[typeof(Access)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (
                    ((db != null && (db.Ignore == null || db.Ignore == false) && (db.Populate == null || db.Populate == true || db.Sort != DbSortMode.None)) || db == null)
                     &&
                     (
                      (access != null &&
                      (access.DisplayMode != DisplayMode.Search)
                     ) || access == null)
                    )
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetDbEditProperties(PropertyDescriptorCollection pdc, User usr, DisplayMode displayMode = DisplayMode.None)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var db = (Db)pd.Attributes[typeof(Db)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (db != null && (template != null && template.Access != null))
                {
                    db.CopyUserFields(template.Db);
                }
                else if (db == null && (template != null && template.Db != null))
                {
                    db = template.Db;
                }

                var access = (Access)pd.Attributes[typeof(Access)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (
                    (
                     (db != null &&
                       (((Db)db).Ignore == null || ((Db)db).Ignore == false) &&
                       (((Db)db).Editable == null || ((Db)db).Editable == true)
                      )
                     || db == null)
                    &&
                     (
                          (access != null &&
                              (
                                (displayMode != DisplayMode.None && (access.DisplayMode & displayMode) == displayMode)
                                    ||
                                (displayMode == DisplayMode.None && access.DisplayMode != DisplayMode.Search)
                              )
                              &&
                              (access.VisibleFor == Permission.None.Value || (usr != null && usr.HasAtLeastOnePermission(access.EditableFor)) || usr == null)
                         )
                         ||
                        access == null
                     )
                    )
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetDbReadProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var db = (Db)pd.Attributes[typeof(Db)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (db != null && (template != null && template.Access != null))
                {
                    db.CopyUserFields(template.Db);
                }
                else if (db == null && (template != null && template.Db != null))
                {
                    db = template.Db;
                }

                if ((db != null && (((Db)db).Ignore == null || ((Db)db).Ignore == false) && (((Db)db).Readable == null || ((Db)db).Readable == true)) || db == null)
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetSearchProperties(PropertyDescriptorCollection pdc)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var template = (Template)pd.Attributes[typeof(Template)];
                var common = (Common)pd.Attributes[typeof(Common)];

                if (common != null && (template != null && template.Common != null))
                {
                    common.CopyUserFields(template.Common);
                }
                else if (common == null && (template != null && template.Common != null))
                {
                    common = template.Common;
                }

                if (common != null && common._Searchable)
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetTotalColumSumProperties(PropertyDescriptorCollection pdc)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var common = (Common)pd.Attributes[typeof(Common)];

                if (common != null && common.TotalSum)
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetSearchProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];
                var common = (Common)pd.Attributes[typeof(Common)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (common != null && (template != null && template.Common != null))
                {
                    common.CopyUserFields(template.Common);
                }
                else if (common == null && (template != null && template.Common != null))
                {
                    common = template.Common;
                }

                if (common != null && common._Searchable)
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetFilterControlProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var common = (Common)pd.Attributes[typeof(Common)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (common != null && (template != null && template.Common != null))
                {
                    common.CopyUserFields(template.Common);
                }
                else if (common == null && (template != null && template.Common != null))
                {
                    common = template.Common;
                }

                if (common!=null && common._Searchable && access != null && (access.DisplayMode & DisplayMode.Search) == DisplayMode.Search
                    && (access.VisibleFor == Permission.None.Value || usr == null
                        || usr.HasAtLeastOnePermission(access.VisibleFor))
                    && (access.SearchableFor == Permission.None.Value || usr == null
                        || usr.HasAtLeastOnePermission(access.SearchableFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetAdvancedProperties(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                if (access != null && (
                        ((access.DisplayMode & DisplayMode.Advanced) == DisplayMode.Advanced) ||
                        ((access.DisplayMode & DisplayMode.Simple) == DisplayMode.Simple)
                    )
                    && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.VisibleFor)))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }

        public AdvancedProperties GetAdvancedPropertiesForInsert(PropertyDescriptorCollection pdc, User usr)
        {
            var orderedProperties = new AdvancedProperties();
            foreach (PropertyDescriptor pd in pdc)
            {
                var access = (Access)pd.Attributes[typeof(Access)];
                var template = (Template)pd.Attributes[typeof(Template)];

                if (access != null && (template != null && template.Access != null))
                {
                    access.CopyUserFields(template.Access);
                }
                else if (access == null && (template != null && template.Access != null))
                {
                    access = template.Access;
                }

                var db = (Db)pd.Attributes[typeof(Db)];

                if (db != null && (template != null && template.Db != null))
                {
                    db.CopyUserFields(template.Db);
                }
                else if (db == null && (template != null && template.Db != null))
                {
                    db = template.Db;
                }

                if (access != null && (
                        ((access.DisplayMode & DisplayMode.Advanced) == DisplayMode.Advanced) ||
                        ((access.DisplayMode & DisplayMode.Simple) == DisplayMode.Simple)
                    )
                    && (access.VisibleFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.VisibleFor))
                    && (access.EditableFor == Permission.None.Value || usr.HasAtLeastOnePermission(access.EditableFor))
                    && (db == null || (db != null && (((Db)db).Editable == null || ((Db)db).Editable == true))))
                {
                    orderedProperties.Add(GetProperty(pd));
                }
            }
            orderedProperties.Sort();

            return orderedProperties;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="pd">
        /// The Property Descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="AdvancedProperty"/>.
        /// </returns>
        private static AdvancedProperty GetProperty(PropertyDescriptor pd)
        {
            var ap = new AdvancedProperty();

            Template attributeTemplate = null;

            foreach (Attribute attribute in pd.Attributes)
            {
                if (attribute != null)
                {
                    if (attribute is Common)
                    {
                        ap.Common = (Common)attribute;
                    }
                    else if (attribute is Translate)
                    {
                        ap.Translate = (Translate)attribute;
                    }
                    else if (attribute is Service)
                    {
                        ap.Service = (Service)attribute;
                    }
                    else if (attribute is Mark)
                    {
                        ap.Mark = (Mark)attribute;
                    }
                    else if (attribute is Access)
                    {
                        ap.Access = (Access)attribute;
                    }
                    else if (attribute is Encryption)
                    {
                        ap.Encryption = (Encryption)attribute;
                    }
                    else if (attribute is Validation)
                    {
                        ap.Validation = (Validation)attribute;
                    }
                    else if (attribute is Db)
                    {
                        ap.Db = (Db)attribute;
                    }
                    else if (attribute is Template)
                    {
                        attributeTemplate = (Template)attribute;
                    }
                    else if (attribute is Image)
                    {
                        ap.Image = (Image)attribute;
                    }
                    else if ((attribute as PropertyItem) != null && !(attribute is BoAttribute))
                    {
                        ap.Custom = (PropertyItem)attribute;
                    }
                }
            }

            if (attributeTemplate != null)
            {
                if (ap.Common == null && attributeTemplate.Common != null)
                {
                    ap.Common = attributeTemplate.Common;
                }
                else if (attributeTemplate.Common != null)
                {
                    ap.Common.CopyUserFields(attributeTemplate.Common);
                }
                else if (ap.Common == null)
                {
                    ap.Common = new Common();
                }

                if (ap.Translate == null && attributeTemplate.Translate != null)
                {
                    ap.Translate = attributeTemplate.Translate;
                }
                else if (attributeTemplate.Translate != null)
                {
                    ap.Translate.CopyUserFields(attributeTemplate.Translate);
                }
                else if (ap.Translate == null)
                {
                    ap.Translate = new Translate();
                }

                if (ap.Service == null && attributeTemplate.Service != null)
                {
                    ap.Service = attributeTemplate.Service;
                }
                else if (attributeTemplate.Service != null)
                {
                    ap.Service.CopyUserFields(attributeTemplate.Service);
                }
                else if (ap.Service == null)
                {
                    ap.Service = new Service();
                }

                if (ap.Access == null && attributeTemplate.Access != null)
                {
                    ap.Access = attributeTemplate.Access;
                }
                else if (attributeTemplate.Access != null)
                {
                    ap.Access.CopyUserFields(attributeTemplate.Access);
                }
                else if (ap.Access == null)
                {
                    ap.Access = new Access();
                }

                if (ap.Validation == null && attributeTemplate.Validation != null)
                {
                    ap.Validation = attributeTemplate.Validation;
                }
                else if (attributeTemplate.Validation != null)
                {
                    ap.Validation.CopyUserFields(attributeTemplate.Validation);
                }
                else if (ap.Validation == null)
                {
                    ap.Validation = new Validation();
                }

                if (ap.Db == null && attributeTemplate.Db != null)
                {
                    ap.Db = attributeTemplate.Db;
                }
                else if (attributeTemplate.Db != null)
                {
                    ap.Db.CopyUserFields(attributeTemplate.Db);
                }
                else if (ap.Db == null)
                {
                    ap.Db = new Db();
                }

                if (ap.Image == null && attributeTemplate.ImageProperties != null)
                {
                    ap.Image = attributeTemplate.ImageProperties;
                }
                else if (attributeTemplate.ImageProperties != null)
                {
                    ap.Image.CopyUserFields(attributeTemplate.ImageProperties);
                }
                else if (ap.Image == null)
                {
                    ap.Image = new Image();
                }
            }
            ap.PropertyName = pd.Name;
            ap.PropertyDescriptor = pd;
            ap.Type = pd.PropertyType;
            if (ap.Translate != null && ap.Translate.Translatable)
            {
                if (string.IsNullOrEmpty(ap.Translate.TableName))
                {
                    ap.Translate.TableName = ap.PropertyDescriptor.ComponentType.Name + "_" + ap.PropertyName;
                }

                if (string.IsNullOrEmpty(ap.Translate.Alias))
                {
                    ap.Translate.Alias = ap.PropertyName + "Alias";
                }
            }
            if (ap.Validation != null && string.IsNullOrEmpty(ap.Validation.AlertMessage))
            {
                ap.Validation.AlertMessage = ap.Common.DisplayName + " " + Tools.Utils.Translate.GetTranslatedValue("is_required", "AdminArea");
            }
            if (ap.Common != null)
            {
                if (string.IsNullOrEmpty(ap.Common.DisplayName))
                {
                    ap.Common.DisplayName = ap.PropertyName;
                }
                else
                {
                    ap.Common.DisplayName = ap.Common.DisplayName.Replace(" ", "_");
                }

                if (!string.IsNullOrEmpty(ap.Common.PropertyDescription))
                {
                    ap.Common.PropertyDescription = ap.PropertyName + "_description";
                }
            }
            if (ap.Db == null)
            {
                ap.Db = new Db();
            }
            if (string.IsNullOrEmpty(ap.Db.ParamName))
            {
                ap.Db.ParamName = ap.PropertyName;
                if (ap.Type.BaseType == typeof(ItemBase)
                    || (ap.Type.BaseType.BaseType != null && ap.Type.BaseType.BaseType == typeof(ItemBase))
                    || (ap.Type.BaseType.BaseType != null && ap.Type.BaseType.BaseType.BaseType != null && ap.Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                    || ap.Type == typeof(ItemBase)
                    || ap.Type == typeof(User))
                {
                    ap.Db.ParamName += "ID";
                }
            }
            if (ap.Db.ParamType == SqlDbType.Image)
            {
                if (ap.Type == typeof(string))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.NVarChar;
                    if (ap.Db.ParamSize == 0)
                    {
                        ap.Db.ParamSize = 100;
                    }
                    /*if (ap.Db.ParamSize == -1)
                    {
                        ap.Db.ParamSize = 4000;
                    } */
                }
                else if (ap.Type == typeof(DateTime))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.DateTime;
                }
                else if (ap.Type == typeof(bool))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.Bit;
                }
                else if (ap.Type == typeof(int) || ap.Type == typeof(int?))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.Int;
                }
                else if (ap.Type == typeof(decimal))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.Decimal;
                }
                else if (ap.Type == typeof(long))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.BigInt;
                }
                else if (ap.Type.BaseType == typeof(ItemBase)
                    || (ap.Type.BaseType.BaseType != null && ap.Type.BaseType.BaseType == typeof(ItemBase))
                    || (ap.Type.BaseType.BaseType != null && ap.Type.BaseType.BaseType.BaseType != null && ap.Type.BaseType.BaseType.BaseType == typeof(ItemBase)))
                {
                    var item = (ItemBase)Activator.CreateInstance(ap.Type);
                    if (item.GetId() is long)
                    {
                        ap.Db.ParamType = System.Data.SqlDbType.BigInt;
                    }
                    else
                    {
                        ap.Db.ParamType = System.Data.SqlDbType.NVarChar;
                        if (ap.Db.ParamSize == 0)
                        {
                            ap.Db.ParamSize = 100;
                        }
                        /*if (ap.Db.ParamSize == -1)
                        {
                            ap.Db.ParamSize = 4000;
                        } */
                    }
                }
                else if (ap.Type == typeof(Guid))
                {
                    ap.Db.ParamType = System.Data.SqlDbType.UniqueIdentifier;
                }
            }

            return ap;
        }
        #endregion
    }
}