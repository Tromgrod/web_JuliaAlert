using System;
using System.Data;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    public class SpecificProduct : ItemBase
    {
        #region Constructors
        public SpecificProduct()
            : base(0) { }

        public SpecificProduct(long id)
            : base(id) { }

        public SpecificProduct(UniqueProduct uniqueProduct, ProductSize productSize)
            : base(0) 
        {
            UniqueProduct = uniqueProduct;
            ProductSize = productSize;
        }

        public SpecificProduct(UniqueProduct uniqueProduct, ProductSize productSize, SqlConnection conn = null)
            : base(0) 
        {
            this.UniqueProduct = uniqueProduct;
            this.ProductSize = productSize;

            var cmd = new SqlCommand($"SELECT SpecificProductId FROM SpecificProduct WHERE UniqueProductId = {UniqueProduct.Id} AND ProductSizeId = {ProductSize.Id}", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };
            var IdDB = cmd.ExecuteScalar();

            Id = IdDB != DBNull.Value && IdDB != null ? Convert.ToInt64(IdDB) : throw new Exception($"В базе даных нет модели {UniqueProduct.GetCode()} с разиером: " + productSize.Name);
        }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public UniqueProduct UniqueProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public ProductSize ProductSize { get; set; }

        [Template(Mode = Template.Number)]
        public int ProductCode { get; set; }

        [Common(DisplayName = "Название"), Template(Mode = Template.VisibleString), Db(_Ignore = true)]
        public string FullName => this.GetName();
        #endregion

        public override string GetCaption() => string.Empty;

        public override string GetName() => this.UniqueProduct != null ? this.UniqueProduct.GetName() : string.Empty;

        public string GetCode() => this.UniqueProduct != null && this.UniqueProduct.Id > 0 ? this.UniqueProduct.GetCode() : string.Empty;

        public override object LoadPopupData(long itemId) => PopulateById(itemId);

        public override void SetByFullCode(string[] fullCodeData)
        {
            if (fullCodeData != null && string.IsNullOrEmpty(fullCodeData[0]) is false)
            {
                var cmd = new SqlCommand("SpecificProduct_SetByFullCode", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("ProductCode", SqlDbType.Int) { Value = int.Parse(fullCodeData[0]) });

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                        this.FromDataRow(rdr);

                    rdr.Close();
                }
            }
        }

        public static SpecificProduct PopulateById(long specificProductId)
        {
            var cmd = new SqlCommand("SpecificProduct_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SpecificProductId", SqlDbType.BigInt) { Value = specificProductId });

            var specificProduct = new SpecificProduct();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    specificProduct.FromDataRow(rdr);

                rdr.Close();
            }

            return specificProduct;
        }

        public static SpecificProduct GetByUniqueProductAndSize(long uniqueProductId, long sizeId)
        {
            var cmd = new SqlCommand("GetByUniqueProductAndSize", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = uniqueProductId });
            cmd.Parameters.Add(new SqlParameter("ProductSizeId", SqlDbType.BigInt) { Value = sizeId });

            var SpecificProduct = new SpecificProduct();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    SpecificProduct = (SpecificProduct)new SpecificProduct().FromDataRow(dr);
                }
                dr.Close();
            }
            return SpecificProduct;
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            this.ProductCode = GenerateProductCode();
            base.Insert(item, Comment, connection, user);
        }

        private static int GenerateProductCode(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("SpecificProduct_GenerateProductCode", conn ?? DataBase.ConnectionFromContext());

            object productCodeObj = null;

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    productCodeObj = rdr[nameof(ProductCode)];

                rdr.Close();
            }
            return int.Parse(productCodeObj.ToString());
        }

        public void DeleteByProduct(Product product, long deleteSize, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Delete_SpecificProduct", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("Product", SqlDbType.BigInt) { Value = product.Id });
            cmd.Parameters.Add(new SqlParameter("DeleteSize", SqlDbType.BigInt) { Value = deleteSize });

            cmd.ExecuteReader();
        }

        public void InsertUniqueValidate(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("SpecificProduct_Insert", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = this.UniqueProduct.Id });
            cmd.Parameters.Add(new SqlParameter("ProductSizeId", SqlDbType.BigInt) { Value = this.ProductSize.Id });
            cmd.Parameters.Add(new SqlParameter("ProductCode", SqlDbType.NVarChar, 5) { Value = GenerateProductCode(conn)});
            cmd.Parameters.Add(new SqlParameter("CurrentUserId", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() });

            var objId = cmd.ExecuteScalar();

            this.Id = objId != DBNull.Value ? Convert.ToInt64(objId) : 0;
        }
    }
}