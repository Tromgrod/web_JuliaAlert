using System;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class InventoryUnit : ItemBase
    {
        #region Constructors
        public InventoryUnit()
            : base(0) { }

        public InventoryUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public Inventory Inventory { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public SpecificProductStock SpecificProductStock { get; set; }

        [Template(Mode = Template.Number)]
        public int CountInStock { get; set; }

        [Template(Mode = Template.Number)]
        public int CurrentCount { get; set; }
        #endregion

        public static InventoryUnit PopulateBySpecificProductStockIdAndInventoryId(SpecificProductStock specificProductStock, Inventory inventory)
        {
            var cmd = new SqlCommand("InventoryUnit_PopulateBySpecificProductStockIdAndInventoryId", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SpecificProductStockId", SqlDbType.BigInt) { Value = specificProductStock.Id });
            cmd.Parameters.Add(new SqlParameter("InventoryId", SqlDbType.BigInt) { Value = inventory.Id });

            var inventoryUnit = new InventoryUnit();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    inventoryUnit.FromDataRow(rdr);

                rdr.Close();
            }
            return inventoryUnit;
        }
    }
}