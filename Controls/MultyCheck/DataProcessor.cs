namespace Controls.MultyCheck
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;

    using Controls.MultyCheck.Models;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools.Controls;
    using LIB.Tools.Utils;

    using Weblib.Models.Common;
    using LIB.BusinessObjects;


    public class DataProcessor : IDataProcessor
    {
        public object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple)
        {
            var model = new Controls.MultyCheck.Models.MultyCheckModel();

            //General.TraceWrite(property.PropertyName);
            try
            {
                model.ReadOnly = ReadOnly;

                model.MultyCheck = new MultyCheckWidgetModel() { Name = property.PropertyName, InputClass = " flat-red" };

                var LookUpCollection = (Dictionary<string, Dictionary<long, ItemBase>>)ContextItemsHolder.ObjectFromContext(ContextItemsHolder.DropDownLookUp);
                if (LookUpCollection == null)
                {
                    LookUpCollection = new Dictionary<string, Dictionary<long, ItemBase>>();
                }

                if (!LookUpCollection.ContainsKey(property.PropertyName))
                {
                    if (property.Common.EditTemplate != EditTemplates.PermissionsSelector)
                    {
                        var item = (ItemBase)Activator.CreateInstance((property.Custom as MultiCheck).ItemType);
                        var AdvancedFilter = BOItem.GetAdvancedLookUpFilter(property);
                        AdvancedFilter += property.Custom != null && property.Custom is MultiCheck ? ((MultiCheck)property.Custom).AdvancedFilter : "";

                        var collection = item.Populate(null, null, true, AdvancedFilter);
                        LookUpCollection.Add(property.PropertyName, collection);
                    }
                    else
                    {
                        var collection = (new Permission()).Populate(null, null, true);
                        LookUpCollection.Add(property.PropertyName, collection);
                    }
                }
                if (property.Custom != null && property.Custom is LIB.AdvancedProperties.MultiCheck && !string.IsNullOrEmpty(((LIB.AdvancedProperties.MultiCheck)property.Custom).GpoupByField))
                {
                    model.MultyCheck.Groups = new Dictionary<string, Dictionary<long, ItemBase>>();
                    foreach (var item in LookUpCollection[property.PropertyName].Values)
                    {
                        var GroupItem = item.GetGroupField(((LIB.AdvancedProperties.MultiCheck)property.Custom).GpoupByField);
                        if (!model.MultyCheck.Groups.ContainsKey(GroupItem))
                            model.MultyCheck.Groups.Add(GroupItem, new Dictionary<long, ItemBase>());

                        model.MultyCheck.Groups[GroupItem].Add(item.Id, item);
                    }
                }
                else
                {
                    model.MultyCheck.Options = LookUpCollection[property.PropertyName];
                }
                ContextItemsHolder.ObjectToContext(ContextItemsHolder.DropDownLookUp, LookUpCollection);

                switch (property.Common.EditTemplate)
                {
                    case EditTemplates.MultiCheck:
                        model.Values = (Dictionary<long, ItemBase>)value;
                        if (model.Values == null)
                        {
                            model.Values = new Dictionary<long, ItemBase>();
                        }
                        foreach (var key in model.Values.Keys)
                        {
                            model.Values[key].SetName(LookUpCollection[property.PropertyName].Values.FirstOrDefault(i => i.Id == model.Values[key].Id).GetName());
                        }
                        break;

                    case EditTemplates.PermissionsSelector:
                        model.Values = Permission.LoadPermissions((long)value);
                        if (model.Values == null)
                        {
                            model.Values = new Dictionary<long, ItemBase>();
                        }
                        break;
                }

                model.MultyCheck.Values = model.Values;
                model.Mode = mode;
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, System.Web.HttpContext.Current);
            }

            return model;
        }

        public object GetValue(AdvancedProperty property, string prefix = "", DisplayMode mode = DisplayMode.Simple)
        {
            //General.TraceWrite(property.PropertyName);

            try
            {
                if (property.Common.EditTemplate == EditTemplates.PermissionsSelector)
                {
                    if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                    {
                        return 0;
                    }
                    long val = 0;
                    foreach (string id in System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(id))
                            val = val | Permission.LoadPermission(Convert.ToInt64(id)).Value;
                    }
                    return val;
                }
                var items = new Dictionary<long, ItemBase>();
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                {
                    foreach (var sid in System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(sid))
                        {
                            var id = Convert.ToInt64(sid);
                            items.Add(id, new ItemBase(id));
                        }
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, System.Web.HttpContext.Current);
            }

            return null;
        }

        public string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            var strValue = "";
            if (value == null)
                value = false;

            switch (property.Common.EditTemplate)
            {
                case EditTemplates.MultiCheck:
                    var Values = (Dictionary<long, ItemBase>)value;
                    foreach (var key in Values.Keys)
                    {
                        strValue += (!string.IsNullOrEmpty(strValue) ? "," : "") + Values[key].GetName();
                    }
                    break;

                case EditTemplates.PermissionsSelector:
                    var PermissionValues = Permission.LoadPermissions((long)value);
                    if (PermissionValues == null)
                    {
                        foreach (var key in PermissionValues.Keys)
                        {
                            strValue += (!string.IsNullOrEmpty(strValue) ? "," : "") + PermissionValues[key].GetName();
                        }
                    }
                    break;
            }

            return strValue;
        }

        public object ToObject(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            return ToString(value, property, BOItem, mode);
        }
    }
}
