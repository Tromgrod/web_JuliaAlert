namespace Controls.Input
{
    using System;

    using Controls.Input.Models;
    using LIB.AdvancedProperties;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;
    using Weblib.Models.Common.Enums;
    using System.Globalization;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    using LIB.BusinessObjects;

    public class DataProcessor : IDataProcessor
    {
        public object SetValue(object value, AdvancedProperty property, ItemBase BOItem, bool ReadOnly = false, DisplayMode mode = DisplayMode.Simple)
        {
            var model = new InputModel() { Mode = mode };

            try
            {
                model.ReadOnly = ReadOnly
                                ||
                                (property.Access.EditableFor != (long)BasePermissionenum.None && !(LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(property.Access.EditableFor)))
                                ||
                                (property.Db != null && property.Db.Editable == false && mode != DisplayMode.FrontEnd);

                var TextboxTypeVal = TextboxType.Text;
                var InputVal = "";
                var useDefault = property.Custom != null && property.Custom is LookUp up && up.DefaultValue;

                switch (property.Common.EditTemplate)
                {
                    case EditTemplates.DateInput:
                        TextboxTypeVal = TextboxType.Date;
                        if (useDefault && mode == DisplayMode.Search)
                        {
                            model.Value = "";
                        }
                        else
                        {
                            DateTime origdate = value is DateTime time && time != DateTime.MinValue ? time : DateTime.MinValue;
                            DateTime date = value is DateTime && (DateTime)value != DateTime.MinValue ? (DateTime)value : (useDefault ? DateTime.MinValue : DateTime.Now);
                            model.Value = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            InputVal = (date != DateTime.MinValue) ? date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                            if (origdate == DateTime.MinValue) model.Value = "";
                        }
                        break;

                    case EditTemplates.DateTimeInput:

                        TextboxTypeVal = TextboxType.DateTime;
                        model.HourTextBox = new TextboxModel();
                        model.MinutesTextBox = new TextboxModel();

                        if (useDefault && mode == DisplayMode.Search)
                        {
                            model.Value = "";
                        }
                        else
                        {
                            DateTime origdatetime = value is DateTime time && time != DateTime.MinValue ? time : DateTime.MinValue;
                            DateTime datetime = value is DateTime && (DateTime)value != DateTime.MinValue ? (DateTime)value : (useDefault ? DateTime.MinValue : DateTime.Now);
                            model.Value = datetime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                            InputVal = (datetime != DateTime.MinValue) ? datetime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                            if (origdatetime == DateTime.MinValue) model.Value = "";

                            model.HourTextBox = new TextboxModel() { Name = property.PropertyName + "_Hours", Type = TextboxTypeVal, Value = datetime.ToString("HH", CultureInfo.InvariantCulture), Class = "input input-small", MaxLength = 2, OnKeyUp = "isHourKey(this)", OnKeyPress = "return isNumberKey(event)" };
                            model.MinutesTextBox = new TextboxModel()
                            {
                                Name = property.PropertyName + "_Minutes",
                                Type = TextboxTypeVal,
                                Value = datetime.ToString("mm", CultureInfo.InvariantCulture),
                                Class = "input input-small",
                                MaxLength = 2,
                                OnKeyUp = "isMinuteKey(this)",
                                OnKeyPress = "return isNumberKey(event)"
                            };
                        }
                        break;

                    case EditTemplates.Password:
                        model.Value = "";
                        TextboxTypeVal = TextboxType.Password;
                        break;

                    case EditTemplates.MultiLine:
                        TextboxTypeVal = TextboxType.MultiLine;
                        model.Value = value != null ? value.ToString() : "";
                        break;

                    case EditTemplates.HtmlInput:
                        TextboxTypeVal = TextboxType.HTML;
                        model.Value = value != null ? value.ToString() : "";
                        break;

                    case EditTemplates.Label:
                    case EditTemplates.NotUpdatableInput:
                        model.ReadOnly = true;
                        model.Value = value != null ? value.ToString() : "";
                        break;

                    case EditTemplates.ColorPicker:
                        TextboxTypeVal = TextboxType.Color;
                        model.Value = value != null ? value.ToString() : "";
                        break;

                    default:
                        model.Value = value != null ? value.ToString() : "";
                        break;
                }

                model.TextBox = new TextboxModel() { Name = property.PropertyName, Type = TextboxTypeVal, Value = !string.IsNullOrEmpty(InputVal) ? InputVal : model.Value };
                if (property.Custom is InputBox box)
                {
                    if (!string.IsNullOrEmpty(box.OnChange))
                    {
                        model.TextBox.OnChange = box.OnChange;
                    }
                }
                if (property.Validation != null)
                {
                    model.TextBox.ValidationType = property.Validation.ValidationType;
                    model.TextBox.RegularExpression = property.Validation.RegularExpression;
                    model.TextBox.ValidationFuction = property.Validation.ValidationFunction;
                    if (!string.IsNullOrEmpty(property.Validation.AlertMessage))
                        model.TextBox.RequiredMessage = property.Validation.AlertMessage;
                }
                model.Label = property.Common.DisplayName;

                if (property.Common.EditTemplate == EditTemplates.DateInput
                    || property.Common.EditTemplate == EditTemplates.DateTimeInput)
                {
                    model.TextBox.Class = "input calendar-input small-input";
                    model.CssEdit = " control-input-date";
                }
                else if (property.Common.EditTemplate == EditTemplates.DiagnosticInput)
                {
                    model.TextBox.Class = "input diagn-input";
                }
                else if (property.Common.EditTemplate == EditTemplates.ColorPicker)
                {
                    model.TextBox.Class = "input";
                    model.CssView = "colorpicker-view ";
                    model.Color = model.Value;
                }
                else
                {
                    model.TextBox.Class = property.Common.ControlClass;
                }

                model.CssView += property.Common.ViewCssClass;
                model.CssEdit += property.Common.EditCssClass;
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
                switch (property.Common.EditTemplate)
                {
                    case EditTemplates.DateInput:

                        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                        {
                            return DateTime.ParseExact(
                                System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName],
                                @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        return DateTime.MinValue;

                    case EditTemplates.HtmlPopUpInput:
                        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_fullpopupvalue"]))
                        {
                            return System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_fullpopupvalue"];
                        }
                        return "";

                    case EditTemplates.HtmlInput:
                        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_fullpopupvalue"]))
                        {
                            return System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_fullpopupvalue"];
                        }
                        return System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName];

                    case EditTemplates.DateTimeInput:
                        if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                        {
                            var date = !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]) ? DateTime.ParseExact(
                                       System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName],
                                        @"dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.MinValue;
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_Hours"]))
                            {
                                date = date.AddHours(Convert.ToInt32(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_Hours"]));
                            }
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_Minutes"]))
                            {
                                date = date.AddMinutes(Convert.ToInt32(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName + "_Minutes"]));
                            }
                            return date;
                        }
                        return DateTime.MinValue;

                    case EditTemplates.Password:
                        return GetpasswordHash(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]);

                    case EditTemplates.DiagnosticInput:
                        var item = (ItemBase)Activator.CreateInstance(property.Type);
                        if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                            return null;
                        return item.LoadFromString(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName], property.PropertyName);

                }
                if (property.Type == typeof(decimal))
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                        return Convert.ToDecimal(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName].Replace(",", "."), CultureInfo.InvariantCulture);

                    return (decimal)0;
                }
                if (property.Type == typeof(int) || property.Type == typeof(int?))
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                        return Convert.ToInt32(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]);

                    return 0;
                }
                if (property.Type == typeof(long))
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]))
                        return Convert.ToInt64(System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName]);

                    return 0;
                }

                return System.Web.HttpContext.Current.Request.Form[prefix + property.PropertyName];
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());

                if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                    ExceptionManagement.HandleExceptionByAPI(ex, System.Web.HttpContext.Current);
            }

            return "";
        }


        /// <summary>
        /// The getpassword hash.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetpasswordHash(string Password)
        {
            if (!string.IsNullOrEmpty(Password))
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "md5");
            }

            return string.Empty;
        }

        public string ToString(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            string strValue;
            switch (property.Common.EditTemplate)
            {
                case EditTemplates.DateInput:

                    DateTime date = value is DateTime time && time != DateTime.MinValue ? time : DateTime.Now;
                    strValue = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (date == DateTime.MinValue) strValue = "";
                    break;

                case EditTemplates.DateTimeInput:

                    DateTime datetime = value is DateTime time1 && time1 != DateTime.MinValue ? time1 : DateTime.Now;
                    strValue = datetime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    if (datetime == DateTime.MinValue) strValue = "";
                    break;

                case EditTemplates.Password:
                    strValue = "";
                    break;

                case EditTemplates.MultiLine:
                case EditTemplates.HtmlInput:
                case EditTemplates.Label:
                case EditTemplates.NotUpdatableInput:
                    strValue = value != null ? value.ToString() : "";
                    if (mode == DisplayMode.CSV)
                        strValue = strValue.Replace("&lt;br/&gt;", ";");
                    else
                        strValue = strValue.Replace("&lt;br/&gt;", "\n").TrimEnd(new char[] { '\r', '\n' });

                    strValue = strValue.Replace("&lt;", "<");
                    strValue = strValue.Replace("&gt;", ">");
                    break;

                default:
                    if (property.Type == typeof(decimal))
                    {
                        strValue = value != null ? ((decimal)value).ToString(CultureInfo.InvariantCulture) : "0";
                    }
                    else if (property.Type == typeof(int) || property.Type == typeof(int?))
                    {
                        strValue = value != null ? ((int)value).ToString(CultureInfo.InvariantCulture) : "0";
                    }
                    else if (property.Type == typeof(long))
                    {
                        strValue = value != null ? ((long)value).ToString(CultureInfo.InvariantCulture) : "0";
                    }
                    else
                    {
                        strValue = value != null ? value.ToString() : "";
                    }
                    if (mode == DisplayMode.CSV)
                        strValue = strValue.Replace("&lt;br/&gt;", ";");
                    else
                        strValue = strValue.Replace("&lt;br/&gt;", "\n").TrimEnd(new char[] { '\r', '\n' });
                    strValue = strValue.Replace("&lt;", "<");
                    strValue = strValue.Replace("&gt;", ">");
                    break;
            }
            return strValue;
        }

        public object ToObject(object value, AdvancedProperty property, ItemBase BOItem, DisplayMode mode = DisplayMode.Print)
        {
            object strValue;

            switch (property.Common.EditTemplate)
            {
                case EditTemplates.DateInput:

                    DateTime date = value is DateTime time1 && time1 != DateTime.MinValue ? time1 : DateTime.Now;
                    strValue = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (date == DateTime.MinValue) strValue = "";
                    break;

                case EditTemplates.DateTimeInput:

                    DateTime datetime = value is DateTime time && time != DateTime.MinValue ? time : DateTime.Now;
                    strValue = datetime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    if (datetime == DateTime.MinValue) strValue = "";
                    break;

                case EditTemplates.Password:
                    strValue = "";
                    break;

                case EditTemplates.MultiLine:
                case EditTemplates.HtmlInput:
                case EditTemplates.Label:
                case EditTemplates.NotUpdatableInput:
                    strValue = value != null ? value.ToString() : "";
                    if (mode == DisplayMode.CSV)
                        strValue = strValue.ToString().Replace("&lt;br/&gt;", ";");
                    else
                        strValue = strValue.ToString().Replace("&lt;br/&gt;", "\n").TrimEnd(new char[] { '\r', '\n' });

                    strValue = strValue.ToString().Replace("&lt;", "<");
                    strValue = strValue.ToString().Replace("&gt;", ">");
                    break;

                default:
                    if (property.Type == typeof(decimal))
                    {
                        return (decimal)value;
                    }
                    else if (property.Type == typeof(int) || property.Type == typeof(int?))
                    {
                        return (int)value;
                    }
                    else if (property.Type == typeof(long))
                    {
                        return (long)value;
                    }
                    else
                    {
                        strValue = value != null ? value.ToString() : "";
                    }
                    if (mode == DisplayMode.CSV)
                        strValue = strValue.ToString().Replace("&lt;br/&gt;", ";");
                    else
                        strValue = strValue.ToString().Replace("&lt;br/&gt;", "\n").TrimEnd(new char[] { '\r', '\n' });
                    strValue = strValue.ToString().Replace("&lt;", "<");
                    strValue = strValue.ToString().Replace("&gt;", ">");
                    break;
            }

            return strValue.ToString().Replace("&nbsp;", "").Trim();
        }
    }
}
