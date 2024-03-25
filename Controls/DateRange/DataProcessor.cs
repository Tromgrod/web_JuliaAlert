namespace Controls.DateRange
{
    using System;
    using Controls.DateRange.Models;
    using LIB.AdvancedProperties;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;
    using Weblib.Models.Common.Enums;
    using System.Globalization;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    using System.Web;

    public class DataProcessor : IDataProcessor
    {
        string[] DateFormats = { "dd/MM/yyyy", "dd/MM", "MM/yyyy", "yyyy" };

        public object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple)
        {
            var model = new DateRangeModel();
            
            try
            {
                model.ReadOnly = ReadOnly;

                var TextboxTypeVal = TextboxType.DateTime;

                model.Value = value != null ? ((LIB.Tools.Controls.DateRange)value) : new LIB.Tools.Controls.DateRange();

                model.TextBoxFrom = new TextboxModel() { Name = property.PropertyName + "From", Class = $"calendar-input small-input from {property.Common.ViewCssClass}{(property.Common.SearchPopUpDate ? " popup-search search-from" : "")}", Type = TextboxTypeVal };
                if(model.Value.From!= DateTime.MinValue)
                {
                    model.TextBoxFrom.Value = model.Value.From.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                model.TextBoxTo = new TextboxModel() { Name = property.PropertyName + "To", Class = $"calendar-input small-input to {property.Common.ViewCssClass}{(property.Common.SearchPopUpDate ? " popup-search search-to" : "")}", Type = TextboxTypeVal };
                if (model.Value.To != DateTime.MinValue)
                {
                    model.TextBoxTo.Value = model.Value.To.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (property.Common.EditTemplate == EditTemplates.DateTimeRange)
                {
                    model.ShowTime = true;
                }

                model.CssView += property.Common.ViewCssClass;
                model.CssEdit += property.Common.EditCssClass;

                model.Mode = mode;
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, HttpContext.Current);
            }

            return model;
        }

        public object GetValue(AdvancedProperty property, string prefix = "", DisplayMode mode = DisplayMode.Simple)
        {
            LIB.Tools.Controls.DateRange range = new LIB.Tools.Controls.DateRange();

            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]))
                {
                    var date = !string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]) ? DateTime.ParseExact(
                        HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"],
                        this.DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.MinValue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From" + "_Hours"]))
                        date = date.AddHours(Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From" + "_Hours"]));
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From" + "_Minutes"]))
                        date = date.AddMinutes(Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From" + "_Minutes"]));

                    range.From = date;
                }
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]))
                {
                    var date = !string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]) ? DateTime.ParseExact(
                        HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"],
                        this.DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.MinValue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To" + "_Hours"]))
                        date = date.AddHours(Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To" + "_Hours"]));
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To" + "_Minutes"]))
                        date = date.AddMinutes(Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To" + "_Minutes"]));

                    range.To = date;
                }
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, HttpContext.Current);
            }
            
            return range;
        }
        
        public string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            var strValue = "";

            if (value == null)
                return "";

            var DateRange = (LIB.Tools.Controls.DateRange)value;

            if (DateRange.From != DateTime.MinValue && DateRange.To != DateTime.MinValue)
                strValue = DateRange.From.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "-" + DateRange.To.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            else if (DateRange.From != DateTime.MinValue)
                strValue = DateRange.From.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            else if (DateRange.To != DateTime.MinValue)
                strValue = DateRange.To.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return strValue;
        }

        public object ToObject(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            return ToString(value, property, BOItem, mode);
        }
    }
}
