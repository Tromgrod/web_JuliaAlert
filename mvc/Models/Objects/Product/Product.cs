using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using LIB.Tools.Utils;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Helpers;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = false)]
    public class Product : ModelBase
    {
        #region Constructors
        public Product()
            : base(0) { }

        public Product(long id)
            : base(id) { }

        public Product(string code, SqlConnection conn = null)
            : base(code, nameof(Code), true, conn)
        {
            Code = code;
        }
        #endregion

        #region Properties
        [Template(Mode = Template.String)]
        public string Name { get; set; }

        [Template(Mode = Template.String)]
        public string Code { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public TypeProduct TypeProduct { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Pattern Pattern { get; set; }

        [Db(_Ignore = true)]
        public List<ColorProduct> Colors { get; set; }

        [Db(_Ignore = true)]
        public List<ProductSize> Sizes { get; set; }

        [Db(_Ignore = true)]
        public List<Decor> Decors { get; set; }
        #endregion

        public override string GetName() => this.Name;

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Product_Populate_Autocomplete", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            var Products = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var Product = (Product)new Product().FromDataRow(dr);
                    Products.Add(Product.Id, Product);
                }
                dr.Close();
            }

            return Products;
        }

        public override bool HaveAccess(string fullModel, string Id)
        {
            var currentUser = LIB.Tools.Security.Authentication.GetCurrentUser();

            return string.IsNullOrEmpty(Id) is false || currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            var currentUser = LIB.Tools.Security.Authentication.GetCurrentUser();
            bool productionAccess = currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);

            if (productionAccess && this.Id > 0)
                Breadcrumbs.Add(new LinkModel() { Caption = "Добавить новую модель", Href = URLHelper.GetUrl("DocControl/Product"), Class = "button" });

            if (this.Pattern?.Id > 0)
                Breadcrumbs.Add(new LinkModel() { Caption = "Перейти к лекалу", Href = URLHelper.GetUrl("DocControl/Pattern/" + this.Pattern.Id), Class = "button" });

            return Breadcrumbs;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            this.Id = searchItem.Id;
            return PopulateOne(conn);
        }

        public Product PopulateOne(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Populate_Product", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ProductId", SqlDbType.BigInt) { Value = this.Id });

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            ds.Tables[0].TableName = "Product";

            ds.Tables[1].TableName = "Colors";
            ds.Tables[2].TableName = "Sizes";
            ds.Tables[3].TableName = "Decors";

            if (ds.Tables["Product"].Rows.Count == 0)
                return this;

            var Product = (Product)new Product().FromDataRow(ds.Tables["Product"].Rows[0]);

            Product.Colors = new List<ColorProduct>();
            Product.Sizes = new List<ProductSize>();
            Product.Decors = new List<Decor>();

            foreach (DataRow dr in ds.Tables["Colors"].Rows)
            {
                var Color = (ColorProduct)new ColorProduct().FromDataRow(dr);
                Product.Colors.Add(Color);
            }

            foreach (DataRow dr in ds.Tables["Sizes"].Rows)
            {
                var Size = (ProductSize)new ProductSize().FromDataRow(dr);
                Product.Sizes.Add(Size);
            }

            foreach (DataRow dr in ds.Tables["Decors"].Rows)
            {
                var Decor = (Decor)new Decor().FromDataRow(dr);
                Product.Decors.Add(Decor);
            }

            return Product;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var sizes = HttpContext.Current.Request.Form[nameof(ProductSize)]
                    .Split(',')
                    .Select(ps => Convert.ToInt64(ps.Replace(" ", "")));

            var colors = HttpContext.Current.Request.Form[nameof(ColorProduct)]
                    .Split(',')
                    .Select(cp => Convert.ToInt64(cp.Replace(" ", "")));

            var decors = HttpContext.Current.Request.Form[nameof(Decor)]
                    .Split(',')
                    .Select(ps => Convert.ToInt64(ps.Replace(" ", "")));

            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection conn = DataBase.CreateNewConnection())
                {
                    try
                    {
                        if (this.Id > 0)
                        {
                            base.Update(this, connection: conn);
                            this.UpdateUniqueModel(sizes, colors, decors, conn);
                        }
                        else
                        {
                            base.Insert(this, connection: conn);
                            this.InsertUniqueModel(sizes, colors, decors, conn: conn);
                        }

                        this.ChangeProductPriceIntoEachSalesChannel(conn);
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }

                    scope.Complete();
                }
            }
        }

        private void ChangeProductPriceIntoEachSalesChannel(SqlConnection conn)
        {
            var salesChannels = new SalesChannel().Populate(conn: conn).Values;

            foreach (SalesChannel salesChannel in salesChannels)
            {
                if (decimal.TryParse(HttpContext.Current.Request.Form[$"Price_{salesChannel.GetName()}"], out decimal price) && price > 0)
                {
                    var ProductPrice = new ProductPrice
                    {
                        Product = this,
                        SalesChannel = salesChannel,
                        Price = price
                    };

                    ProductPrice.Insert(ProductPrice, connection: conn);
                }
            }
        }

        public void InsertUniqueModel(IEnumerable<long> sizes, IEnumerable<long> colors, IEnumerable<long> decors, Compound compound = null, SqlConnection conn = null)
        {
            foreach (var color in colors)
            {
                foreach (var decor in decors)
                {
                    var UniqueProduct = new UniqueProduct(this, color, decor, true)
                    {
                        Compound = compound
                    };

                    UniqueProduct.InsertUniqueValidate(conn);

                    if (UniqueProduct.Id > 0)
                    {
                        foreach (var size in sizes)
                        {
                            var specificProduct = new SpecificProduct
                            {
                                UniqueProduct = UniqueProduct,
                                ProductSize = new ProductSize(size)
                            };

                            specificProduct.InsertUniqueValidate(conn);
                        }
                    }
                }
            }
        }

        private void UpdateUniqueModel(IEnumerable<long> formSize, IEnumerable<long> formColor, IEnumerable<long> formDecor, SqlConnection conn = null)
        {
            var product = this.PopulateOne(conn);

            var sizeFromDB = product.Sizes.Select(s => s.Id);
            var colorFromDB = product.Colors.Select(c => c.Id);
            var decorFromDB = product.Decors.Select(d => d.Id);

            var deleteSizes = sizeFromDB.Except(formSize);
            var deleteColors = colorFromDB.Except(formColor);
            var deleteDecors = decorFromDB.Except(formDecor);

            var insertSizes = formSize.Except(sizeFromDB);
            var insertColors = formColor.Except(colorFromDB);
            var insertDecors = formDecor.Except(decorFromDB);

            var anyColor = colorFromDB.Intersect(formColor);
            var anyDecor = decorFromDB.Intersect(formDecor);
            var anySize = sizeFromDB.Intersect(formSize);

            foreach (var deleteColor in deleteColors)
            {
                var DeleteItems = this.GetUniqueProductFromDB(deleteColor, nameof(ColorProduct), conn);
                new UniqueProduct().Delete(DeleteItems, connection: conn);
            }

            foreach (var deleteDecor in deleteDecors)
            {
                var DeleteItems = this.GetUniqueProductFromDB(deleteDecor, nameof(Decor), conn);
                new UniqueProduct().Delete(DeleteItems, connection: conn);
            }

            foreach (var deleteSize in deleteSizes)
            {
                new SpecificProduct().DeleteByProduct(this, deleteSize, conn);
            }

            foreach (var insertColor in insertColors)
            {
                new UniqueProduct().InsertColor(insertColor, this, conn);
            }

            foreach (var insertDecor in insertDecors)
            {
                new UniqueProduct().InsertDecor(insertDecor, this, conn);
            }

            var UniqueProductList = UniqueProduct.PopulateByProduct(this, conn).Values;
            foreach (UniqueProduct uniqueProduct in UniqueProductList)
            {
                foreach (var insertSize in insertSizes)
                {
                    var productSize = new ProductSize(insertSize);
                    new SpecificProduct(uniqueProduct, productSize).InsertUniqueValidate(conn);
                }
            }

            if (anyColor.Count() == 0 || anyDecor.Count() == 0 || anySize.Count() == 0)
            {
                this.InsertUniqueModel
                (
                    insertSizes.Count() > 0 ? insertSizes : formSize,
                    insertColors.Count() > 0 ? insertColors : formColor,
                    insertDecors.Count() > 0 ? insertDecors : formDecor,
                    conn: conn
                );
            }
        }

        private Dictionary<long, ItemBase> GetUniqueProductFromDB(long insertValue, string typeInsert, SqlConnection conn = null)
        {
            var cmd = new SqlCommand($"SELECT * FROM UniqueProduct WHERE ProductId = {this.Id} AND {typeInsert}Id = {insertValue}", conn ?? DataBase.ConnectionFromContext());

            var UniqueProducts = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var UniqueProduct = (UniqueProduct)new UniqueProduct().FromDataRow(dr);
                    UniqueProducts.Add(UniqueProduct.Id, UniqueProduct);
                }
                dr.Close();
            }
            return UniqueProducts;
        }

        public override RequestResult SaveForm() => new RequestResult()
        {
            Result = RequestResultType.Reload,
            RedirectURL = URLHelper.GetUrl("DocControl/" + this.GetType().Name + "/" + Id.ToString()),
            Message = "Данные успешно сохранены"
        };

        public bool IsUniqueCode(SqlConnection conn = null)
        {
            var cmd = new SqlCommand($"SELECT CASE WHEN COUNT(ProductId) > 0 THEN 0 ELSE 1 END FROM Product WHERE Code = N'{this.Code}' AND DeletedBy IS NULL", conn ?? DataBase.ConnectionFromContext());

            var boolObj = cmd.ExecuteScalar();

            return boolObj != DBNull.Value && Convert.ToBoolean(boolObj);
        }

        public decimal GetMaxEstimatedPriceBySalesChannel(SalesChannel salesChannel)
        {
            var UniqueProducts = UniqueProduct.PopulateByProduct(this).Values;

            var lastSalesChannelCoefficient = SalesChannelCoefficient.GetLastBySalesChannel(salesChannel);

            decimal maxEstimated = default;

            if (UniqueProducts != null && UniqueProducts.Count > 0)
                maxEstimated = UniqueProducts.Max(up => salesChannel.GetEstimatedPrice(up.LastFactoryPrice, lastSalesChannelCoefficient));

            return maxEstimated;
        }
    }
}