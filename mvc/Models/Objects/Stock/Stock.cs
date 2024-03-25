using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using LIB.Tools.Utils;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Stock
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Склады"
       , SingleName = "Склад"
       , DoCancel = true
       , LogRevisions = true)]
    public class Stock : ItemBase
    {
        #region Constructors
        public Stock()
            : base(0) { }

        public Stock(long id)
            : base(id) { }

        public Stock(string name, SqlConnection conn = null)
            : base(name, conn: conn) 
        {
            Name = name;
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Название склада"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Валюта"), Template(Mode = Template.ParentDropDown)]
        public Currency Currency { get; set; }

        [Common(DisplayName = "Главный склад"), Template(Mode = Template.CheckBox), Access(EditableFor = (long)BasePermissionenum.SuperAdmin)]
        public bool IsMainStock { get; set; }
        #endregion

        public static Dictionary<long, ItemBase> PopulateByMD() => PopulateByCurrencies(Currency.Enum.MDL);

        public static Dictionary<long, ItemBase> PopulateByWord() => PopulateByCurrencies(Currency.Enum.USD, Currency.Enum.EUR);

        private static Dictionary<long, ItemBase> PopulateByCurrencies(params Currency.Enum[] currencies)
        {
            var cmd = new SqlCommand("Stock_PopulateByCurrencies", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var currenciesSrt = string.Join(",", currencies.Select(c => (long)c));
            cmd.Parameters.Add(new SqlParameter("Currencies", SqlDbType.NVarChar, 100) { Value = currenciesSrt });

            var stocks = new Dictionary<long, ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var stock = (Stock)new Stock().FromDataRow(rdr);
                    stocks.Add(stock.Id, stock);
                }

                rdr.Close();
            }

            return stocks;
        }

        public static Dictionary<long, ItemBase> GetStocksByUser()
        {
            var cmd = new SqlCommand("UserStock_GetStocksByUser", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UserId", SqlDbType.BigInt) { Value = Authentication.GetCurrentUser().Id });

            var stocks = new Dictionary<long, ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var stock = (Stock)new Stock().FromDataRow(rdr);
                    stocks.Add(stock.Id, stock);
                }

                rdr.Close();
            }

            if (stocks.Count == 0)
                stocks = new Stock().Populate();

            return stocks;
        }

        public static Stock GetMainStock()
        {
            var cmd = new SqlCommand("SELECT * FROM Stock WHERE DeletedBy IS NULL AND IsMainStock = 1", DataBase.ConnectionFromContext());

            var stock = new Stock();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    stock.FromDataRow(rdr);

                rdr.Close();
            }

            return stock;
        }
    }
}