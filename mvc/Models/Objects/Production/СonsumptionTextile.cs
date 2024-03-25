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
    public class СonsumptionTextile : ItemBase
    {
        #region Constructors
        public СonsumptionTextile()
            : base(0) { }

        public СonsumptionTextile(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public UniqueProduct UniqueProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public TextileColor TextileColor { get; set; }

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
            string strSql = "СonsumptionTextile_PopulateByParentId";

            var cmd = new SqlCommand(strSql, conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = uniqueProductId });

            var consumptionTextiles = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var consumptionTextile = (СonsumptionTextile)(new СonsumptionTextile().FromDataRow(rdr));
                    consumptionTextiles.Add(consumptionTextile.Id, consumptionTextile);
                }
                rdr.Close();
            }
            return consumptionTextiles;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var textileId = long.Parse(HttpContext.Current.Request.Form[nameof(Textile)]);
            var colorId = long.Parse(HttpContext.Current.Request.Form[nameof(ColorProduct)]);

            this.TextileColor = new TextileColor().GetByTextileAndColor(textileId, colorId);
        }
    }
}