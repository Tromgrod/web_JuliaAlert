using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.BusinessObjects;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class SupplySpecificProductUnit : ItemBase
    {
        #region Constructors
        public SupplySpecificProductUnit()
            : base(0) { }

        public SupplySpecificProductUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplySpecificProduct SupplySpecificProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public SpecificProduct SpecificProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Factory FactoryCut { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal CutCost { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal FactoryPrice { get; set; }

        [Db(_Ignore = true)]
        public int TotalImplementCount => GetTotalImplementCount();

        [Db(_Ignore = true)]
        public int TotalTailoringCount => GetTotalTailoringCount();
        #endregion

        public override string GetCaption() => nameof(SupplySpecificProduct) + nameof(SupplySpecificProduct.Id);

        public override string GetName() => this.SupplySpecificProduct.Id.ToString();

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long supplySpecificProductId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(supplySpecificProductId);
        }

        public static SupplySpecificProductUnit PopulateById(long supplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = supplySpecificProductUnitId });

            var supplySpecificProductUnit = new SupplySpecificProductUnit();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    supplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }

            return supplySpecificProductUnit;
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long supplySpecificProductId)
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_PopulateByParentId", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductId", SqlDbType.BigInt) { Value = supplySpecificProductId });

            var supplySpecificProductUnits = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplySpecificProductUnit = (SupplySpecificProductUnit)(new SupplySpecificProductUnit().FromDataRow(rdr));
                    supplySpecificProductUnits.Add(supplySpecificProductUnit.Id, supplySpecificProductUnit);
                }
                rdr.Close();
            }
            return supplySpecificProductUnits;
        }

        public static SupplySpecificProductUnit GetLastByParrent(UniqueProduct uniqueProduct)
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_GetLastByParrent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = uniqueProduct.Id });

            var supplySpecificProductUnit = new SupplySpecificProductUnit();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    supplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }
            return supplySpecificProductUnit;
        }

        public static List<ImplementSupplySpecificProductUnit> PopulateImplementSupplySpecificProductUnits(long supplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_PopulateImplementSupplySpecificProductUnits", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = supplySpecificProductUnitId });

            var implementSupplySpecificProductUnits = new List<ImplementSupplySpecificProductUnit>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var implementSupplySpecificProductUnit = (ImplementSupplySpecificProductUnit)new ImplementSupplySpecificProductUnit().FromDataRow(rdr);

                    implementSupplySpecificProductUnits.Add(implementSupplySpecificProductUnit);
                }

                rdr.Close();
            }

            return implementSupplySpecificProductUnits;
        }

        public static List<TailoringSupplySpecificProductUnit> PopulateTailoringSupplySpecificProductUnits(long supplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_PopulateTailoringSupplySpecificProductUnits", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = supplySpecificProductUnitId });

            var tailoringSupplySpecificProductUnits = new List<TailoringSupplySpecificProductUnit>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var tailoringSupplySpecificProductUnit = (TailoringSupplySpecificProductUnit)new TailoringSupplySpecificProductUnit().FromDataRow(rdr);

                    tailoringSupplySpecificProductUnits.Add(tailoringSupplySpecificProductUnit);
                }

                rdr.Close();
            }

            return tailoringSupplySpecificProductUnits;
        }

        private int GetTotalImplementCount()
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_GetTotalImplementCount", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = this.Id });

            var countObj = cmd.ExecuteScalar();

            return countObj != DBNull.Value ? Convert.ToInt32(countObj) : default;
        }

        private int GetTotalTailoringCount()
        {
            var cmd = new SqlCommand("SupplySpecificProductUnit_GetTotalTailoringCount", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = this.Id });

            var countObj = cmd.ExecuteScalar();

            return countObj != DBNull.Value ? Convert.ToInt32(countObj) : default;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var uniqueProductId = long.Parse(HttpContext.Current.Request.Form[nameof(UniqueProduct)]);
            var productSizeId = long.Parse(HttpContext.Current.Request.Form[nameof(ProductSize)]);

            this.SpecificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, productSizeId);
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var supplySpecificProductUnit = (SupplySpecificProductUnit)item;

            var specificProduct = SpecificProduct.PopulateById(supplySpecificProductUnit.SpecificProduct.Id);

            this.FactoryPrice = specificProduct.UniqueProduct.GetProductionPrice();

            UpdateTextileColorCount(specificProduct, -supplySpecificProductUnit.Count);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var supplySpecificProductUnit = (SupplySpecificProductUnit)item;

            var supplySpecificProductUnitFromDB = PopulateById(supplySpecificProductUnit.Id);

            var specificProduct = SpecificProduct.PopulateById(supplySpecificProductUnitFromDB.SpecificProduct.Id);

            UpdateTextileColorCount(specificProduct, supplySpecificProductUnitFromDB.Count - supplySpecificProductUnit.Count);

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            var stock = Stock.GetMainStock();

            foreach (SupplySpecificProductUnit supplySpecificProductUnit in dictionary.Values)
            {
                var specificProduct = SpecificProduct.PopulateById(PopulateById(supplySpecificProductUnit.Id).SpecificProduct.Id);

                UpdateTextileColorCount(specificProduct, supplySpecificProductUnit.Count);

                foreach (ImplementSupplySpecificProductUnit implementSupplySpecificProductUnit in PopulateImplementSupplySpecificProductUnits(supplySpecificProductUnit.Id))
                {
                    SpecificProductStock.UpdateCountInStock(implementSupplySpecificProductUnit.SupplySpecificProductUnit.SpecificProduct, stock, - implementSupplySpecificProductUnit.Count, implementSupplySpecificProductUnit.SupplySpecificProductUnit.SupplySpecificProduct.Date, actionType);
                }
            }

            return base.Delete(dictionary, Comment, connection, user);
        }

        private static void UpdateTextileColorCount(SpecificProduct specificProduct, int updatedCount)
        {
            var consumptionTextiles = СonsumptionTextile.PopulateByParentId(specificProduct.UniqueProduct.Id);

            foreach (СonsumptionTextile consumptionTextile in consumptionTextiles.Values)
            {
                var textileColor = TextileColor.PopulateById(consumptionTextile.TextileColor.Id);

                textileColor.UpdateProperties(nameof(textileColor.CurrentCount), textileColor.CurrentCount + (consumptionTextile.Consumption * updatedCount));
            }
        }
    }
}