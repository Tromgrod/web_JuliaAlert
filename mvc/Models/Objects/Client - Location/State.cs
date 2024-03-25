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
       , DisplayName = "Штаты"
       , SingleName = "Штат"
       , DoCancel = true
       , LogRevisions = true)]
    public class State : ItemBase
    {
        #region Constructors
        public State()
            : base(0) { }

        public State(long id)
            : base(id) { }

        public State(string name, string shortName, SqlConnection conn = null)
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
        [Common(DisplayName = "Штат"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Аббревиатура штата"), Template(Mode = Template.Name)]
        public string ShortName { get; set; }
        #endregion

        public override string GetName() => Name + (!string.IsNullOrEmpty(ShortName) ? " (" + ShortName + ")" : "");

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Populate_State", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            var States = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var State = (State)new State().FromDataRow(dr);
                    States.Add(State.Id, State);
                }
                dr.Close();
            }

            return States;
        }

        public enum Enum : long
        {
            None,
            Chisinau = 63
        }
    }
}