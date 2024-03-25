using LIB.Models.Common;

namespace Weblib.Models.Common
{
    public class Icon : iBaseControlModel
    {
        public string Class { get; set; }

        public static Icon Search = new Icon { Class = "btn-search" };
        public static Icon Add = new Icon { Class = "btn-add" };
        public static Icon Logout = new Icon { Class = "btn-logout" };
    }
}