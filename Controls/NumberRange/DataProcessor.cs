namespace Controls.NumberRange
{
    using System;
    using System.Web;
    using Controls.NumberRange.Models;
    using LIB.AdvancedProperties;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;
    using Weblib.Models.Common.Enums;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;

    public class DataProcessor : IDataProcessor
    {
        public object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple)
        {
            var model = new NumberRangeModel();

            try
            {
                model.ReadOnly = ReadOnly;

                var TextboxTypeVal = TextboxType.Number;

                switch (property.Common.EditTemplate)
                {
                    case EditTemplates.NumberRange:
                        var numbersRange = (NumbersRange)value;

                        model.Value = value != null && numbersRange.From != numbersRange.To ? numbersRange.From.ToString() : "0";

                        model.TextBoxFrom = new TextboxModel() { Name = property.PropertyName + "From", Class = "input input-numberrange", Type = TextboxTypeVal, Value = value != null ? numbersRange.From.ToString() : "" };
                        model.TextBoxTo = new TextboxModel() { Name = property.PropertyName + "To", Class = "input input-numberrange", Type = TextboxTypeVal, Value = value != null ? numbersRange.To.ToString() : "" };
                        break;
                    case EditTemplates.DecimalNumberRange:
                        var decimalNumberRange = (DecimalNumberRange)value;

                        model.Value = value != null && decimalNumberRange.From != decimalNumberRange.To ? decimalNumberRange.From.ToString() : "0";

                        if (property.Common.DecimalRound > 0)
                            model.Value = Math.Round(decimal.Parse(model.Value), property.Common.DecimalRound, MidpointRounding.AwayFromZero).ToString();

                        model.Value += value != null ? decimalNumberRange.PostFix : string.Empty;

                        model.TextBoxFrom = new TextboxModel() { Name = property.PropertyName + "From", Class = "input input-numberrange", Type = TextboxTypeVal, Value = value != null && decimalNumberRange.From != 0 ? decimalNumberRange.From.ToString() : "" };
                        model.TextBoxTo = new TextboxModel() { Name = property.PropertyName + "To", Class = "input input-numberrange", Type = TextboxTypeVal, Value = value != null && decimalNumberRange.To != 0 ? decimalNumberRange.To.ToString() : "" };
                        break;
                }
                model.Mode = mode;
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());
            }

            model.Value += property.Common.Postfix;
            model.CssView += property.Common.ViewCssClass;
            model.ReportModifyFunc += property.Common.ReportModifyFunc;

            return model;
        }
        public object GetValue(AdvancedProperty property, string prefix = "", DisplayMode mode = DisplayMode.Simple)
        {
            switch (property.Common.EditTemplate)
            {
                case EditTemplates.NumberRange:
                    var range = new NumbersRange();

                    try
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]))
                        {
                            range.From = Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]);
                        }
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]))
                        {
                            range.To = Convert.ToInt32(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        General.TraceWarn(ex.ToString());
                    }

                    return range;
                case EditTemplates.DecimalNumberRange:
                    var decimalrange = new DecimalNumberRange();

                    try
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]))
                        {
                            decimalrange.From = Convert.ToDecimal(HttpContext.Current.Request.Form[prefix + property.PropertyName + "From"]);
                        }
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]))
                        {
                            decimalrange.To = Convert.ToDecimal(HttpContext.Current.Request.Form[prefix + property.PropertyName + "To"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        General.TraceWarn(ex.ToString());
                    }
                    return decimalrange;
            }

            return null;
        }
        public string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            string strValue;

            if (value == null)
                return "";

            dynamic NumbersRange = value;

            if (NumbersRange.From != 0 && NumbersRange.To != 0)
                strValue = NumbersRange.From.ToString() + "-" + NumbersRange.To.ToString();
            else if (NumbersRange.From != 0)
                strValue = NumbersRange.From.ToString();
            else if (NumbersRange.To != 0)
                strValue = NumbersRange.To.ToString();
            else
                strValue = NumbersRange.From.ToString();

            if (property.Common.EditTemplate == EditTemplates.DecimalNumberRange && property.Common.DecimalRound > 0)
            {
                strValue = Math.Round(decimal.Parse(strValue), property.Common.DecimalRound, MidpointRounding.AwayFromZero).ToString();
            }

            strValue += NumbersRange.PostFix;

            return strValue;
        }
    }
}
