using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    public class Gift : ItemBase
    {
        #region Constructors
        public Gift()
            : base(0) { }

        public Gift(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SpecificProduct SpecificProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Order Order { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }
        #endregion

        #region Override methods
        public override string GetCaption() => string.Empty;

        public override string GetName() => this.SpecificProduct != null ? this.SpecificProduct.GetName() : null;

        public override Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            long OrderId = Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]);

            return PopulateByOrder(OrderId);
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var sizeId = long.Parse(HttpContext.Current.Request.Form[nameof(ProductSize)]);
            var uniqueProductId = long.Parse(HttpContext.Current.Request.Form[nameof(UniqueProduct)]);

            this.SpecificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, sizeId);
        }
        #endregion

        #region Populate
        private Dictionary<long, ItemBase> PopulateByOrder(long OrderId)
        {
            var conn = DataBase.ConnectionFromContext();
            string strSql = "Gift_Populate_ByOrder";

            var cmd = new SqlCommand(strSql, conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.BigInt) { Value = OrderId });

            var Gifts = new Dictionary<long, ItemBase>();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var Gift = (Gift)(new Gift().FromDataRow(rdr));
                    Gifts.Add(Gift.Id, Gift);
                }
                rdr.Close();
            }
            return Gifts;
        }
        #endregion
    }
}