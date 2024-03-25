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
    [Serializable] 
    [Bo(DoCancel = true, LogRevisions = true)]
    public class MovingProductUnit : ItemBase
    {
        #region Constructors
        public MovingProductUnit()
            : base(0) { }

        public MovingProductUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public MovingProduct MovingProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Stock StockFrom { get; set; }

        [Template(Mode = Template.Number),
         Db(_Ignore = true)]
        public int StockFromCount { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Stock StockTo { get; set; }

        [Template(Mode = Template.Number),
         Db(_Ignore = true)]
        public int StockToCount { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public SpecificProduct SpecificProduct { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }
        #endregion

        public override string GetCaption() => nameof(Count);

        public override string GetName() => this.Count.ToString();

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long movingProductId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(movingProductId);
        }

        public static MovingProductUnit PopulateById(long movingProductUnitId)
        {
            var cmd = new SqlCommand("MovingProductUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("MovingProductUnitId", SqlDbType.BigInt) { Value = movingProductUnitId });

            var movingProductUnit = new MovingProductUnit();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    movingProductUnit.FromDataRow(rdr);

                rdr.Close();
            }
            return movingProductUnit;
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long movingProductId)
        {
            var cmd = new SqlCommand("MovingProductUnit_PopulateByParentId", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("MovingProductId", SqlDbType.BigInt) { Value = movingProductId });

            var movingProductUnits = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var movingProductUnit = (MovingProductUnit)new MovingProductUnit().FromDataRow(rdr);

                    movingProductUnit.StockFromCount = Convert.ToInt32(rdr["StockFromCount"]);
                    movingProductUnit.StockToCount = Convert.ToInt32(rdr["StockToCount"]);

                    movingProductUnits.Add(movingProductUnit.Id, movingProductUnit);
                }
                rdr.Close();
            }
            return movingProductUnits;
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Insert;

            var movingProductUnit = (MovingProductUnit)item;
            var movingProduct = MovingProduct.PopulateById(movingProductUnit.MovingProduct.Id);

            SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, -movingProductUnit.Count, movingProduct.Date, actionType);
            SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, movingProductUnit.Count, movingProduct.Date, actionType);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var movingProductUnit = (MovingProductUnit)item;

            var movingProductUnitFromDB = PopulateById(movingProductUnit.Id);

            if (movingProductUnit.SpecificProduct.Id != movingProductUnitFromDB.SpecificProduct.Id)
                return;

            if (movingProductUnitFromDB.StockFrom.Id == movingProductUnit.StockFrom.Id &&
                movingProductUnitFromDB.StockTo.Id == movingProductUnit.StockTo.Id)
            {
                var newCount = movingProductUnit.Count - movingProductUnitFromDB.Count;

                SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, -newCount, movingProductUnitFromDB.MovingProduct.Date, actionType);
                SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, newCount, movingProductUnitFromDB.MovingProduct.Date, actionType);
            }
            else
            {
                if (movingProductUnitFromDB.StockFrom.Id != movingProductUnit.StockFrom.Id)
                {
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnitFromDB.StockFrom, movingProductUnitFromDB.Count, movingProductUnitFromDB.MovingProduct.Date, actionType);
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, -movingProductUnit.Count, movingProductUnitFromDB.MovingProduct.Date, actionType);
                }
                if (movingProductUnitFromDB.StockTo.Id != movingProductUnit.StockTo.Id)
                {
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnitFromDB.StockTo, -movingProductUnitFromDB.Count, movingProductUnitFromDB.MovingProduct.Date, actionType);
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, movingProductUnit.Count, movingProductUnitFromDB.MovingProduct.Date, actionType);
                }
            }

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            foreach (ItemBase item in dictionary.Values)
            {
                var movingProductUnit = GetFullMovingProductUnit(item);

                SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, movingProductUnit.Count, movingProductUnit.MovingProduct.Date, actionType);
                SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, -movingProductUnit.Count, movingProductUnit.MovingProduct.Date, actionType);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }

        private static MovingProductUnit GetFullMovingProductUnit(ItemBase movingProductUnitItem)
        {
            var movingProductUnit = (MovingProductUnit)movingProductUnitItem;

            if (movingProductUnit.SpecificProduct == null ||
                movingProductUnit.StockFrom == null ||
                movingProductUnit.StockTo == null ||
                movingProductUnit.MovingProduct == null ||
                movingProductUnit.MovingProduct.Date == default)
            {
                movingProductUnit = PopulateById(movingProductUnit.Id);
            }

            return movingProductUnit;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var uniqueProductId = long.Parse(HttpContext.Current.Request.Form[nameof(UniqueProduct)]);
            var productSizeId = long.Parse(HttpContext.Current.Request.Form[nameof(ProductSize)]);

            this.SpecificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, productSizeId);
        }
    }
}