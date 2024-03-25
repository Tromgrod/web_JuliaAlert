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
    public class SupplyFindingUnit : ItemBase
    {
        #region Constructors
        public SupplyFindingUnit()
            : base(0) { }

        public SupplyFindingUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplyFinding SupplyFinding { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public FindingColor FindingColor { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Count { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Price { get; set; }
        #endregion

        public override string GetCaption() => nameof(Count);

        public override string GetName() => this.Count.ToString();

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long supplyFindingId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(supplyFindingId);
        }

        public static SupplyFindingUnit PopulateById(long supplyFindingUnitId)
        {
            var supplyFindingUnit = new SupplyFindingUnit();

            if (supplyFindingUnitId > 0)
            {
                var cmd = new SqlCommand("SupplyFindingUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("SupplyFindingUnitId", SqlDbType.BigInt) { Value = supplyFindingUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        supplyFindingUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return supplyFindingUnit;
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long supplyFindingId)
        {
            var cmd = new SqlCommand("SupplyFindingUnit_PopulateByParentId", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplyFindingId", SqlDbType.BigInt) { Value = supplyFindingId });

            var supplyFindingUnits = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplyFindingUnit = (SupplyFindingUnit)new SupplyFindingUnit().FromDataRow(rdr);
                    supplyFindingUnits.Add(supplyFindingUnit.Id, supplyFindingUnit);
                }
                rdr.Close();
            }
            return supplyFindingUnits;
        }

        public bool IsReturn()
        {
            if (this.Id > 0)
            {
                var cmd = new SqlCommand($"SELECT COUNT(ReturnSupplyFindingUnitId) FROM ReturnSupplyFindingUnit WHERE SupplyFindingUnitId = {this.Id}", DataBase.ConnectionFromContext());

                return cmd.ExecuteScalar() is int count && count > 0;
            }

            return false;
        }

        public override object LoadPopupData(long itemId) => ReturnSupplyFindingUnit.PopulateByParent(itemId);

        public decimal GetCount()
        {
            decimal count = default;

            if (this.Id > 0)
            {
                var cmd = new SqlCommand($"SELECT TOP 1 [Count] FROM SupplyFindingUnit WHERE SupplyFindingUnitId = {this.Id}", DataBase.ConnectionFromContext());

                count = cmd.ExecuteScalar() is decimal countDB ? countDB : default;
            }

            return count;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var findingId = long.Parse(HttpContext.Current.Request.Form[nameof(Finding)]);
            var colorId = long.Parse(HttpContext.Current.Request.Form[nameof(ColorProduct)]);

            this.FindingColor = new FindingColor().GetByFindingAndColor(findingId, colorId);
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var supplyFindingUnit = (SupplyFindingUnit)item;

            var supplyFinding = SupplyFinding.PopulateById(supplyFindingUnit.SupplyFinding.Id);

            var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(supplyFinding.LocationStorage.Id, supplyFindingUnit.FindingColor);

            findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount + supplyFindingUnit.Count);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var supplyFindingUnit = (SupplyFindingUnit)item;

            var supplyFindingUnitFromDB = PopulateById(supplyFindingUnit.Id);

            var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(supplyFindingUnitFromDB.SupplyFinding.LocationStorage.Id, supplyFindingUnitFromDB.FindingColor);

            findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount + (supplyFindingUnit.Count - supplyFindingUnitFromDB.Count));

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var supplyFindingUnit = PopulateById(deleteItem.Id);

                var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(supplyFindingUnit.SupplyFinding.LocationStorage.Id, supplyFindingUnit.FindingColor);

                findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount - supplyFindingUnit.Count);

                var returnSupplyFindingUnit = ReturnSupplyFindingUnit.PopulateByParent(supplyFindingUnit.Id);

                if (returnSupplyFindingUnit.Id > 0)
                {
                    returnSupplyFindingUnit.Delete();
                }
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}