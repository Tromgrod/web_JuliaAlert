using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.Utils;
using LIB.Models;
using LIB.Tools.BO;
using JuliaAlertLib.BusinessObjects;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class SupplySpecificProduct : ModelBase
    {
        #region Constructors
        public SupplySpecificProduct()
            : base(0) { }

        public SupplySpecificProduct(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string DocumentNumber { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.Description)]
        public string Description { get; set; }

        [Db(_Ignore = true)]
        public int TotalCount { get; set; }
        #endregion

        public override string GetName() => this.DocumentNumber;

        public override string GetCaption() => nameof(this.DocumentNumber);

        public static List<ItemBase> LoadList()
        {
            var cmd = new SqlCommand("SupplySpecificProduct_LoadList", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var supplySpecificProducts = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplySpecificProduct = (SupplySpecificProduct)(new SupplySpecificProduct()).FromDataRow(rdr);

                    var totalCountObj = rdr[nameof(TotalCount)];
                    supplySpecificProduct.TotalCount = totalCountObj != DBNull.Value ? Convert.ToInt32(totalCountObj) : default;

                    supplySpecificProducts.Add(supplySpecificProduct);
                }

                rdr.Close();
            }

            return supplySpecificProducts;
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Новый приход", Href = URLHelper.GetUrl("DocControl/SupplySpecificProduct"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "", SqlConnection connection = null, LIB.BusinessObjects.User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            var stock = Stock.GetMainStock();

            foreach (SupplySpecificProduct supplySpecificProduct in dictionary.Values)
            {
                foreach (SupplySpecificProductUnit supplySpecificProductUnit in SupplySpecificProductUnit.PopulateByParentId(supplySpecificProduct.Id).Values)
                {
                    foreach (ImplementSupplySpecificProductUnit implementSupplySpecificProductUnit in SupplySpecificProductUnit.PopulateImplementSupplySpecificProductUnits(supplySpecificProductUnit.Id))
                    {
                        SpecificProductStock.UpdateCountInStock(implementSupplySpecificProductUnit.SupplySpecificProductUnit.SpecificProduct, stock, - implementSupplySpecificProductUnit.Count, implementSupplySpecificProductUnit.SupplySpecificProductUnit.SupplySpecificProduct.Date, actionType);
                    }
                }
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}