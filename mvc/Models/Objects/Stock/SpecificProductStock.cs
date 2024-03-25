using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class SpecificProductStock : ItemBase
    {
        #region Constructors
        public SpecificProductStock()
            : base(0) { }

        public SpecificProductStock(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SpecificProduct SpecificProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Stock Stock { get; set; }

        [Template(Mode = Template.Number)]
        public int CurrentCount { get; set; }
        #endregion

        public static List<ItemBase> LoadMainStockList() => LoadList(true);

        public static List<ItemBase> LoadSubsidiaryStockList() => LoadList(false);

        public static List<ItemBase> LoadAllStockList() => LoadList(null);

        private static List<ItemBase> LoadList(bool? isMainStock = null)
        {
            var cmd = new SqlCommand("SpecificProductStock_LoadList", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            if (isMainStock.HasValue)
                cmd.Parameters.Add(new SqlParameter("IsMainStock", SqlDbType.Bit) { Value = isMainStock.Value });

            var stocks = Stock.GetStocksByUser();
            var stocksStr = string.Join(",", stocks.Values.Select(s => s.Id));

            cmd.Parameters.Add(new SqlParameter("StockIds", SqlDbType.NVarChar, 100) { Value = stocksStr });

            var specificProductStocks = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var specificProductStock = (SpecificProductStock)new SpecificProductStock().FromDataRow(rdr);

                    specificProductStocks.Add(specificProductStock);
                }

                rdr.Close();
            }

            return specificProductStocks;
        }

        public static SpecificProductStock GetBySpecificProduct(SpecificProduct specificProduct, Stock stock)
        {
            var cmd = new SqlCommand($"SELECT * FROM SpecificProductStock WHERE SpecificProductId = {specificProduct.Id} AND StockId = {stock.Id}", DataBase.ConnectionFromContext());

            var specificProductStock = new SpecificProductStock();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    specificProductStock.FromDataRow(dr);

                dr.Close();
            }

            return specificProductStock;
        }

        public static SpecificProductStock GetByMainStock(SpecificProduct specificProduct)
        {
            var cmd = new SqlCommand(
                "SELECT sps.* FROM SpecificProductStock sps " +
                "JOIN Stock s ON s.StockId = sps.StockId AND s.IsMainStock = 1 " +
                $"WHERE sps.SpecificProductId = {specificProduct.Id}", DataBase.ConnectionFromContext());

            var specificProductStock = new SpecificProductStock();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    specificProductStock.FromDataRow(dr);

                dr.Close();
            }

            return specificProductStock;
        }

        public static void UpdateCountInStock(SpecificProduct specificProduct, Stock stock, int count, DateTime date, SpecificProductStockHistory.ActionTypeEnum actionType)
        {
            var stacktrace = new StackTrace();
            var prevframe = stacktrace.GetFrame(1);
            var method = prevframe.GetMethod();

            var specificProductStock = GetBySpecificProduct(specificProduct, stock);

            if (specificProductStock.Id <= 0)
            {
                specificProductStock = new SpecificProductStock()
                {
                    SpecificProduct = specificProduct,
                    Stock = stock,
                    CurrentCount = count
                };

                specificProductStock.Insert(specificProductStock);
            }
            else
            {
                specificProductStock.UpdateProperties(nameof(specificProductStock.CurrentCount), specificProductStock.CurrentCount + count);
            }

            SpecificProductStockHistory.Insert(specificProductStock, count, date, actionType, method.ReflectedType.Name);
        }
    }
}