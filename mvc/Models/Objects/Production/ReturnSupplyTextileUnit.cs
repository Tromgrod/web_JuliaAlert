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
    public class ReturnSupplyTextileUnit : ItemBase
    {
        #region Constructors
        public ReturnSupplyTextileUnit()
            : base(0) { }

        public ReturnSupplyTextileUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplyTextileUnit SupplyTextileUnit { get; set; }

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
            if (this.SupplyTextileUnit.GetCount() < this.ReturnCount)
                return new RequestResult() { Result = RequestResultType.Fail, Message = "Количество возврата больше количества заказа" };

            return base.SaveForm();
        }

        public override string GetCaption() => string.Empty;

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var returnSupplyTextileUnit = (ReturnSupplyTextileUnit)item;

            var supplyTextileUnit = SupplyTextileUnit.PopulateById(returnSupplyTextileUnit.SupplyTextileUnit.Id);

            supplyTextileUnit.TextileColor.UpdateProperties(nameof(supplyTextileUnit.TextileColor.CurrentCount), supplyTextileUnit.TextileColor.CurrentCount - returnSupplyTextileUnit.ReturnCount);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var returnSupplyTextileUnit = (ReturnSupplyTextileUnit)item;

            var returnSupplyTextileUnitFromDB = PopulateById(returnSupplyTextileUnit.Id);

            var textileColor = TextileColor.PopulateById(returnSupplyTextileUnitFromDB.SupplyTextileUnit.TextileColor.Id);

            textileColor.UpdateProperties(nameof(textileColor.CurrentCount), textileColor.CurrentCount + (returnSupplyTextileUnitFromDB.ReturnCount - returnSupplyTextileUnit.ReturnCount));

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var returnSupplyTextileUnit = PopulateById(deleteItem.Id);

                var textileColor = TextileColor.PopulateById(returnSupplyTextileUnit.SupplyTextileUnit.TextileColor.Id);

                textileColor.UpdateProperties(nameof(textileColor.CurrentCount), textileColor.CurrentCount + returnSupplyTextileUnit.ReturnCount);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
        #endregion

        #region Populate Methods
        public static ReturnSupplyTextileUnit PopulateById(long returnSupplyTextileUnitId)
        {
            var returnSupplyTextileUnit = new ReturnSupplyTextileUnit();

            if (returnSupplyTextileUnitId > 0)
            {
                var cmd = new SqlCommand("ReturnSupplyTextileUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("ReturnSupplyTextileUnitId", SqlDbType.BigInt) { Value = returnSupplyTextileUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        returnSupplyTextileUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return returnSupplyTextileUnit;
        }

        public static ReturnSupplyTextileUnit PopulateByParent(long supplyTextileUnitId)
        {
            var returnSupplyTextileUnit = new ReturnSupplyTextileUnit();

            if (supplyTextileUnitId > 0)
            {
                var cmd = new SqlCommand("ReturnSupplyTextileUnit_PopulateByParent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("SupplyTextileUnitId", SqlDbType.BigInt) { Value = supplyTextileUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        returnSupplyTextileUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return returnSupplyTextileUnit;
        }
        #endregion
    }
}