using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class СonsumptionFinding : ItemBase
    {
        #region Constructors
        public СonsumptionFinding()
            : base(0) { }

        public СonsumptionFinding(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public UniqueProduct UniqueProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public FindingColor FindingColor { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Consumption { get; set; }
        #endregion

        public override string GetCaption() => nameof(Consumption);

        public override string GetName() => this.Consumption.ToString();

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long uniqueProductId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByParentId(uniqueProductId);
        }

        public static Dictionary<long, ItemBase> PopulateByParentId(long uniqueProductId)
        {
            var conn = DataBase.ConnectionFromContext();
            string strSql = "СonsumptionFinding_PopulateByParentId";

            var cmd = new SqlCommand(strSql, conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = uniqueProductId });

            var consumptionFindings = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var consumptionFinding = (СonsumptionFinding)(new СonsumptionFinding().FromDataRow(rdr));
                    consumptionFindings.Add(consumptionFinding.Id, consumptionFinding);
                }
                rdr.Close();
            }
            return consumptionFindings;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var findingId = long.Parse(HttpContext.Current.Request.Form[nameof(Finding)]);
            var colorId = long.Parse(HttpContext.Current.Request.Form[nameof(ColorProduct)]);

            this.FindingColor = new FindingColor().GetByFindingAndColor(findingId, colorId);
        }
    }
}