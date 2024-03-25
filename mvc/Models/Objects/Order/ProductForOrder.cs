using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.BusinessObjects;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class ProductForOrder : ItemBase
    {
        #region Constructors
        public ProductForOrder()
            : base(0) { }

        public ProductForOrder(long id)
            : base(id) { }

        public ProductForOrder(SpecificProduct specificProduct, Order order, SqlConnection conn = null)
            : base(0) 
        {
            this.SpecificProduct = specificProduct;
            this.Order = order;

            var cmd = new SqlCommand($"SELECT ProductForOrderId FROM ProductForOrder WHERE SpecificProductId = {specificProduct.Id} AND OrderId = {order.Id}", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };

            var IdDB = cmd.ExecuteScalar();

            Id = IdDB != DBNull.Value && IdDB != null ? Convert.ToInt64(IdDB) : 0;
        }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SpecificProduct SpecificProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Order Order { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal FactoryPrice { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Price { get; set; }

        [Template(Mode = Template.Number)]
        public int Discount { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal FinalPrice { get; set; }
        #endregion

        #region Override methods
        public override string GetName() => "ProductForOrderId";

        public override string GetCaption() => string.Empty;

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long OrderId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByOrder(OrderId);
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var sizeId = long.Parse(HttpContext.Current.Request.Form[nameof(ProductSize)]);
            var uniqueProductId = long.Parse(HttpContext.Current.Request.Form[nameof(UniqueProduct)]);

            this.SpecificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, sizeId);
        }

        public override object LoadPopupData(long itemId) => PopulateReturnProduct(itemId);

        public override void ManyCollectFromForm(int index, string prefix = "")
        {
            long.TryParse(HttpContext.Current.Request.Form[nameof(UniqueProduct)].Split(',')[index], out var UniqueProductId);
            long.TryParse(HttpContext.Current.Request.Form[nameof(ProductSize)].Split(',')[index], out var ProductSizeId);
            long.TryParse(HttpContext.Current.Request.Form[nameof(this.Order)].Split(',')[index], out var OrderId);
            int.TryParse(HttpContext.Current.Request.Form[nameof(this.Count)].Split(',')[index], out var Count);
            decimal.TryParse(HttpContext.Current.Request.Form[nameof(this.Price)].Split(',')[index], out var Price);
            int.TryParse(HttpContext.Current.Request.Form[nameof(this.Discount)].Split(',')[index], out var Discount);
            decimal.TryParse(HttpContext.Current.Request.Form[nameof(this.FinalPrice)].Split(',')[index], out var FinalPrice);

            this.SpecificProduct = SpecificProduct.GetByUniqueProductAndSize(UniqueProductId, ProductSizeId);
            this.Order = new Order(OrderId);
            this.Count = Count;
            this.FactoryPrice = this.SpecificProduct.UniqueProduct.LastFactoryPrice;
            this.Price = Price;
            this.Discount = Discount;
            this.FinalPrice = FinalPrice;
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Insert;

            var productForOrder = (ProductForOrder)item;

            var order = Order.PopulateById(productForOrder.Order.Id, connection);

            SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, order.Stock, -productForOrder.Count, order.OrderDate, actionType);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var productForOrder = (ProductForOrder)item;

            var order = Order.PopulateById(productForOrder.Order.Id, connection);

            var productForOrderFromDB = PopulateById(productForOrder.Id);

            SpecificProductStock.UpdateCountInStock(productForOrderFromDB.SpecificProduct, order.Stock, productForOrderFromDB.Count - productForOrder.Count, order.OrderDate, actionType);

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            foreach (ProductForOrder item in dictionary.Values)
            {
                var productForOrder = item;

                if (productForOrder.SpecificProduct == null || productForOrder.Count == default)
                {
                    productForOrder = PopulateById(productForOrder.Id);
                }

                var order = Order.PopulateById(productForOrder.Order.Id);

                SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, order.Stock, productForOrder.Count, order.OrderDate, actionType);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
        #endregion

        #region Populate
        public bool IsReturn()
        {
            if (this.Id > 0)
            {
                var cmd = new SqlCommand($"SELECT COUNT(ReturnId) FROM [Return] WHERE ProductForOrderId = {this.Id} AND DeletedBy IS NULL", DataBase.ConnectionFromContext());

                return cmd.ExecuteScalar() is int count && count > 0;
            }

            return false;
        }

        public static Return PopulateReturnProduct(long productForOrderId)
        {
            var cmd = new SqlCommand("ProductForOrder_PopulateReturnProduct", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ProductForOrderId", SqlDbType.BigInt) { Value = productForOrderId });

            var @return = new Return();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    @return.FromDataRow(rdr);

                rdr.Close();
            }
            return @return;
        }

        public static Dictionary<DateTime, int> PopulateCountByYear(int Year, SalesChannel salesChannel = null)
        {
            var cmd = new SqlCommand("Populate_ProductForOrder_CountByYear", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("Year", SqlDbType.Int) { Value = Year });
            if (salesChannel != null)
                cmd.Parameters.Add(new SqlParameter("SalesChannelId", SqlDbType.BigInt) { Value = salesChannel.Id });

            var ds = new DataSet();
            var da = new SqlDataAdapter { SelectCommand = cmd };
            da.Fill(ds);

            var counts = new Dictionary<DateTime, int>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var orderDate = (DateTime)dr[nameof(ProductForOrder.Order.OrderDate)];
                var count = (int)dr[nameof(ProductForOrder.Count)];

                counts.Add(orderDate, count);
            }
            return counts;
        }

        public static Dictionary<long, ItemBase> PopulateByOrder(long OrderId)
        {
            var cmd = new SqlCommand("ProductForOrder_Populate_ByOrder", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("OrderId", SqlDbType.BigInt) { Value = OrderId });

            var ProductForOrders = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var ProductForOrder = (ProductForOrder)(new ProductForOrder().FromDataRow(rdr));
                    ProductForOrders.Add(ProductForOrder.Id, ProductForOrder);
                }
                rdr.Close();
            }
            return ProductForOrders;
        }

        public static ProductForOrder PopulateById(long productForOrderId, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("ProductForOrder_PopulateById", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ProductForOrderId", SqlDbType.BigInt) { Value = productForOrderId });

            var productForOrder = new ProductForOrder();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    productForOrder.FromDataRow(rdr);

                rdr.Close();
            }
            return productForOrder;
        }
        #endregion
    }
}