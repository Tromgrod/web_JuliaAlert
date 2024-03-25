using System;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.Utils;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using System.Data.SqlClient;
using LIB.Tools.BO;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class SupplyTextile : ModelBase
    {
        #region Constructors
        public SupplyTextile()
            : base(0) { }

        public SupplyTextile(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string DocumentNumber { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Provider Provider { get; set; }

        [Template(Mode = Template.Description)]
        public string Description { get; set; }

        [Db(_Ignore = true)]
        public decimal TotalPrice { get; set; }
        #endregion

        public override string GetName() => this.DocumentNumber;

        public override string GetCaption() => nameof(this.DocumentNumber);

        public static List<ItemBase> LoadList()
        {
            var cmd = new SqlCommand("SupplyTextile_LoadList", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var supplyTextiles = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplyTextile = (SupplyTextile)(new SupplyTextile()).FromDataRow(rdr);

                    var toatlPriceObj = rdr[nameof(TotalPrice)];
                    supplyTextile.TotalPrice = toatlPriceObj != DBNull.Value ? Convert.ToDecimal(toatlPriceObj) : default;

                    supplyTextiles.Add(supplyTextile);
                }

                rdr.Close();
            }

            return supplyTextiles;
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Новый приход", Href = URLHelper.GetUrl("DocControl/SupplyTextile"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, LIB.BusinessObjects.User user = null)
        {
            foreach (SupplyTextile supplyTextile in dictionary.Values)
            {
                new SupplyTextileUnit().Delete(SupplyTextileUnit.PopulateByParentId(supplyTextile.Id), connection: connection);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}