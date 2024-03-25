using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.BusinessObjects;
using LIB.Tools.Utils;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    public class ProductPrice : ItemBase
    {
        #region Constructors
        public ProductPrice()
            : base(0) { }
        public ProductPrice(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public Product Product { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public SalesChannel SalesChannel { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal Price { get; set; }
        #endregion

        #region Override Methods
        public override string GetCaption() => string.Empty;

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var cmd = new SqlCommand("Insert-Update_ProductPrice", connection ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.BigInt) { Value = this.Product.Id });
            cmd.Parameters.Add(new SqlParameter("@SalesChannelId", SqlDbType.BigInt) { Value = this.SalesChannel.Id });
            cmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = this.Price });
            cmd.Parameters.Add(new SqlParameter("@CurrentUser", SqlDbType.BigInt) { Value = Authentication.GetCurrentUser().Id });

            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Populate
        public static List<ProductPrice> PopulateByProduct(Product Product, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("ProductPrice_PopulateByProduct", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("ProductId", SqlDbType.BigInt) { Value = Product.Id });

            var TypeProducts = new List<ProductPrice>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var TypeProduct = (ProductPrice)new ProductPrice().FromDataRow(rdr);
                    TypeProducts.Add(TypeProduct);
                }
                rdr.Close();
            }
            return TypeProducts;
        }
        #endregion
    }
}