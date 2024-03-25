using LIB.Models.Common;

namespace LIB.Models
{
    public class LinkModel : iBaseControlModel
    {
        public string Caption { get; set; }

        public string Href { get; set; }

        public string Class { get; set; }

        public string Action { get; set; }

        public string Value { get; set; }
}
}