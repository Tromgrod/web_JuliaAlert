using System;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Model
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Составы материалов"
       , SingleName = "Состав материала"
       , DoCancel = true
       , LogRevisions = true)]
    public class Compound : ItemBase
    {
        #region Constructors
        public Compound()
            : base(0) { }

        public Compound(long id)
            : base(id) { }

        public Compound(string name, SqlConnection conn)
            : base(name, conn: conn) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Состав материала"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}