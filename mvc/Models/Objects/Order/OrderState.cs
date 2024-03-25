using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using LIB.Tools.Utils;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Order
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Статусы заказов"
       , SingleName = "Статус заказа"
       , DoCancel = true
       , LogRevisions = true)]
    public class OrderState : AggregateBase
    {
        #region Constructors
        public OrderState()
            : base(0) { }
        public OrderState(long id)
            : base(id) { }

        public OrderState(string name, SqlConnection conn = null)
            : base(name, conn)
        {
            Name = name;
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Статус заказа"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion

        #region Methods
        public static List<ChartDataInfo> LoadOrserPerOrderState(params Currency.Enum[] currencies)
        {
            var cmd = new SqlCommand("OrderState_Populate_ByAllOrder", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var currenciesSrt = string.Concat(currencies.Select(c => (long)c + ","));
            cmd.Parameters.Add(new SqlParameter("Currencies", SqlDbType.NVarChar, 100) { Value = currenciesSrt });

            var ChartDataInfos = new List<ChartDataInfo>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var orderState = new OrderState();
                    orderState.FromDataRow(rdr);

                    var ChartDataInfo = new ChartDataInfo(orderState.Id, orderState.Name, orderState.Count, orderState.Color);
                    ChartDataInfos.Add(ChartDataInfo);
                }

                rdr.Close();
            }

            return ChartDataInfos;
        }

        public static List<ChartDataInfo> LoadLocalOrserPerOrderState() => LoadOrserPerOrderState(Currency.Enum.MDL);

        public static List<ChartDataInfo> LoadOrserPerOrderState() => LoadOrserPerOrderState(Currency.Enum.USD, Currency.Enum.EUR);
        #endregion

        public enum Enum : long
        {
            None = 0,
            Accepted = 1,
            Delivered = 2,
            Sent = 3,
            RefundClaimed = 4,
            CargoInMD = 5,
            Return = 6,
            DryCleaning = 7,
            Repair = 8,
            Paid = 9,
            PartialReturn = 10,
            Canceled = 11
        }
    }
}