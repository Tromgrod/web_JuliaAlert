using System;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.Utils;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using LIB.Tools.BO;
using System.Data.SqlClient;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class SupplyFinding : ModelBase
    {
        #region Constructors
        public SupplyFinding()
            : base(0) { }

        public SupplyFinding(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string DocumentNumber { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public LocationStorage LocationStorage { get; set; }

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
            var cmd = new SqlCommand("SupplyFinding_LoadList", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var supplyFindings = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplyFinding = (SupplyFinding)(new SupplyFinding()).FromDataRow(rdr);

                    var toatlPriceObj = rdr[nameof(TotalPrice)];
                    supplyFinding.TotalPrice = toatlPriceObj != DBNull.Value ? Convert.ToDecimal(toatlPriceObj) : default;

                    supplyFindings.Add(supplyFinding);
                }

                rdr.Close();
            }

            return supplyFindings;
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Новый приход", Href = URLHelper.GetUrl("DocControl/SupplyFinding"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public static SupplyFinding PopulateById(long supplyFindingId)
        {
            var supplyFinding = new SupplyFinding();

            if (supplyFindingId > 0)
            {
                var cmd = new SqlCommand("SupplyFinding_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("SupplyFindingId", SqlDbType.BigInt) { Value = supplyFindingId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        supplyFinding.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return supplyFinding;
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, LIB.BusinessObjects.User user = null)
        {
            foreach (SupplyFinding supplyFinding in dictionary.Values)
            {
                new SupplyFindingUnit().Delete(SupplyFindingUnit.PopulateByParentId(supplyFinding.Id), connection: connection);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}