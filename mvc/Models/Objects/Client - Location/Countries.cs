using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Collections.Generic;
using System.Data.SqlClient;
using LIB.Tools.Utils;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Geography
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Страны"
       , SingleName = "Страну"
       , DoCancel = true
       , LogRevisions = true)]
    public class Countries : ItemBase
    {
        #region Constructors
        public Countries()
            : base(0) { }

        public Countries(long id)
            : base(id) { }

        public Countries(string name, string shortName, SqlConnection conn = null)
            : base(name, throwException: false, conn: conn)
        {
            if (this.Id <= 0 && !string.IsNullOrEmpty(name))
            {
                Name = name;
                ShortName = shortName;

                this.Insert(this, connection: conn);
            }
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Старана"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Аббревиатура страны"), Template(Mode = Template.Name)]
        public string ShortName { get; set; }
        #endregion

        public override string GetName() => Name + (!string.IsNullOrEmpty(ShortName) ? " (" + ShortName + ")" : "");

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Populate_Countries", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            var Countries = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var Country = (Countries)new Countries().FromDataRow(dr);
                    Countries.Add(Country.Id, Country);
                }
                dr.Close();
            }

            return Countries;
        }

        public enum Enum : long
        {
            None,
            Moldova = 647
        }
    }
}