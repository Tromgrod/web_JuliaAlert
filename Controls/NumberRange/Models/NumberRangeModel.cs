namespace Controls.NumberRange.Models
{
    using LIB.Tools.Controls;

    using Weblib.Models.Common;

    public class NumberRangeModel : IDataModel
    {
        public string Value { get; set; }

        public TextboxModel TextBoxFrom { get; set; }
        public TextboxModel TextBoxTo { get; set; }
        public bool ReadOnly { get; set; }
        public string CssView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public string ReportModifyFunc { get; set; }
        public LIB.AdvancedProperties.DisplayMode Mode { get; set; }
        public bool hasValue()
        {
            if (string.IsNullOrEmpty(Value))
                return false;

            return true;
        }
    }
}
