using System;
using System.Data.SqlClient;
using LIB.AdvancedProperties;

namespace LIB.Tools.BO
{
    [Serializable]
    public class AggregateBase: ItemBase
    {
        #region Constructors
        public AggregateBase()
            : base(0) { }

        public AggregateBase(long id)
            : base(id) { }

        public AggregateBase(string name, SqlConnection conn = null)
            : base(name, conn: conn) { }
        #endregion

        [Common(DisplayName = "Цвет"), Template(Mode = Template.ColorPicker), Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public string Color { get; set; }

        [Common(EditTemplate = EditTemplates.Hidden), Db(_Editable = false, _Populate = false)]
        public int Count { get; set; }

        public virtual string GetStyleColor() => $"background: {this.Color};";
    }
}