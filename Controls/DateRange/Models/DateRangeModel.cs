namespace Controls.DateRange.Models
{
    using LIB.Tools.Controls;
    using System;
    using Weblib.Models.Common;

    public class DateRangeModel : IDataModel
    {
        public DateRange Value { get; set; }
        public TextboxModel TextBoxFrom { get; set; }
        public TextboxModel TextBoxTo { get; set; }
        public bool ShowTime { get; set; }
        public bool ReadOnly { get; set; }
        public string CssView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public LIB.AdvancedProperties.DisplayMode Mode { get; set; }
        public bool hasValue()
        {
            if (Value == null)
                return false;

            if (Value.From == DateTime.MinValue && Value.To == DateTime.MinValue)
                return false;

            return true;
        }
    }
}
