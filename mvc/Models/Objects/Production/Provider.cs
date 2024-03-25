using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using LIB.Tools.Utils;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Поставщики"
       , SingleName = "Поставщика"
       , DoCancel = true
       , LogRevisions = true)]
    public class Provider : ItemBase
    {
        #region Constructors
        public Provider()
            : base(0) { }

        public Provider(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Поставщик"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Фискальный код"), Template(Mode = Template.VisibleString)]
        public string FiscalCode { get; set; }

        [Common(DisplayName = "Номер телефона"), Template(Mode = Template.VisibleString)]
        public string PhoneNumber { get; set; }
        #endregion

        public override object LoadPopupData(long itemId) => PopulateById(itemId);

        public static Provider PopulateById(long providerId, SqlConnection conn = null)
        {
            conn = conn ?? DataBase.ConnectionFromContext();
            string strSql = "SELECT * FROM Provider WHERE DeletedBy IS NULL AND ProviderId = " + providerId;

            var provider = new Provider();
            using (var rdr = new SqlCommand(strSql, conn).ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    provider.FromDataRow(rdr);

                rdr.Close();
            }
            return provider;
        }

        public override string GetName() => this.Name;
    }
}