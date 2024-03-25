using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Data.SqlClient;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Model
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Декоры"
       , SingleName = "Декор"
       , DoCancel = true
       , LogRevisions = true)]
    public class Decor : ItemBase
    {
        #region Constructors
        public Decor()
            : base(0) { }

        public Decor(long id)
            : base(id) { }

        public Decor(string code, SqlConnection conn = null)
            : base(code, nameof(Code), true, conn)
        {
            Code = code;
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Декор"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Код декора"), Template(Mode = Template.VisibleString), Db(Sort = DbSortMode.Asc)]
        public string Code { get; set; }
        #endregion

        public override string GetName() => this.Name + (string.IsNullOrEmpty(this.Code) ? "" : " (" + this.Code + ")");
    }
}