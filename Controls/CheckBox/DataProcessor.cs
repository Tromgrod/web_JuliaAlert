namespace Controls.CheckBox
{
    using System;
    using Controls.CheckBox.Models;
    using LIB.AdvancedProperties;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    
    public class DataProcessor : IDataProcessor
    {
        public object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple)
        {
            var model = new CheckBoxModel();
            try
            {
                model.ReadOnly = ReadOnly;

                if (value == null)
                    value = false;
                model.Value = (bool)value;

                model.Checkbox = new CheckboxModel() { Name = property.PropertyName,Id = property.PropertyName + new Random().Next().ToString(), Checked = model.Value, Class = "flat-red" };
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
            try
            {
                return System.Web.HttpContext.Current.Request.Form[property.PropertyName] == "1";
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, System.Web.HttpContext.Current);
            }

            return false;
        }

        public string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            if (value == null)
                value = false;

            return (bool)value ? "Да" : "Нет";
        }

        public object ToObject(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            return ToString(value, property, BOItem, mode);
        }
    }
}