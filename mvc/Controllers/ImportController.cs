using System;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using Weblib.Controllers;
using JuliaAlert.Models.Objects;
using LIB.Tools.Security;
using LIB.Helpers;
using LIB.Tools.Utils;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace JuliaAlert.Controllers
{
    public class ImportController : BaseController
    {
        private static string[] DateFormats => new string[] { "dd/MM/yyyy", "d/M/yyy", "dd/MM/yy", "dd.MM.yyyy" };

        public ActionResult ImportExcelOrder()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Request = new RequestResult { Result = RequestResultType.Success };

            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection conn = DataBase.CreateNewConnection())
                {
                    int i = 0;

                    try
                    {
                        var ExcelData = this.ReadExcel(this.Request.Form["File"]);

                        if ((//Order
                            ExcelData.Columns.Contains("Order Number") &&
                            ExcelData.Columns.Contains("Invoice Number") &&
                            ExcelData.Columns.Contains("Order State") &&
                            ExcelData.Columns.Contains("Sales Channel") &&
                            ExcelData.Columns.Contains("Departure Date") &&
                            ExcelData.Columns.Contains("Receiving Date") &&
                            ExcelData.Columns.Contains("Tracking Number") &&
                            ExcelData.Columns.Contains("Stock") &&
                            ExcelData.Columns.Contains("Delivery") &&
                            ExcelData.Columns.Contains("TAX") &&
                            ExcelData.Columns.Contains("Description") &&
                            //Client
                            ExcelData.Columns.Contains("Client") &&
                            ExcelData.Columns.Contains("Country Client") &&
                            ExcelData.Columns.Contains("Abbreviation Country Client") &&
                            ExcelData.Columns.Contains("State Client") &&
                            ExcelData.Columns.Contains("Abbreviation State Client") &&
                            ExcelData.Columns.Contains("City Client") &&
                            ExcelData.Columns.Contains("Index Client") &&
                            ExcelData.Columns.Contains("Address Client") &&
                            ExcelData.Columns.Contains("Comment Client") &&
                            ExcelData.Columns.Contains("Phone Client") &&
                            //Model
                            ExcelData.Columns.Contains("Model") &&
                            //ProductForOrder
                            ExcelData.Columns.Contains("Price") &&
                            ExcelData.Columns.Contains("Count") &&
                            ExcelData.Columns.Contains("Size") &&
                            ExcelData.Columns.Contains("Discount") &&
                            //Return
                            ExcelData.Columns.Contains("Return Count") &&
                            ExcelData.Columns.Contains("Return Date") &&
                            ExcelData.Columns.Contains("Receiving Return Date") &&
                            ExcelData.Columns.Contains("Cause Return") &&
                            ExcelData.Columns.Contains("In Country") &&
                            ExcelData.Columns.Contains("Return Tracking Number")) is false)
                        {
                            throw new Exception("Одной из колонок не хватает!");
                        }

                        var OrderStates = new List<OrderState>();
                        var SalesChannels = new List<SalesChannel>();
                        var Products = new List<Product>();
                        var Colors = new List<ColorProduct>();
                        var Decors = new List<Decor>();
                        var Sizes = new List<ProductSize>();
                        var UniqueProducts = new List<UniqueProduct>();
                        var SpecificProducts = new List<SpecificProduct>();
                        var stocks = new List<Stock>();

                        for (; i < ExcelData.Rows.Count; i++)
                        {
                            string //Order
                                   OrderNumber = ExcelData.Rows[i]["Order Number"].ToString(),
                                   InvoiceNumber = ExcelData.Rows[i]["Invoice Number"].ToString(),
                                   OrderStateName = ExcelData.Rows[i]["Order State"].ToString(),
                                   SalesChannelName = ExcelData.Rows[i]["Sales Channel"].ToString(),
                                   OrderDateStr = ExcelData.Rows[i]["Order Date"].ToString(),
                                   DepartureDateStr = ExcelData.Rows[i]["Departure Date"].ToString(),
                                   ReceivingDateStr = ExcelData.Rows[i]["Receiving Date"].ToString(),
                                   TrackingNumber = ExcelData.Rows[i]["Tracking Number"].ToString(),
                                   stockName = ExcelData.Rows[i]["Stock"].ToString(),
                                   DeliveryStr = ExcelData.Rows[i]["Delivery"].ToString(),
                                   TAXStr = ExcelData.Rows[i]["TAX"].ToString(),
                                   Description = ExcelData.Rows[i]["Description"].ToString(),
                                   //Client
                                   ClientName = ExcelData.Rows[i]["Client"].ToString(),
                                   CountryName = ExcelData.Rows[i]["Country Client"].ToString(),
                                   CountryShortName = ExcelData.Rows[i]["Abbreviation Country Client"].ToString(),
                                   StateName = ExcelData.Rows[i]["State Client"].ToString(),
                                   StateShortName = ExcelData.Rows[i]["Abbreviation State Client"].ToString(),
                                   CityName = ExcelData.Rows[i]["City Client"].ToString(),
                                   Index = ExcelData.Rows[i]["Index Client"].ToString(),
                                   Address = ExcelData.Rows[i]["Address Client"].ToString(),
                                   Comment = ExcelData.Rows[i]["Comment Client"].ToString(),
                                   Phone = ExcelData.Rows[i]["Phone Client"].ToString(),
                                   //Model
                                   ModelCode = ExcelData.Rows[i]["Model"].ToString(),
                                   //ProductForOrder
                                   PriceStr = ExcelData.Rows[i]["Price"].ToString(),
                                   CountStr = ExcelData.Rows[i]["Count"].ToString(),
                                   SizeName = ExcelData.Rows[i]["Size"].ToString(),
                                   DiscountStr = ExcelData.Rows[i]["Discount"].ToString().Trim().Replace("%", ""),
                                   //Return
                                   ReturnCountStr = ExcelData.Rows[i]["Return Count"].ToString(),
                                   ReturnDateStr = ExcelData.Rows[i]["Return Date"].ToString(),
                                   ReceivingReturnDateStr = ExcelData.Rows[i]["Receiving Return Date"].ToString(),
                                   CauseReturn = ExcelData.Rows[i]["Cause Return"].ToString(),
                                   InCountryStr = ExcelData.Rows[i]["In Country"].ToString(),
                                   ReturnTrackingNumber = ExcelData.Rows[i]["Return Tracking Number"].ToString();

                            if (string.IsNullOrEmpty(OrderNumber))
                                continue;

                            var Client = new Client(ClientName, conn)
                            {
                                Countries = string.IsNullOrEmpty(CountryName) ? null : new Countries(CountryName, CountryShortName, conn),
                                State = string.IsNullOrEmpty(StateName) ? null : new State(StateName, StateShortName, conn),
                                City = string.IsNullOrEmpty(CityName) ? null : new City(CityName, conn),
                                Index = string.IsNullOrEmpty(Index) ? null : Index,
                                Address = string.IsNullOrEmpty(Address) ? null : Address,
                                Comment = string.IsNullOrEmpty(Comment) ? null : Comment,
                                Phone = string.IsNullOrEmpty(Phone) ? null : Phone
                            };

                            if (Client.Id > 0)
                                Client.Update(Client, connection: conn);
                            else
                                Client.Insert(Client, connection: conn);

                            var OrderState = OrderStates.FirstOrDefault(os => os.Name == OrderStateName);

                            if (OrderState == null || OrderState.Id <= 0)
                            {
                                OrderState = new OrderState(OrderStateName, conn);
                                OrderStates.Add(OrderState);
                            }

                            var SalesChannel = SalesChannels.FirstOrDefault(sc => sc.Name == SalesChannelName);

                            if (SalesChannel == null || SalesChannel.Id <= 0)
                            {
                                SalesChannel = new SalesChannel(SalesChannelName, conn);
                                SalesChannels.Add(SalesChannel);
                            }

                            var stock = stocks.FirstOrDefault(ss => ss.Name == stockName);

                            if (stock == null || stock.Id <= 0)
                            {
                                stock = new Stock(stockName, conn);
                                stocks.Add(stock);
                            }

                            var Order = new Order(OrderNumber, conn);

                            if (Order.Id <= 0)
                            {
                                Order.InvoiceNumber = InvoiceNumber;
                                Order.OrderState = OrderState;
                                Order.SalesChannel = SalesChannel;
                                Order.OrderDate = DateTime.TryParseExact(OrderDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime OrderDate) ? OrderDate : int.TryParse(OrderDateStr, out int OrderDateInt) ? new DateTime(1899, 12, 30).AddDays(OrderDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты или дата пустая (Order Date): {OrderDateStr}");
                                Order.DepartureDate = DateTime.TryParseExact(DepartureDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime DepartureDate) ? DepartureDate : string.IsNullOrEmpty(DepartureDateStr) ? DateTime.MinValue : int.TryParse(DepartureDateStr, out int DepartureDateInt) ? new DateTime(1899, 12, 30).AddDays(DepartureDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты (Departure Date): {DepartureDateStr}");
                                Order.ReceivingDate = DateTime.TryParseExact(ReceivingDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime ReceivingDate) ? ReceivingDate : string.IsNullOrEmpty(ReceivingDateStr) ? DateTime.MinValue : int.TryParse(ReceivingDateStr, out int ReceivingDateInt) ? new DateTime(1899, 12, 30).AddDays(ReceivingDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты (Receiving Date): {ReceivingDateStr}");
                                Order.TrackingNumber = string.IsNullOrEmpty(TrackingNumber) ? null : TrackingNumber;
                                Order.Stock = stock;
                                Order.Delivery = decimal.TryParse(DeliveryStr, out decimal Delivery) ? Delivery : string.IsNullOrEmpty(DeliveryStr) ? 0 : throw new Exception($"Строка: {i + 1}. Некоректный формат цены доставки: {DeliveryStr}");
                                Order.TAX = decimal.TryParse(TAXStr, out decimal TAX) ? TAX : string.IsNullOrEmpty(TAXStr) ? 0 : throw new Exception($"Строка: {i + 1}. Некоректный формат TAX: {TAXStr}");
                                Order.Description = string.IsNullOrEmpty(Description) ? null : Description;
                                Order.Client = Client;

                                Order.Insert(Order, connection: conn);
                            }

                            var Codes = ModelCode.Split('-');

                            if (Codes.Length != 3)
                                throw new Exception($"Строка: {i + 1}. Неверный формат кода: {Codes}");

                            var Product = Products.FirstOrDefault(p => p.Code == Codes[0]);

                            if (Product == null || Product.Id <= 0)
                            {
                                Product = new Product(Codes[0], conn);
                                Products.Add(Product);
                            }

                            var Color = Colors.FirstOrDefault(c => c.Code == Codes[1]);

                            if (Color == null || Color.Id <= 0)
                            {
                                Color = new ColorProduct(Codes[1], conn);
                                Colors.Add(Color);
                            }

                            var Decor = Decors.FirstOrDefault(d => d.Code == Codes[2]);

                            if (Decor == null || Decor.Id <= 0)
                            {
                                Decor = new Decor(Codes[2], conn);
                                Decors.Add(Decor);
                            }

                            var Size = Sizes.FirstOrDefault(s => s.Name == SizeName);

                            if (Size == null || Size.Id <= 0)
                            {
                                Size = new ProductSize(SizeName, conn);
                                Sizes.Add(Size);
                            }

                            var UniqueProduct = UniqueProducts.FirstOrDefault(up => up.Product.Id == Product.Id && up.ColorProduct.Id == Color.Id && Decor.Id == Decor.Id);

                            if (UniqueProduct == null || UniqueProduct.Id <= 0)
                            {
                                UniqueProduct = new UniqueProduct(Product, Color, Decor, conn);
                                UniqueProducts.Add(UniqueProduct);
                            }

                            var SpecificProduct = SpecificProducts.FirstOrDefault(sp => sp.UniqueProduct.Id == UniqueProduct.Id && sp.ProductSize.Id == Size.Id);

                            if (SpecificProduct == null || SpecificProduct.Id <= 0)
                            {
                                SpecificProduct = new SpecificProduct(UniqueProduct, Size, conn);
                                SpecificProducts.Add(SpecificProduct);
                            }

                            var DiscountExcel = int.TryParse(DiscountStr, out int Discount) ? Discount : string.IsNullOrEmpty(DiscountStr) ? 0 : decimal.TryParse(DiscountStr, out decimal DiscountDec) ? (DiscountDec >= 0 && DiscountDec <= 1 ? Convert.ToInt32(DiscountDec * 100) : throw new Exception($"Строка: {i + 1}. Некоректный формат скидки: {DiscountStr}")) : throw new Exception($"Строка: {i + 1}. Некоректный формат скидки: {DiscountStr}");

                            var ProductForOrder = new ProductForOrder(SpecificProduct, Order, conn)
                            {
                                Price = decimal.TryParse(PriceStr, out decimal Price) && Price > 0 ? Price : throw new Exception($"Строка: {i + 1}. Некоректный формат цены товара или число меньше или равно 0: {PriceStr}"),
                                Discount = DiscountExcel,
                                FinalPrice = Price * (1 - Convert.ToDecimal(DiscountExcel) / 100),
                                Count = int.TryParse(CountStr, out int Count) && Count > 0 ? Count : throw new Exception($"Строка: {i + 1}. Некоректный формат количества товара или число меньше или равно 0: {CountStr}")
                            };

                            if (ProductForOrder.Id <= 0)
                            {
                                ProductForOrder.Insert(ProductForOrder, connection: conn);

                                if (int.TryParse(ReturnCountStr, out int ReturnCount) is false && string.IsNullOrEmpty(ReturnCountStr) is false)
                                {
                                    throw new Exception($"Строка: {i + 1}. Некоректный формат количества возвращенного товара: {ReturnCountStr}");
                                }
                                else if (ReturnCount > ProductForOrder.Count)
                                {
                                    throw new Exception($"Строка: {i + 1}. Количество возвращенного товара больше количества купленого товара: {ProductForOrder.Count} < {ReturnCount}");
                                }
                                else if (ReturnCount > 0)
                                {
                                    var Return = new Return
                                    {
                                        ProductForOrder = ProductForOrder,
                                        ReturnCount = ReturnCount,
                                        ReturnDate = DateTime.TryParseExact(ReturnDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime ReturnDate) ? ReturnDate : int.TryParse(ReturnDateStr, out int ReturnDateInt) ? new DateTime(1899, 12, 30).AddDays(ReturnDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты или дата пустая (Return Date): {ReturnDateStr}"),
                                        ReceivingReturnDate = DateTime.TryParseExact(ReceivingReturnDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime ReceivingReturnDate) ? ReceivingReturnDate : string.IsNullOrEmpty(ReceivingReturnDateStr) ? DateTime.MinValue : int.TryParse(ReceivingReturnDateStr, out int ReceivingReturnDateInt) ? new DateTime(1899, 12, 30).AddDays(ReceivingReturnDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты (Receiving Return Date): {ReceivingReturnDateStr}"),
                                        CauseReturn = string.IsNullOrEmpty(CauseReturn) ? null : CauseReturn,
                                        InCountry = int.TryParse(InCountryStr, out int InCountryInt) ? Convert.ToBoolean(InCountryInt) : string.IsNullOrEmpty(InCountryStr) ? false : throw new Exception($"Строка: {i + 1}. Неверный формат 'В стране' значение должно быть 0 или 1: {InCountryStr}"),
                                        TrackingNumber = string.IsNullOrEmpty(TrackingNumber) ? null : TrackingNumber
                                    };

                                    Return.Insert(Return, connection: conn);
                                }
                            }
                            else
                            {
                                Request.Message += $"Предупреждение! Строка: {i + 1}\nВ базе даных в заказе '{Order.OrderNumber}' уже существует проданый товар: Код - '{SpecificProduct.UniqueProduct.GetCode()}' Размер - '{SpecificProduct.ProductSize.Name}'\nТовар проигнорирован\n\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();

                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = $"Строка: {i + 1}! \n {ex.Message}" });
                    }
                }

                scope.Complete();
            }

            Request.Message = string.IsNullOrEmpty(Request.Message) ? "Импорт прошел успешно!" : "Импорт прошел успешно!\n\n" + Request.Message;

            GC.Collect();

            return this.Json(Request);
        }

        public ActionResult ImportExcelLocalOrder()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Request = new RequestResult { Result = RequestResultType.Success };

            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection conn = DataBase.CreateNewConnection())
                {
                    int i = 0;

                    try
                    {
                        var ExcelData = this.ReadExcel(this.Request.Form["File"]);

                        if ((//Order
                            ExcelData.Columns.Contains("Order State") &&
                            ExcelData.Columns.Contains("Sales Channel") &&
                            ExcelData.Columns.Contains("Subsidiary Stock") &&
                            ExcelData.Columns.Contains("Description") &&
                            //Client
                            ExcelData.Columns.Contains("Client") &&
                            ExcelData.Columns.Contains("Comment Client") &&
                            ExcelData.Columns.Contains("Phone Client") &&
                            //Model
                            ExcelData.Columns.Contains("Model") &&
                            //ProductForOrder
                            ExcelData.Columns.Contains("Price") &&
                            ExcelData.Columns.Contains("Count") &&
                            ExcelData.Columns.Contains("Size") &&
                            ExcelData.Columns.Contains("Discount") &&
                            //Return
                            ExcelData.Columns.Contains("Return Count") &&
                            ExcelData.Columns.Contains("Return Date") &&
                            ExcelData.Columns.Contains("Cause Return")) is false)
                        {
                            throw new Exception("Одной из колонок не хватает!");
                        }

                        var Orders = new List<Order>();
                        var Clients = new List<Client>();
                        var OrderStates = new List<OrderState>();
                        var SalesChannels = new List<SalesChannel>();
                        var Products = new List<Product>();
                        var Colors = new List<ColorProduct>();
                        var Decors = new List<Decor>();
                        var Sizes = new List<ProductSize>();
                        var UniqueProducts = new List<UniqueProduct>();
                        var SpecificProducts = new List<SpecificProduct>();
                        var stocks = new List<Stock>();

                        var Country = new Countries("Moldova", "MD", conn);
                        var State = new State("mun. Chisinau", "KIV", conn);
                        var City = new City("Chisinau", conn);

                        var OrdersFromDB = new Order().Populate(conn: conn);

                        for (; i < ExcelData.Rows.Count; i++)
                        {
                            string //Order
                                   OrderStateName = ExcelData.Rows[i]["Order State"].ToString(),
                                   SalesChannelName = ExcelData.Rows[i]["Sales Channel"].ToString(),
                                   OrderDateStr = ExcelData.Rows[i]["Order Date"].ToString(),
                                   stockName = ExcelData.Rows[i]["Stock"].ToString(),
                                   Description = ExcelData.Rows[i]["Description"].ToString(),
                                   //Client
                                   ClientName = ExcelData.Rows[i]["Client"].ToString(),
                                   Comment = ExcelData.Rows[i]["Comment Client"].ToString(),
                                   Phone = ExcelData.Rows[i]["Phone Client"].ToString(),
                                   //Model
                                   ModelCode = ExcelData.Rows[i]["Model"].ToString(),
                                   //ProductForOrder
                                   PriceStr = ExcelData.Rows[i]["Price"].ToString(),
                                   CountStr = ExcelData.Rows[i]["Count"].ToString(),
                                   SizeName = ExcelData.Rows[i]["Size"].ToString(),
                                   DiscountStr = ExcelData.Rows[i]["Discount"].ToString().Trim().Replace("%", ""),
                                   //Return
                                   ReturnCountStr = ExcelData.Rows[i]["Return Count"].ToString(),
                                   ReturnDateStr = ExcelData.Rows[i]["Return Date"].ToString(),
                                   CauseReturn = ExcelData.Rows[i]["Cause Return"].ToString();

                            if (string.IsNullOrEmpty(OrderStateName))
                                continue;

                            var OrderState = OrderStates.FirstOrDefault(os => os.Name == OrderStateName);

                            if (OrderState == null || OrderState.Id <= 0)
                            {
                                OrderState = new OrderState(OrderStateName, conn);
                                OrderStates.Add(OrderState);
                            }

                            var SalesChannel = SalesChannels.FirstOrDefault(sc => sc.Name == SalesChannelName);

                            if (SalesChannel == null || SalesChannel.Id <= 0)
                            {
                                SalesChannel = new SalesChannel(SalesChannelName, conn);
                                SalesChannels.Add(SalesChannel);
                            }

                            var stock = stocks.FirstOrDefault(ss => ss.Name == stockName);

                            if (stock == null || stock.Id <= 0)
                            {
                                stock = new Stock(stockName, conn);
                                stocks.Add(stock);
                            }

                            var Codes = ModelCode.Split('-');

                            if (Codes.Length != 3)
                                throw new Exception($"Строка: {i + 1}. Неверный формат кода: {Codes}");

                            var Product = Products.FirstOrDefault(p => p.Code == Codes[0]);

                            if (Product == null || Product.Id <= 0)
                            {
                                Product = new Product(Codes[0], conn);
                                Products.Add(Product);
                            }

                            var Color = Colors.FirstOrDefault(c => c.Code == Codes[1]);

                            if (Color == null || Color.Id <= 0)
                            {
                                Color = new ColorProduct(Codes[1], conn);
                                Colors.Add(Color);
                            }

                            var Decor = Decors.FirstOrDefault(d => d.Code == Codes[2]);

                            if (Decor == null || Decor.Id <= 0)
                            {
                                Decor = new Decor(Codes[2], conn);
                                Decors.Add(Decor);
                            }

                            var Size = Sizes.FirstOrDefault(s => s.Name == SizeName);

                            if (Size == null || Size.Id <= 0)
                            {
                                Size = new ProductSize(SizeName, conn);
                                Sizes.Add(Size);
                            }

                            var UniqueProduct = UniqueProducts.FirstOrDefault(up => up.Product.Id == Product.Id && up.ColorProduct.Id == Color.Id && Decor.Id == Decor.Id);

                            if (UniqueProduct == null || UniqueProduct.Id <= 0)
                            {
                                UniqueProduct = new UniqueProduct(Product, Color, Decor, conn);
                                UniqueProducts.Add(UniqueProduct);
                            }

                            var SpecificProduct = SpecificProducts.FirstOrDefault(sp => sp.UniqueProduct.Id == UniqueProduct.Id && sp.ProductSize.Id == Size.Id);

                            if (SpecificProduct == null || SpecificProduct.Id <= 0)
                            {
                                SpecificProduct = new SpecificProduct(UniqueProduct, Size, conn);
                                SpecificProducts.Add(SpecificProduct);
                            }

                            Order Order = null;

                            var Client = Clients.FirstOrDefault(c => c.Name == ClientName);

                            if (Client == null || Client.Id <= 0)
                            {
                                Client = new Client(ClientName, conn)
                                {
                                    Countries = Country,
                                    State = State,
                                    City = City,
                                    Comment = string.IsNullOrEmpty(Comment) ? null : Comment,
                                    Phone = string.IsNullOrEmpty(Phone) ? null : Phone
                                };

                                if (Client.Id > 0)
                                    Client.Update(Client, connection: conn);
                                else
                                    Client.Insert(Client, connection: conn);

                                var OrderDateExcel = DateTime.TryParseExact(OrderDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime OrderDate) ? OrderDate : int.TryParse(OrderDateStr, out int OrderDateInt) ? new DateTime(1899, 12, 30).AddDays(OrderDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты или дата пустая (Order Date): {OrderDateStr}");

                                var OrderFromDB = OrdersFromDB.Values
                                    .Select(item => (Order)item)
                                    .FirstOrDefault(o => o.OrderState.Id == OrderState.Id && o.SalesChannel.Id == SalesChannel.Id && o.OrderDate == OrderDateExcel && o.Client.Id == Client.Id);

                                if (OrderFromDB != null)
                                    throw new Exception($"Строка: {i + 1}. В Базе даных уже существует заказ с клиетом: {Client.Name} с датой: {OrderDateExcel:dd/MM/yyyy}. Группировка невозможна!");

                                Order = new Order
                                {
                                    OrderState = OrderState,
                                    SalesChannel = SalesChannel,
                                    OrderDate = OrderDateExcel,
                                    Stock = stock,
                                    Description = string.IsNullOrEmpty(Description) ? null : Description,
                                    Client = Client
                                };

                                Order.Insert(Order, connection: conn);

                                Orders.Add(Order);
                                Clients.Add(Client);
                            }
                            else
                            {
                                Order = Orders.FirstOrDefault(o => o.Client.Name == Client.Name);

                                if (Order == null && Order.Id <= 0)
                                    throw new Exception("Jopa");
                            }

                            var DiscountExcel = int.TryParse(DiscountStr, out int Discount) ? Discount : string.IsNullOrEmpty(DiscountStr) ? 0 : decimal.TryParse(DiscountStr, out decimal DiscountDec) ? (DiscountDec >= 0 && DiscountDec <= 1 ? Convert.ToInt32(DiscountDec * 100) : throw new Exception($"Строка: {i + 1}. Некоректный формат скидки: {DiscountStr}")) : throw new Exception($"Строка: {i + 1}. Некоректный формат скидки: {DiscountStr}");

                            var ProductForOrder = new ProductForOrder(SpecificProduct, Order, conn)
                            {
                                Price = decimal.TryParse(PriceStr, out decimal Price) && Price > 0 ? Price : throw new Exception($"Строка: {i + 1}. Некоректный формат цены товара или число меньше или равно 0: {PriceStr}"),
                                Discount = DiscountExcel,
                                FinalPrice = Price * (1 - Convert.ToDecimal(DiscountExcel) / 100),
                                Count = int.TryParse(CountStr, out int Count) && Count > 0 ? Count : throw new Exception($"Строка: {i + 1}. Некоректный формат количества товара или число меньше или равно 0: {CountStr}")
                            };

                            if (ProductForOrder.Id <= 0)
                            {
                                ProductForOrder.Insert(ProductForOrder, connection: conn);

                                if (int.TryParse(ReturnCountStr, out int ReturnCount) is false && string.IsNullOrEmpty(ReturnCountStr) is false)
                                {
                                    throw new Exception($"Строка: {i + 1}. Некоректный формат количества возвращенного товара: {ReturnCountStr}");
                                }
                                else if (ReturnCount > ProductForOrder.Count)
                                {
                                    throw new Exception($"Строка: {i + 1}. Количество возвращенного товара больше количества купленого товара: {ProductForOrder.Count} < {ReturnCount}");
                                }
                                else if (ReturnCount > 0)
                                {
                                    var Return = new Return
                                    {
                                        ProductForOrder = ProductForOrder,
                                        ReturnCount = ReturnCount,
                                        ReturnDate = DateTime.TryParseExact(ReturnDateStr, DateFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime ReturnDate) ? ReturnDate : int.TryParse(ReturnDateStr, out int ReturnDateInt) ? new DateTime(1899, 12, 30).AddDays(ReturnDateInt) : throw new Exception($"Строка: {i + 1}. Некоректный формат даты или дата пустая (Return Date): {ReturnDateStr}"),
                                        CauseReturn = string.IsNullOrEmpty(CauseReturn) ? null : CauseReturn,
                                        InCountry = true
                                    };

                                    Return.Insert(Return, connection: conn);
                                }
                            }
                            else
                            {
                                Request.Message += $"Предупреждение! Строка: {i + 1}\nВ базе даных в заказе '{Order.OrderNumber}' уже существует проданый товар: Код - '{SpecificProduct.UniqueProduct.GetCode()}' Размер - '{SpecificProduct.ProductSize.Name}'\nТовар проигнорирован\n\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();

                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = $"Строка: {i + 1}! \n {ex.Message}" });
                    }
                }

                scope.Complete();
            }

            Request.Message = string.IsNullOrEmpty(Request.Message) ? "Импорт прошел успешно!" : "Импорт прошел успешно!\n\n" + Request.Message;

            GC.Collect();

            return this.Json(Request);
        }

        public ActionResult ImportExcelModel()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Request = new RequestResult { Result = RequestResultType.Success };

            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection conn = DataBase.CreateNewConnection())
                {
                    int i = 0;

                    try
                    {
                        using (var ExcelData = this.ReadExcel(this.Request.Form["File"]))
                        {
                            if ((
                                ExcelData.Columns.Contains("Model Code") &&
                                ExcelData.Columns.Contains("Model Name") &&
                                ExcelData.Columns.Contains("Sizes") &&
                                ExcelData.Columns.Contains("Decor Codes") &&
                                ExcelData.Columns.Contains("Color Codes") &&
                                ExcelData.Columns.Contains("Type Product") &&
                                ExcelData.Columns.Contains("Compound")) is false)
                            {
                                throw new Exception("Одной из колонок не хватает!");
                            }

                            for (; i < ExcelData.Rows.Count; i++)
                            {
                                string ModelCode = ExcelData.Rows[i]["Model Code"].ToString(),
                                       ModelName = ExcelData.Rows[i]["Model Name"].ToString(),
                                       SizeNames = ExcelData.Rows[i]["Sizes"].ToString(),
                                       DecorCodes = ExcelData.Rows[i]["Decor Codes"].ToString(),
                                       ColorCodes = ExcelData.Rows[i]["Color Codes"].ToString(),
                                       TypeProductName = ExcelData.Rows[i]["Type Product"].ToString(),
                                       CompoundName = ExcelData.Rows[i]["Compound"].ToString();

                                var Product = new Product
                                {
                                    Code = ModelCode,
                                    Name = ModelName
                                };

                                if (Product.IsUniqueCode(conn) is false)
                                    throw new Exception($"Строка: {i + 1}. Модель с таким кодом уже существует: {Product.Code}");

                                var SizeList = SizeNames.Split(';')
                                    .Select(size => size.Trim().Replace("[", "").Replace("]", ""))
                                    .Where(size => !string.IsNullOrEmpty(size) && size != "");

                                var DecorCodeList = DecorCodes.Split(';')
                                    .Select(code => code.Trim().Replace("[", "").Replace("]", ""))
                                    .Where(code => !string.IsNullOrEmpty(code) && code != "");

                                var ColorCodeList = ColorCodes.Split(';')
                                    .Select(code => code.Trim().Replace("[", "").Replace("]", ""))
                                    .Where(code => !string.IsNullOrEmpty(code) && code != "");

                                Product.TypeProduct = new TypeProduct(TypeProductName, conn);

                                Product.Insert(Product, connection: conn);

                                var SizeIdList = new List<long>();

                                foreach (var SizeName in SizeList)
                                {
                                    SizeIdList.Add(new ProductSize(SizeName, conn).Id);
                                }
                                if (SizeIdList.Count <= 0)
                                    throw new Exception($"Строка: {i + 1}. Список размеров пуст!");

                                var DecorIdList = new List<long>();

                                foreach (var DecorCode in DecorCodeList)
                                {
                                    DecorIdList.Add(new Decor(DecorCode, conn).Id);
                                }
                                if (DecorIdList.Count <= 0)
                                    throw new Exception($"Строка: {i + 1}. Список декора пуст!");

                                var ColorIdList = new List<long>();

                                foreach (var ColorCode in ColorCodeList)
                                {
                                    ColorIdList.Add(new ColorProduct(ColorCode, conn).Id);
                                }
                                if (ColorIdList.Count <= 0)
                                    throw new Exception($"Строка: {i + 1}. Список цветов пуст!");

                                var Compound = new Compound(CompoundName, conn);

                                Product.InsertUniqueModel(SizeIdList, ColorIdList, DecorIdList, Compound, conn);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();

                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = $"Строка: {i + 1}! \n {ex.Message}" });
                    }
                }

                scope.Complete();
            }

            GC.Collect();

            Request.Message = string.IsNullOrEmpty(Request.Message) ? "Импорт прошел успешно!" : "Импорт прошел успешно!\n\n" + Request.Message;

            return this.Json(Request);
        }

        #region Read Excel
        private DataTable ReadExcel(string path)
        {
            try
            {
                var dt = new DataTable();

                using (var ssDoc = SpreadsheetDocument.Open(path, false))
                {
                    var sheets = ssDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    var relationshipId = sheets.First().Id.Value;
                    var worksheetPart = (WorksheetPart)ssDoc.WorkbookPart.GetPartById(relationshipId);
                    var workSheet = worksheetPart.Worksheet;
                    var sheetData = workSheet.GetFirstChild<SheetData>();
                    var rows = sheetData.Descendants<Row>().ToList();

                    for (var i = 0; i < rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            foreach (var cell in rows[0].Descendants<Cell>())
                            {
                                dt.Columns.Add(GetCellValue(ssDoc, cell));
                            }
                        }
                        else
                        {
                            var cells = rows[i].Descendants<Cell>();

                            if (cells != null && cells.Count() == dt.Columns.Count)
                            {
                                var tempRow = dt.NewRow();

                                foreach (var cell in cells)
                                {
                                    var index = GetIndex(cell.CellReference);
                                    tempRow[index] = GetCellValue(ssDoc, cell);
                                }

                                dt.Rows.Add(tempRow);
                            }
                        }
                    }
                }

                return dt;
            }
            catch
            {
                throw new Exception("Ошибка чтения файла! Обратитемь к администрации");
            }
        }

        private string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            var stringTablePart = document.WorkbookPart.SharedStringTablePart;

            if (cell.CellValue == null)
                return string.Empty;

            var value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                return stringTablePart.SharedStringTable.ChildElements[int.TryParse(value, out int result) ? result : 0].InnerText;

            return value;
        }

        private int GetIndex(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return -1;

            int index = 0;
            foreach (var ch in name)
            {
                if (char.IsLetter(ch))
                {
                    int value = ch - 'A' + 1;
                    index = value + index * 26;
                }
                else
                    break;
            }

            return index - 1;
        }
        #endregion
    }
}