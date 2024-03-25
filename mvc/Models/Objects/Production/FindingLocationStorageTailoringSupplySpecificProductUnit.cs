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
using System.Linq;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class FindingLocationStorageTailoringSupplySpecificProductUnit : ItemBase
    {
        #region Constructors
        public FindingLocationStorageTailoringSupplySpecificProductUnit()
            : base(0) { }

        public FindingLocationStorageTailoringSupplySpecificProductUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public TailoringSupplySpecificProductUnit TailoringSupplySpecificProductUnit { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public FindingColor FindingColor { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public LocationStorage LocationStorage { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Consumption { get; set; }
        #endregion

        public override string GetName() => string.Empty;

        public override string GetCaption() => string.Empty;

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long tailoringSupplySpecificProductUnitId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(tailoringSupplySpecificProductUnitId);
        }

        public static FindingLocationStorageTailoringSupplySpecificProductUnit PopulateById(long findingLocationStorageTailoringSupplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("FindingLocationStorageTailoringSupplySpecificProductUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingLocationStorageTailoringSupplySpecificProductUnitId", SqlDbType.BigInt) { Value = findingLocationStorageTailoringSupplySpecificProductUnitId });

            var findingLocationStorageTailoringSupplySpecificProductUnit = new FindingLocationStorageTailoringSupplySpecificProductUnit();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    findingLocationStorageTailoringSupplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }

            return findingLocationStorageTailoringSupplySpecificProductUnit;
        }

        public static List<FindingLocationStorageTailoringSupplySpecificProductUnit> PopulateByParentIdBase(long tailoringSupplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("FindingLocationStorageTailoringSupplySpecificProductUnit_PopulateByParentId", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TailoringSupplySpecificProductUnitId", SqlDbType.BigInt) { Value = tailoringSupplySpecificProductUnitId });

            var findingLocationStorageTailoringSupplySpecificProductUnits = new List<FindingLocationStorageTailoringSupplySpecificProductUnit>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var findingLocationStorageTailoringSupplySpecificProductUnit = (FindingLocationStorageTailoringSupplySpecificProductUnit)(new FindingLocationStorageTailoringSupplySpecificProductUnit().FromDataRow(rdr));
                    findingLocationStorageTailoringSupplySpecificProductUnits.Add(findingLocationStorageTailoringSupplySpecificProductUnit);
                }
                rdr.Close();
            }
            return findingLocationStorageTailoringSupplySpecificProductUnits;
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long tailoringSupplySpecificProductUnitId)
        {
            long increment = 0;

            return PopulateByParentIdBase(tailoringSupplySpecificProductUnitId).Select(i => (ItemBase)i).ToDictionary(_ => increment++);
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var findingLocationStorageTailoringSupplySpecificProductUnit = (FindingLocationStorageTailoringSupplySpecificProductUnit)item;

            var tailoringSupplySpecificProductUnit = TailoringSupplySpecificProductUnit.PopulateById(findingLocationStorageTailoringSupplySpecificProductUnit.TailoringSupplySpecificProductUnit.Id);

            UpdateFindingColorCount(findingLocationStorageTailoringSupplySpecificProductUnit, findingLocationStorageTailoringSupplySpecificProductUnit.Consumption * -tailoringSupplySpecificProductUnit.Count);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var findingLocationStorageTailoringSupplySpecificProductUnit = (FindingLocationStorageTailoringSupplySpecificProductUnit)item;

            var findingLocationStorageTailoringSupplySpecificProductUnitFromDB = PopulateById(findingLocationStorageTailoringSupplySpecificProductUnit.Id);

            var tailoringSupplySpecificProductUnit = TailoringSupplySpecificProductUnit.PopulateById(findingLocationStorageTailoringSupplySpecificProductUnit.TailoringSupplySpecificProductUnit.Id);

            UpdateFindingColorCount(findingLocationStorageTailoringSupplySpecificProductUnitFromDB, findingLocationStorageTailoringSupplySpecificProductUnit.Consumption * tailoringSupplySpecificProductUnit.Count);
            UpdateFindingColorCount(findingLocationStorageTailoringSupplySpecificProductUnit, findingLocationStorageTailoringSupplySpecificProductUnit.Consumption * -tailoringSupplySpecificProductUnit.Count);

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var findingLocationStorageTailoringSupplySpecificProductUnit = PopulateById(deleteItem.Id);

                var tailoringSupplySpecificProductUnit = TailoringSupplySpecificProductUnit.PopulateById(findingLocationStorageTailoringSupplySpecificProductUnit.TailoringSupplySpecificProductUnit.Id);

                UpdateFindingColorCount(findingLocationStorageTailoringSupplySpecificProductUnit, findingLocationStorageTailoringSupplySpecificProductUnit.Consumption * tailoringSupplySpecificProductUnit.Count);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }

        private static void UpdateFindingColorCount(FindingLocationStorageTailoringSupplySpecificProductUnit findingLocationStorageTailoringSupplySpecificProductUnit, decimal updatedCount)
        {
            var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(findingLocationStorageTailoringSupplySpecificProductUnit.LocationStorage.Id, findingLocationStorageTailoringSupplySpecificProductUnit.FindingColor);

            if (findingLocationStorage.Id > 0)
            {
                findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount + updatedCount);
            }
            else
            {
                findingLocationStorage = new FindingLocationStorage
                {
                    LocationStorage = findingLocationStorageTailoringSupplySpecificProductUnit.LocationStorage,
                    FindingColor = findingLocationStorageTailoringSupplySpecificProductUnit.FindingColor,
                    CurrentCount = updatedCount
                };

                findingLocationStorage.Insert(findingLocationStorage);
            }
        }
    }
}