// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Field.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The Field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;

    [Serializable]
    [Bo(ModulesAccess = (long)Modulesenum.SMI
       , DisplayName = "Fields"
       , SingleName = "Field"
       , LogRevisions = true)]
    public class Field : ItemBase
    {
        #region Constructors
        public Field()
            : base(0) { }

        public Field(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Template(Mode = Template.Name)]
        public string FieldName { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Page Page { get; set; }

        [Template(Mode = Template.MultiCheck), MultiCheck(ItemType = typeof(DisplayMode))]
        public Dictionary<long, ItemBase> DisplayModes { get; set; }

        [Template(Mode = Template.String)]
        public string PrintName { get; set; }

        [Template(Mode = Template.PermissionsSelector), Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced)]
        public long Permission { get; set; }
        #endregion

        #region Populate
        public static Dictionary<long, Field> LoadByPage(string pageObjectId)
        {
            var cmd = new SqlCommand("Field_Populate_Page", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@PageObjectId", SqlDbType.NVarChar, 50) { Value = pageObjectId });

            var fields = new Dictionary<long, Field>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var field = (Field)new Field().FromDataRow(rdr);
                    fields.Add(field.Id, field);
                }
                rdr.Close();
            }
            return fields;
        }
        #endregion
    }
}