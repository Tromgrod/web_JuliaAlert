using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class SupplyTextileUnit : ItemBase
    {
        #region Constructors
        public SupplyTextileUnit()
            : base(0) { }

        public SupplyTextileUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplyTextile SupplyTextile { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public TextileColor TextileColor { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Count { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Price { get; set; }
        #endregion

        public override string GetCaption() => nameof(Count);

        public override string GetName() => this.Count.ToString();

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long supplyTextileId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(supplyTextileId);
        }

        public static SupplyTextileUnit PopulateById(long supplyTextileUnitId)
        {
            var supplyTextileUnit = new SupplyTextileUnit();

            if (supplyTextileUnitId > 0)
            {
                var cmd = new SqlCommand("SupplyTextileUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("SupplyTextileUnitId", SqlDbType.BigInt) { Value = supplyTextileUnitId });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        supplyTextileUnit.FromDataRow(rdr);

                    rdr.Close();
                }
            }

            return supplyTextileUnit;
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long supplyTextileId)
        {
            var conn = DataBase.ConnectionFromContext();
            string strSql = "SupplyTextileUnit_PopulateByParentId";

            var cmd = new SqlCommand(strSql, conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplyTextileId", SqlDbType.BigInt) { Value = supplyTextileId });

            var supplyTextileUnits = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var supplyTextileUnit = (SupplyTextileUnit)(new SupplyTextileUnit().FromDataRow(rdr));
                    supplyTextileUnits.Add(supplyTextileUnit.Id, supplyTextileUnit);
                }
                rdr.Close();
            }
            return supplyTextileUnits;
        }

        public bool IsReturn()
        {
            if (this.Id > 0)
            {
                var cmd = new SqlCommand($"SELECT COUNT(ReturnSupplyTextileUnitId) FROM ReturnSupplyTextileUnit WHERE SupplyTextileUnitId = {this.Id}", DataBase.ConnectionFromContext());

                return cmd.ExecuteScalar() is int count && count > 0;
            }

            return false;
        }

        public override object LoadPopupData(long itemId) => ReturnSupplyTextileUnit.PopulateByParent(itemId);

        public decimal GetCount()
        {
            decimal count = default;

            if (this.Id > 0)
            {
                var cmd = new SqlCommand($"SELECT TOP 1 [Count] FROM SupplyTextileUnit WHERE SupplyTextileUnitId = {this.Id}", DataBase.ConnectionFromContext());

                count = cmd.ExecuteScalar() is decimal countDB ? countDB : default;
            }

            return count;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var textileId = long.Parse(HttpContext.Current.Request.Form[nameof(Textile)]);
            var colorId = long.Parse(HttpContext.Current.Request.Form[nameof(ColorProduct)]);

            this.TextileColor = new TextileColor().GetByTextileAndColor(textileId, colorId);
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var supplyTextileUnit = (SupplyTextileUnit)item;

            var textileColor = TextileColor.PopulateById(supplyTextileUnit.TextileColor.Id);

            textileColor.UpdateProperties(nameof(textileColor.CurrentCount), textileColor.CurrentCount + supplyTextileUnit.Count);

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var supplyTextileUnit = (SupplyTextileUnit)item;

            var supplyTextileUnitFromDB = PopulateById(supplyTextileUnit.Id);

            var textileColor = TextileColor.PopulateById(supplyTextileUnitFromDB.TextileColor.Id);

            supplyTextileUnit.TextileColor.UpdateProperties(nameof(supplyTextileUnit.TextileColor.CurrentCount), textileColor.CurrentCount + (supplyTextileUnit.Count - supplyTextileUnitFromDB.Count));

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var supplyTextileUnit = PopulateById(deleteItem.Id);

                supplyTextileUnit.TextileColor.UpdateProperties(nameof(supplyTextileUnit.TextileColor.CurrentCount), supplyTextileUnit.TextileColor.CurrentCount - supplyTextileUnit.Count);

                var returnSupplyTextileUnit = ReturnSupplyTextileUnit.PopulateByParent(supplyTextileUnit.Id);

                if (returnSupplyTextileUnit.Id > 0)
                {
                    returnSupplyTextileUnit.Delete();
                }
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}