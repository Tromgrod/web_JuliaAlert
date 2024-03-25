namespace Controls.Image.Models
{
    using LIB.BusinessObjects;
    using LIB.Tools.Controls;
    using Weblib.Models.Common;

    public class ImageModel : IDataModel
    {
        public Graphic Value { get; set; }
        public int AdminThumbnailWidth { get; set; }
        public int AdminThumbnailHeight { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public string BOName { get; set; }
        public string PropertyName { get; set; }
        public bool ReadOnly { get; set; }
        public string CssView { get; set; }
        public string CssEdit { get; set; }
        public string Label { get; set; }
        public LIB.AdvancedProperties.DisplayMode Mode { get; set; }
        public string UniqueId { get; set; }

        public bool hasValue()
        {
            if (Value==null)
                return false;

            return true;
        }
    }
}
