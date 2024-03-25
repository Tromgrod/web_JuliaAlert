namespace Controls.Link.Models
{
    using LIB.Tools.Controls;

    using Weblib.Models.Common;

    public class LinkModel : IDataModel
    {
        public LIB.Models.LinkModel Link { get; set; }
        public bool ReadOnly { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string CssView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public LIB.AdvancedProperties.DisplayMode Mode { get; set; }
        public bool hasValue()
        {
            return false;
        }
    }
}
