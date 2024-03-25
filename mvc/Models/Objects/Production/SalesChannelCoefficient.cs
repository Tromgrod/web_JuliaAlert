using System;
using System.Data;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Коэффициенты каналов<br> продаж"
       , SingleName = "Коэффициент канала продаж"
       , DoCancel = true
       , EditAccess = (long)BasePermissionenum.SuperAdmin
       , LogRevisions = true)]
    public class SalesChannelCoefficient : ItemBase
    {
        #region Constructors
        public SalesChannelCoefficient()
            : base(0) { }

        public SalesChannelCoefficient(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Канал продаж"), Template(Mode = Template.ParentDropDown)]
        public SalesChannel SalesChannel { get; set; }

        [Common(DisplayName = "Расход"), Template(Mode = Template.Decimal)]
        public decimal Expense { get; set; }

        [Common(DisplayName = "Коэффициент"), Template(Mode = Template.Decimal)]
        public decimal Coefficient { get; set; }
        #endregion

        public static SalesChannelCoefficient GetLastBySalesChannel(SalesChannel salesChannel)
        {
            var cmd = new SqlCommand($"SELECT TOP(1) * FROM SalesChannelCoefficient WHERE SalesChannelId = {salesChannel.Id} ORDER BY DateCreated DESC", DataBase.ConnectionFromContext());

            var salesChannelCoefficient = new SalesChannelCoefficient();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    salesChannelCoefficient.FromDataRow(rdr);

                rdr.Close();
            }
            return salesChannelCoefficient;
        }
    }
}