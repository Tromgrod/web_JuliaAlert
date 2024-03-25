using System;
using System.Web;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Security;
using LIB.Helpers;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class Order : ModelBase
    {
        #region Constructors
        public Order()
            : base(0) { }

        public Order(long id)
            : base(id) { }

        public Order(string orderNumber, SqlConnection conn = null)
            : base(orderNumber, nameof(OrderNumber), false, conn) => OrderNumber = orderNumber;
        #endregion

        #region Properties
        [Common(DisplayName = "Номер заказа"), Template(Mode = Template.Name)]
        public string OrderNumber { get; set; }

        [Common(DisplayName = "Номер Invoice"), Template(Mode = Template.Name)]
        public string InvoiceNumber { get; set; }

        [Common(DisplayName = "Статус заказа"), Template(Mode = Template.ParentDropDown)]
        public OrderState OrderState { get; set; }

        [Common(DisplayName = "Канал продаж"), Template(Mode = Template.ParentDropDown)]
        public SalesChannel SalesChannel { get; set; }

        [Common(DisplayName = "Способ доставки"), Template(Mode = Template.ParentDropDown)]
        public DeliveryMethod DeliveryMethod { get; set; }

        [Common(DisplayName = "Стоимость доставки"), Template(Mode = Template.Decimal)]
        public decimal Delivery { get; set; }

        [Common(DisplayName = "Налог"), Template(Mode = Template.Decimal)]
        public decimal TAX { get; set; }

        [Common(DisplayName = "Номер отслеживания"), Template(Mode = Template.String)]
        public string TrackingNumber { get; set; }

        [Common(DisplayName = "Дата оформления заказа"), Template(Mode = Template.Date)]
        public DateTime OrderDate { get; set; }

        [Common(DisplayName = "Дата отправки"), Template(Mode = Template.Date)]
        public DateTime DepartureDate { get; set; }

        [Common(DisplayName = "Дата получения"), Template(Mode = Template.Date)]
        public DateTime ReceivingDate { get; set; }

        [Common(DisplayName = "Имя клиента"), Template(Mode = Template.ParentDropDown)]
        public Client Client { get; set; }

        [Common(DisplayName = "Описание"), Template(Mode = Template.Html)]
        public string Description { get; set; }

        [Common(DisplayName = "Склад"), Template(Mode = Template.ParentDropDown)]
        public Stock Stock { get; set; }

        [Template(Mode = Template.LinkItems), LinkItem(LinkType = typeof(ProductForOrder))]
        public Dictionary<long, ItemBase> ProductsForOrder { get; set; }

        [Template(Mode = Template.LinkItems), LinkItem(LinkType = typeof(Gift))]
        public Dictionary<long, ItemBase> Gifts { get; set; }

        [Db(_Ignore = true)]
        public decimal Price { get; set; }
        #endregion

        #region Override Methods
        public override string GetCaption() => nameof(this.OrderNumber);

        public override string GetName() => this.OrderNumber;

        public override bool HaveAccess(string fullModel, string Id)
        {
            var currentUser = Authentication.GetCurrentUser();

            if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales))
                return true;
            else if (fullModel.Contains("Local_") && currentUser.HasAtLeastOnePermission((long)BasePermissionenum.MDAccess))
                return true;

            return false;
        }

        public override bool HaveSaveAccess(string fullModel = null, string Id = null)
        {
            var currentUser = Authentication.GetCurrentUser();

            var modelName = HttpContext.Current.Request.Form["Object"];

            if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales))
                return true;
            else if (modelName.Contains("Local_") && currentUser.HasAtLeastOnePermission((long)BasePermissionenum.MDAccess))
                return true;

            return false;
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Добавить новый заказ", Href = URLHelper.GetUrl("DocControl/" + viewModel), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override object PopulatePrintData()
        {
            var order = PopulateById(this.Id);

            var deliveryStr = HttpContext.Current.Request.Form[nameof(this.Delivery)];
            var taxStr = HttpContext.Current.Request.Form[nameof(this.TAX)];

            if (decimal.TryParse(deliveryStr, out decimal delivery) && decimal.TryParse(taxStr, out decimal tax))
            {
                order.Delivery = delivery;
                order.TAX = tax;

                order.Update(order);
            }

            return order;
        }

        public override object LoadPopupData(long itemId) => PopulateById(itemId);

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null) => PopulateById(this.Id, conn);

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            this.Client.CollectFromForm();

            if (this.Client.Id <= 0)
            {
                this.Client.Id = this.Client.GetIdByName();

                if (this.Client.Id > 0 && this.Client.GetCurrency() != null)
                {
                    throw new Exception($"Клиент {this.Client.Name} уже существует в заказах с другой валютой!");
                }
                else if (this.Client.Id > 0)
                {
                    return;
                }
            }

            this.Client.SaveForm();
        }

        public override RequestResult SaveForm()
        {
            if ((this.DepartureDate != DateTime.MinValue && this.OrderDate > this.DepartureDate) ||
                (this.ReceivingDate != DateTime.MinValue && this.DepartureDate > this.ReceivingDate))
            {
                return new RequestResult() { Result = RequestResultType.Fail, Message = "Ошибка в датах" };
            }

            var Object = HttpContext.Current.Request.Form["Object"];

            if (this.Id > 0)
            {
                this.Update(this);

                return new RequestResult() { Result = RequestResultType.Reload, RedirectURL = URLHelper.GetUrl("DocControl/" + Object + "/" + Id.ToString()), Message = "Заказ обновлен" };
            }

            this.Insert(this);

            return new RequestResult() { Result = RequestResultType.Reload, RedirectURL = URLHelper.GetUrl("DocControl/" + Object + "/" + Id.ToString()), Message = "Заказ создан" };
        }

        public override void Update(ItemBase item, LIB.AdvancedProperties.DisplayMode DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var order = (Order)item;

            var orderFromDB = PopulateById(order.Id);

            if (order.OrderDate.Date != orderFromDB.OrderDate.Date ||
                order.Stock.Id != orderFromDB.Stock.Id)
            {
                foreach (ProductForOrder productForOrder in ProductForOrder.PopulateByOrder(order.Id).Values)
                {
                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, order.Stock, productForOrder.Count, order.OrderDate, actionType);

                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, orderFromDB.Stock, -productForOrder.Count, orderFromDB.OrderDate, actionType);
                }
            }

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (Order order in dictionary.Values)
            {
                new ProductForOrder().Delete(ProductForOrder.PopulateByOrder(order.Id), connection: connection);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
        #endregion

        #region Methods
        private static List<ItemBase> LoadList(params Currency.Enum[] currencies)
        {
            var cmd = new SqlCommand("Order_Populate_Latests", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var currenciesSrt = string.Concat(currencies.Select(c => (long)c + ","));
            cmd.Parameters.Add(new SqlParameter("Currencies", SqlDbType.NVarChar, 100) { Value = currenciesSrt });

            var orders = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var order = (Order)(new Order().FromDataRow(rdr));

                    var PriceObj = rdr["Price"];
                    order.Price = PriceObj != DBNull.Value ? Convert.ToDecimal(PriceObj) : default;

                    orders.Add(order);
                }
                rdr.Close();
            }

            return orders;
        }

        public static List<ItemBase> LoadList() => LoadList(Currency.Enum.USD, Currency.Enum.EUR);

        public static List<ItemBase> LoadLocalList() => LoadList(Currency.Enum.MDL);

        public static Order PopulateById(long orderId, SqlConnection conn = null)
        {
            if (orderId <= 0)
                return null;

            var cmd = new SqlCommand("Order_Populate_One", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.BigInt) { Value = orderId });

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            ds.Tables[0].TableName = "Order";
            ds.Tables[1].TableName = "ProductsForOrder";
            ds.Tables[2].TableName = "Gifts";

            if (ds.Tables["Order"].Rows.Count == 0)
                return null;

            var Order = (Order)new Order().FromDataRow(ds.Tables["Order"].Rows[0]);

            Order.ProductsForOrder = new Dictionary<long, ItemBase>();
            Order.Gifts = new Dictionary<long, ItemBase>();

            foreach (DataRow dr in ds.Tables["ProductsForOrder"].Rows)
            {
                var ProductForOrder = (ProductForOrder)new ProductForOrder().FromDataRow(dr);
                Order.ProductsForOrder.Add(ProductForOrder.Id, ProductForOrder);
            }

            foreach (DataRow dr in ds.Tables["Gifts"].Rows)
            {
                var Gift = (Gift)new Gift().FromDataRow(dr);
                Order.Gifts.Add(Gift.Id, Gift);
            }

            return Order;
        }

        public static List<Order> PopulateByClient(Client client)
        {
            var cmd = new SqlCommand("Order_PopulateByClient", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@ClientId", SqlDbType.BigInt) { Value = client.Id });

            var orders = new List<Order>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var order = (Order)(new Order().FromDataRow(rdr));

                    var PriceObj = rdr["Price"];
                    order.Price = PriceObj != DBNull.Value ? Convert.ToDecimal(PriceObj) : default;

                    orders.Add(order);
                }
                rdr.Close();
            }

            return orders;
        }

        public decimal GetTotalSum()
        {
            decimal price = 0;

            if (this.ProductsForOrder == null || this.ProductsForOrder.Count == 0)
                return 0;

            foreach (ProductForOrder ProductForOrder in this.ProductsForOrder.Values)
                price += ProductForOrder.Price * ProductForOrder.Count;

            return price + this.Delivery + this.TAX;
        }

        public decimal GetDiscountSum()
        {
            decimal price = 0;

            if (this.ProductsForOrder == null || this.ProductsForOrder.Count == 0)
                return 0;

            foreach (ProductForOrder ProductForOrder in this.ProductsForOrder.Values)
                price += (ProductForOrder.Price - ProductForOrder.FinalPrice) * ProductForOrder.Count;

            return price;
        }

        public decimal GetFinalTotalSum()
        {
            decimal price = 0;

            if ((this.ProductsForOrder == null || this.ProductsForOrder.Count == 0) && this.Price == default)
                return 0;
            else if (this.Price != default)
                return this.Price;

            foreach (ProductForOrder ProductForOrder in this.ProductsForOrder.Values)
                price += ProductForOrder.FinalPrice * ProductForOrder.Count;

            return price + this.Delivery + this.TAX;
        }
        #endregion
    }
}