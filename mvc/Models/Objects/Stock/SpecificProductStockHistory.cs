using System;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class SpecificProductStockHistory : ItemBase
    {
        #region Constructors
        public SpecificProductStockHistory()
            : base(0) { }

        public SpecificProductStockHistory(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SpecificProductStock SpecificProductStock { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.String)]
        public string Class { get; set; }

        [Template(Mode = Template.String)]
        public string ActionType { get; set; }
        #endregion

        public static SpecificProductStock PopulateBySpecificProductStockIdAndDate(SpecificProductStock specificProductStock, DateTime date)
        {
            var cmd = new SqlCommand("SpecificProductStockHistory_PopulateBySpecificProductStockIdAndDate", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SpecificProductStockId", SqlDbType.BigInt) { Value = specificProductStock.Id });
            cmd.Parameters.Add(new SqlParameter("Date", SqlDbType.DateTime) { Value = date });

            var specificProductStockOld = new SpecificProductStock();
            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    specificProductStockOld.FromDataRow(dr);

                dr.Close();
            }

            return specificProductStockOld;
        }

        public static void Insert(SpecificProductStock specificProductStock, int count, DateTime date, ActionTypeEnum actionType, string @class)
        {
            if (count != default)
            {
                var specificProductStockHistory = new SpecificProductStockHistory
                {
                    SpecificProductStock = specificProductStock,
                    Count = count,
                    Date = date,
                    Class = @class,
                    ActionType = actionType.ToString()
                };

                specificProductStockHistory.Insert(specificProductStockHistory);
            }
        }

        public enum ActionTypeEnum
        {
            None,
            Insert,
            Update,
            Delete
        }
    }
}