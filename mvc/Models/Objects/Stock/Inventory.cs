using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.BusinessObjects;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Stock
   , ModulesAccess = (long)(Modulesenum.ControlPanel)
   , DisplayName = "Инвентаризация"
   , SingleName = "Инвентаризацию"
   , DoCancel = true
   , LogRevisions = true)]
    public class Inventory : ItemBase
    {
        #region Constructors
        public Inventory()
            : base(0) { }

        public Inventory(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Дата"), Template(Mode = Template.Date)]
        public DateTime Date { get; set; }
        #endregion

        public override string GetName() => this.Date.ToString("dd.MM.yyyy");

        public override string GetCaption() => nameof(Date);

        public static Inventory PopulateById(long inventoryId)
        {
            var cmd = new SqlCommand($"SELECT * FROM Inventory WHERE InventoryId = {inventoryId} AND DeletedBy IS NULL", DataBase.ConnectionFromContext());

            var inventory = new Inventory();
            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    inventory.FromDataRow(dr);

                dr.Close();
            }

            return inventory;
        }

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Inventory_PopulateAutocomplete", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            if (string.IsNullOrEmpty(Param) is false)
                cmd.Parameters.Add(new SqlParameter("@param", SqlDbType.NVarChar, -1) { Value = Param });

            var inventories = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var inventory = (Inventory)new Inventory().FromDataRow(dr);
                    inventories.Add(inventory.Id, inventory);
                }
                dr.Close();
            }

            return inventories;
        }
    }
}