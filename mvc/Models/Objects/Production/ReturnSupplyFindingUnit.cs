using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Helpers;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class ReturnSupplyFindingUnit : ItemBase
    {
        #region Constructors
        public ReturnSupplyFindingUnit()
            : base(0) { }

        public ReturnSupplyFindingUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplyFindingUnit SupplyFindingUnit { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime ReturnDate { get; set; }

        [Template(Mode = Template.Description)]
        public string CauseReturn { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal ReturnCount { get; set; }
        #endregion

        #region Override Methods
        public override RequestResult SaveForm()
        {
            if (this.SupplyFindingUnit.GetCount() < this.ReturnCount)
                return new RequestResult() { Result = RequestResultType.Fail, Message = "Количество возврата больше количества заказа" };

            return base.SaveForm();
        }

        public override string GetCaption() => string.Empty;

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var returnSupplyFindingUnit = (ReturnSupplyFindingUnit)item;

            var supplyFindingUnit = SupplyFindingUnit.PopulateById(returnSupplyFindingUnit.SupplyFindingUnit.Id);

            var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(supplyFindingUnit.SupplyFinding.LocationStorage.Id, supplyFindingUnit.FindingColor);

            findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount - returnSupplyFindingUnit.ReturnCount);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var returnSupplyFindingUnit = (ReturnSupplyFindingUnit)item;

            var returnSupplyFindingUnitFromDB = PopulateById(returnSupplyFindingUnit.Id);

            var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(returnSupplyFindingUnitFromDB.SupplyFindingUnit.SupplyFinding.LocationStorage.Id, returnSupplyFindingUnitFromDB.SupplyFindingUnit.FindingColor);

            findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount + (returnSupplyFindingUnitFromDB.ReturnCount - returnSupplyFindingUnit.ReturnCount));

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var returnSupplyFindingUnit = PopulateById(deleteItem.Id);

                var findingLocationStorage = FindingLocationStorage.PopulateByFindingColorAndLocationStorage(returnSupplyFindingUnit.SupplyFindingUnit.SupplyFinding.LocationStorage.Id, returnSupplyFindingUnit.SupplyFindingUnit.FindingColor);

                findingLocationStorage.UpdateProperties(nameof(findingLocationStorage.CurrentCount), findingLocationStorage.CurrentCount + returnSupplyFindingUnit.ReturnCount);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
        #endregion

        #region Populate Methods
        public static ReturnSupplyFindingUnit PopulateById(long ReturnSupplyFindingUnitId)
        {
            var returnSupplyFindingUnit = new ReturnSupplyFindingUnit();

            if (ReturnSupplyFindingUnitId > 0)
            {
                var cmd = new SqlCommand("ReturnSupplyFindingUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("ReturnSupplyFindingUnitId", SqlDbType.BigInt) { Value = ReturnSupplyFindingUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        returnSupplyFindingUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return returnSupplyFindingUnit;
        }

        public static ReturnSupplyFindingUnit PopulateByParent(long supplyFindingUnitId)
        {
            var returnSupplyFindingUnit = new ReturnSupplyFindingUnit();

            if (supplyFindingUnitId > 0)
            {
                var cmd = new SqlCommand("ReturnSupplyFindingUnit_PopulateByParent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("SupplyFindingUnitId", SqlDbType.BigInt) { Value = supplyFindingUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        returnSupplyFindingUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return returnSupplyFindingUnit;
        }
        #endregion
    }
}