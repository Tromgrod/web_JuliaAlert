using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Data.SqlClient;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Geography
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Города"
       , SingleName = "Город"
       , DoCancel = true
       , LogRevisions = true)]
    public class City : ItemBase
    {
        #region Constructors
        public City()
            : base(0) { }

        public City(long id)
            : base(id) { }

        public City(string name, SqlConnection conn = null)
            : base(name, throwException: false, conn: conn)
        {
            if (this.Id <= 0 && !string.IsNullOrEmpty(name))
            {
                Name = name;

                this.Insert(this, connection: conn);
            }
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Город"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion

        public enum Enum : long
        {
            None,
            Chisinau = 1701
        }
    }
}