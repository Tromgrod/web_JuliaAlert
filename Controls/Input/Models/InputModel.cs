using LIB.AdvancedProperties;

namespace Controls.Input.Models
{
    using LIB.Tools.Controls;

    using Weblib.Models.Common;

    public class InputModel : IDataModel
    {
        public InputModel()
        {
            Mode = DisplayMode.Simple;
        }
        public string Value { get; set; }
        public InputType Type { get; set; }
        public TextboxModel TextBox { get; set; }
        public TextboxModel HourTextBox { get; set; }
        public TextboxModel MinutesTextBox { get; set; }
        public bool ReadOnly { get; set; }
        public string CssView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public DisplayMode Mode { get; set; }
        public bool hasValue()
        {
            if (string.IsNullOrEmpty(Value))
                return false;

            return true;
        }
    }
}
