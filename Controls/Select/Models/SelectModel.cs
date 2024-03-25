namespace Controls.Select.Models
{
    using LIB.Models;
    using LIB.Tools.BO;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;

    public class SelectModel : IDataModel
    {
        public ItemBase Value { get; set; }
        public DropDownModel DropDown { get; set; }
        public MultyCheckModel MultyCheckModel { get; set; }
        public LIB.Models.LinkModel Link { get; set; }
        public bool ReadOnly { get; set; }
        public string CssView { get; set; }
        public string CssStyleView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public LIB.AdvancedProperties.DisplayMode Mode { get; set; }
        public bool hasValue()
        {
            if (MultyCheckModel!=null)
                return MultyCheckModel.hasValue();

            if (Value == null)
                return false;

            if (Value.Id == 0)
                return false;

            return true;
        }
    }
}
