using System;
using LIB.Tools.BO;
using LIB.AdvancedProperties;
using LIB.Tools.Utils;
using System.Data.SqlClient;
using System.Data;
using LIB.Helpers;
using System.Web;
using LIB.BusinessObjects;
using System.Collections.Generic;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class ImplementSupplySpecificProductUnit : ItemBase
    {
        #region Constructors
        public ImplementSupplySpecificProductUnit()
            : base(0) { }

        public ImplementSupplySpecificProductUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplySpecificProductUnit SupplySpecificProductUnit { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }

        [Db(_Ignore = true)]
        public string DateString => this.Date != DateTime.MinValue ? this.Date.ToString("dd/MM/yyyy") : string.Empty;
        #endregion

        public override string GetName() => this.SupplySpecificProductUnit.Id.ToString();

        public override string GetCaption() => nameof(this.SupplySpecificProductUnit) + nameof(this.SupplySpecificProductUnit.Id);

        public override object LoadPopupData(long itemId) => SupplySpecificProductUnit.PopulateImplementSupplySpecificProductUnits(itemId);

        public static ImplementSupplySpecificProductUnit PopulateById(long implementSupplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("ImplementSupplySpecificProductUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ImplementSupplySpecificProductUnitId", SqlDbType.BigInt) { Value = implementSupplySpecificProductUnitId });

            var implementSupplySpecificProductUnit = new ImplementSupplySpecificProductUnit();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    implementSupplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }

            return implementSupplySpecificProductUnit;
        }

        public override RequestResult SaveForm()
        {
            int.TryParse(HttpContext.Current.Request.Form["SupplySpecificProductUnitCount"], out var supplySpecificProductUnitCount);
            int.TryParse(HttpContext.Current.Request.Form["ImplementSupplySpecificProductUnitCount"], out var implementSupplySpecificProductUnitCount);
            int.TryParse(HttpContext.Current.Request.Form["TotalImplementCount"], out var totalImplementCount);

            if (totalImplementCount - implementSupplySpecificProductUnitCount + this.Count <= supplySpecificProductUnitCount)
                return base.SaveForm();
            else
                return new RequestResult { Result = RequestResultType.Fail, Message = "Общее количество прихода больше чем в прайс листе" };
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Insert;

            var implementSupplySpecificProductUnit = (ImplementSupplySpecificProductUnit)item;

            var supplySpecificProductUnit = SupplySpecificProductUnit.PopulateById(implementSupplySpecificProductUnit.SupplySpecificProductUnit.Id);

            var stock = Stock.GetMainStock();

            SpecificProductStock.UpdateCountInStock(supplySpecificProductUnit.SpecificProduct, stock, implementSupplySpecificProductUnit.Count, implementSupplySpecificProductUnit.Date, actionType);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var implementSupplySpecificProductUnit = (ImplementSupplySpecificProductUnit)item;

            var implementSupplySpecificProductUnitFromDB = PopulateById(implementSupplySpecificProductUnit.Id);

            var stock = Stock.GetMainStock();

            SpecificProductStock.UpdateCountInStock(implementSupplySpecificProductUnitFromDB.SupplySpecificProductUnit.SpecificProduct, stock, implementSupplySpecificProductUnit.Count - implementSupplySpecificProductUnitFromDB.Count, implementSupplySpecificProductUnit.Date, actionType);

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            foreach (ImplementSupplySpecificProductUnit item in dictionary.Values)
            {
                var implementSupplySpecificProductUnit = PopulateById(item.Id);

                var stock = Stock.GetMainStock();

                SpecificProductStock.UpdateCountInStock(implementSupplySpecificProductUnit.SupplySpecificProductUnit.SpecificProduct, stock, - implementSupplySpecificProductUnit.Count, implementSupplySpecificProductUnit.Date, actionType);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}