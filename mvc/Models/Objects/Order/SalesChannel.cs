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

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Order
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Каналы продаж"
       , SingleName = "Канал продаж"
       , DoCancel = false
       , LogRevisions = true)]
    public class SalesChannel : ItemBase
    {
        #region Constructors
        public SalesChannel()
            : base(0) { }

        public SalesChannel(long id)
            : base(id) { }

        public SalesChannel(string name, SqlConnection conn = null)
            : base(name, conn: conn) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Канал продажи"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Тип канала продаж"), Template(Mode = Template.ParentDropDown)]
        public TypeSalesChannel TypeSalesChannel { get; set; }

        [Common(DisplayName = "Процентная ставка"), Template(Mode = Template.Decimal)]
        public decimal InterestRate { get; set; }

        [Common(DisplayName = "Валюта"), Template(Mode = Template.ParentDropDown)]
        public Currency Currency { get; set; }

        [Common(DisplayName = "Формула"), Template(Mode = Template.VisibleString), Access(EditableFor = (long)BasePermissionenum.SuperAdmin)]
        public string Formula { get; set; }
        #endregion

        public decimal GetEstimatedPrice(decimal factoryPrice, SalesChannelCoefficient salesChannelCoefficient)
        {
            var usdRate = ExchangeRate.GetReverseCourse(ExchangeRate.Instance[Currency.NBM_Id.USD].Value);
            var eurRate = ExchangeRate.GetReverseCourse(ExchangeRate.Instance[Currency.NBM_Id.EUR].Value);
            var rubRate = ExchangeRate.GetReverseCourse(ExchangeRate.Instance[Currency.NBM_Id.RUB].Value);

            var formula = this.Formula
                .Replace("FPrice", factoryPrice.ToString())
                .Replace("Coef", salesChannelCoefficient.Coefficient.ToString())
                .Replace("Expense", salesChannelCoefficient.Expense.ToString())
                .Replace("USD", usdRate.ToString())
                .Replace("EUR", eurRate.ToString())
                .Replace("RUB", rubRate.ToString());

            return new DataTable().Compute(formula, null) is decimal price ? price : default;
        }

        public static List<SalesChannel> PopulateSalesChannelsFormula()
        {
            var cmd = new SqlCommand("SalesChannel_PopulateSalesChannelsFormula", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var salesChannels = new List<SalesChannel>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var salesChannel = (SalesChannel)new SalesChannel().FromDataRow(rdr);
                    salesChannels.Add(salesChannel);
                }

                rdr.Close();
            }

            return salesChannels;
        }

        public static Dictionary<long, ItemBase> PopulateByMD() => PopulateByCurrencies(Currency.Enum.MDL);

        public static Dictionary<long, ItemBase> PopulateByWord() => PopulateByCurrencies(Currency.Enum.USD, Currency.Enum.EUR);

        private static Dictionary<long, ItemBase> PopulateByCurrencies(params Currency.Enum[] currencies)
        {
            var cmd = new SqlCommand("SalesChannel_PopulateByCurrencies", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var currenciesSrt = string.Concat(currencies.Select(c => (long)c + ","));
            cmd.Parameters.Add(new SqlParameter("Currencies", SqlDbType.NVarChar, 100) { Value = currenciesSrt });

            var salesChannels = new Dictionary<long, ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var salesChannel = (SalesChannel)new SalesChannel().FromDataRow(rdr);
                    salesChannels.Add(salesChannel.Id, salesChannel);
                }

                rdr.Close();
            }

            return salesChannels;
        }

        public override string GetName() => this.Name + (this.InterestRate > 0 ? $" ({this.InterestRate:F})" : string.Empty);

        public enum Enum : long
        {
            JuliaAllert = 2
        }
    }
}