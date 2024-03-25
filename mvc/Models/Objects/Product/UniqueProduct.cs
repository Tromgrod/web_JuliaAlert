using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LIB.Models;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Security;
using LIB.Tools.AdminArea;
using LIB.BusinessObjects;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Model
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Модели"
       , SingleName = "Модель"
       , AllowCreate = false
       , AllowCopy = false
       , AllowEdit = false
       , AllowDelete = false
       , LogRevisions = true
       , ViewLink = "DocControl/UniqueProduct"
       , DefaultSimpleSearch = "" +
        " + '|' + COALESCE(CAST([Product].Code + '-' + [ColorProduct].Code + '-' + [Decor].Code as nvarchar(1000)),'')" +
        " + '|' + COALESCE(CAST([Product].Code + ' ' + [ColorProduct].Code + ' ' + [Decor].Code as nvarchar(1000)),'')")]
    public class UniqueProduct : ModelBase
    {
        #region Constructors
        public UniqueProduct()
            : base(0) { }

        public UniqueProduct(long id)
            : base(id) { }

        public UniqueProduct(Product product, long color, long decor, bool enabled = true)
            : base(0)
        {
            this.Product = product;
            this.ColorProduct = new ColorProduct(color);
            this.Decor = new Decor(decor);
            this.Enabled = enabled;
        }

        public UniqueProduct(Product product, ColorProduct colorProduct, Decor decor, SqlConnection conn = null)
            : base(0)
        {
            this.Product = product;
            this.ColorProduct = colorProduct;
            this.Decor = decor;

            var cmd = new SqlCommand($"SELECT UniqueProductId FROM UniqueProduct WHERE ProductId = {Product.Id} AND ColorProductId = {ColorProduct.Id} AND DecorId = {Decor.Id}", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };
            var IdDB = cmd.ExecuteScalar();

            Id = IdDB != DBNull.Value && IdDB != null ? Convert.ToInt64(IdDB) : throw new Exception($"В базе даных нет модели с кодом: " + this.GetCode());
        }
        #endregion

        #region Properties
        public Product Product { get; set; }

        [Common(DisplayName = "Цвет модели"), Template(Mode = Template.ParentDropDown)]
        public ColorProduct ColorProduct { get; set; }

        [Common(DisplayName = "Декор"), Template(Mode = Template.ParentDropDown)]
        public Decor Decor { get; set; }

        [Common(DisplayName = "Изображение"), Template(Mode = Template.Image), Image(ThumbnailWidth = 615, ThumbnailHeight = 630)]
        public Graphic Image { get; set; }

        [Common(DisplayName = "Состоит из"), Template(Mode = Template.ParentDropDown)]
        public Compound Compound { get; set; }

        [Common(DisplayName = "Показывать"), Template(Mode = Template.CheckBox)]
        public bool Enabled { get; set; }

        [Common(DisplayName = "Текущая цена фабрики"), Db(_Ignore = true)]
        public decimal LastFactoryPrice => GetLastFactoryPrice();

        [Common(DisplayName = "Последние место пошива"), Db(_Ignore = true)]
        public Factory LastFactoryTailoring => LastTailoringSupplySpecificProductUnit.FactoryTailoring;

        [Common(DisplayName = "Стоимость пошива"), Db(_Ignore = true)]
        public decimal LastTailoringCost => LastTailoringSupplySpecificProductUnit.TailoringCost;

        [Common(DisplayName = "Последние место кроя"), Db(_Ignore = true)]
        public Factory LastFactoryCut => LastSupplySpecificProductUnit.FactoryCut;

        [Common(DisplayName = "Последняя стоимость кроя"), Db(_Ignore = true)]
        public decimal LastCutCost => LastSupplySpecificProductUnit.CutCost;

        private SupplySpecificProductUnit _LastSupplySpecificProductUnit;
        [Db(_Ignore = true)]
        public SupplySpecificProductUnit LastSupplySpecificProductUnit
        {
            get
            {
                if (_LastSupplySpecificProductUnit == null || _LastSupplySpecificProductUnit.Id <= 0)
                    _LastSupplySpecificProductUnit = SupplySpecificProductUnit.GetLastByParrent(this);

                return _LastSupplySpecificProductUnit;
            }
        }

        private TailoringSupplySpecificProductUnit _LastTailoringSupplySpecificProductUnit;
        [Db(_Ignore = true)]
        public TailoringSupplySpecificProductUnit LastTailoringSupplySpecificProductUnit
        {
            get
            {
                if (_LastTailoringSupplySpecificProductUnit == null || _LastTailoringSupplySpecificProductUnit.Id <= 0)
                    _LastTailoringSupplySpecificProductUnit = TailoringSupplySpecificProductUnit.GetLastByParrent(LastSupplySpecificProductUnit);

                return _LastTailoringSupplySpecificProductUnit;
            }
        }

        [Db(_Ignore = true)]
        public List<SpecificProduct> SpecificProducts { get; set; }
        #endregion

        public decimal GetProductionPrice() => this.GetLastPriceConsumables() + this.LastTailoringCost + this.LastCutCost + GetLast<ProductionExpense>().Expense;

        public decimal GetLastFactoryPrice() => this.GetProductionPrice() / 0.75m;

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            this.Id = searchItem.Id;
            return this.PopulateById();
        }

        public override string GetImageName() => nameof(this.Image);

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>
            {
                new LinkModel() { Caption = "Назад к основе модели", Href = URLHelper.GetUrl("DocControl/Product/" + Product.Id), Class = "button" }
            };

            return Breadcrumbs;
        }

        public override string GetCaption() => string.Empty;

        public override string GetName()
        {
            var Product = this.Product;
            var ColorProduct = this.ColorProduct;
            var Decor = this.Decor;

            if (this.Id == 0)
            {
                return "";
            }
            else if (Product == null || ColorProduct == null || Decor == null ||
                string.IsNullOrEmpty(Product.Name) || string.IsNullOrEmpty(ColorProduct.Name) || string.IsNullOrEmpty(Decor.Name))
            {
                var UniqueProduct = this.PopulateById();

                if (UniqueProduct == null)
                    return "";

                Product = UniqueProduct.Product;
                ColorProduct = UniqueProduct.ColorProduct;
                Decor = UniqueProduct.Decor;
            }

            return Regex.Replace(Product.Name + " " + ColorProduct.Name.ToLower() + " " + Decor.Name.ToLower().Replace("-", ""), @"\s+", " ");
        }

        public override string GetAutocompleteName() => this.GetName() + " (" + this.GetCode() + ")";

        public string GetCode()
        {
            if (this.Id == 0 && Product.Id == 0 && ColorProduct.Id == 0 && Decor.Id == 0)
            {
                return "";
            }
            else if (Product == null || ColorProduct == null || Decor == null ||
                string.IsNullOrEmpty(Product.Code) || string.IsNullOrEmpty(ColorProduct.Code) || string.IsNullOrEmpty(Decor.Code))
            {
                var UniqueProduct = this.PopulateById();

                if (UniqueProduct == null)
                    return "";
                else
                    return UniqueProduct.GetCode();
            }
            else
                return this.Product.Code + '-' + this.ColorProduct.Code + '-' + this.Decor.Code;
        }

        public override Dictionary<long, ItemBase> Populate(ItemBase item = null, SqlConnection conn = null, bool sortByName = false, string AdvancedFilter = "", bool ShowCanceled = false, User sUser = null, bool ignoreQueryFilter = false)
        {
            var cmd = new SqlCommand("Populate_UniqueProduct", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

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

        public static List<UniqueProduct> PopulateAll()
        {
            var cmd = new SqlCommand("UniqueProduct_PopulateAll", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var uniqueProducts = new List<UniqueProduct>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var uniqueProduct = (UniqueProduct)new UniqueProduct().FromDataRow(dr);
                    uniqueProducts.Add(uniqueProduct);
                }
                dr.Close();
            }

            return uniqueProducts;
        }

        public void InsertUniqueValidate(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("UniqueProduct_Insert", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ProductId", SqlDbType.BigInt) { Value = this.Product.Id });
            cmd.Parameters.Add(new SqlParameter("ColorProductId", SqlDbType.BigInt) { Value = this.ColorProduct.Id });
            cmd.Parameters.Add(new SqlParameter("DecorId", SqlDbType.BigInt) { Value = this.Decor.Id });
            cmd.Parameters.Add(new SqlParameter("CurrentUserId", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() });

            if (this.Compound != null && this.Compound.Id > 0)
            {
                cmd.Parameters.Add(new SqlParameter("CompoundId", SqlDbType.BigInt) { Value = this.Compound.Id });
            }

            var objId = cmd.ExecuteScalar();

            this.Id = objId != DBNull.Value ? Convert.ToInt64(objId) : 0;
        }

        public static Dictionary<long, UniqueProduct> PopulateByProduct(Product product, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Populate_UniqueProduct_ByProduct", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ProductId", SqlDbType.BigInt) { Value = product.Id });

            var UniqueProducts = new Dictionary<long, UniqueProduct>();

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

        public decimal GetLastPriceConsumables()
        {
            var cmd = new SqlCommand("UniqueProduct_GetLastPriceConsumables", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("UniqueProductId", SqlDbType.BigInt) { Value = this.Id });

            var priceObj = cmd.ExecuteScalar();

            return priceObj != null && priceObj != DBNull.Value ? Convert.ToDecimal(priceObj) : default;
        }

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Populate_UniqueProduct", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

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

        public Dictionary<long, ItemBase> GetSizes()
        {
            var cmd = new SqlCommand("UniqueProduct_GetSizes", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UniqueProductId", SqlDbType.BigInt) { Value = this.Id });

            var productSizes = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var productSize = (ProductSize)new ProductSize().FromDataRow(dr);
                    productSizes.Add(productSize.Id, productSize);
                }

                dr.Close();
            }
            return productSizes;
        }

        public UniqueProduct PopulateById()
        {
            var cmd = new SqlCommand("Populate_UniqueProduct_ById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UniqueProductId", SqlDbType.BigInt) { Value = this.Id });

            var ds = new DataSet();
            var da = new SqlDataAdapter { SelectCommand = cmd };
            da.Fill(ds);

            ds.Tables[0].TableName = "UniqueProduct";
            ds.Tables[1].TableName = "SpecificProducts";

            if (ds.Tables["UniqueProduct"].Rows.Count == 0)
                return this;

            UniqueProduct UniqueProduct = (UniqueProduct)new UniqueProduct().FromDataRow(ds.Tables["UniqueProduct"].Rows[0]);

            UniqueProduct.SpecificProducts = new List<SpecificProduct>();

            foreach (DataRow dr in ds.Tables["SpecificProducts"].Rows)
            {
                var specificProduct = (SpecificProduct)new SpecificProduct().FromDataRow(dr);
                UniqueProduct.SpecificProducts.Add(specificProduct);
            }

            return UniqueProduct;
        }

        public void InsertColor(long insertColor, Product product, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("InsertColor", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@InsertColor", SqlDbType.BigInt) { Value = insertColor });
            cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.BigInt) { Value = product.Id });
            cmd.Parameters.Add(new SqlParameter("@CurrentUser", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() });

            cmd.ExecuteReader();
        }

        public void InsertDecor(long insertDecor, Product product, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("InsertDecor", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@InsertDecor", SqlDbType.BigInt) { Value = insertDecor });
            cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.BigInt) { Value = product.Id });
            cmd.Parameters.Add(new SqlParameter("@CurrentUser", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() });

            cmd.ExecuteReader();
        }

        public override void Update(ItemBase item, LIB.AdvancedProperties.DisplayMode DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var cmd = new SqlCommand("UniqueProduct_Update", connection ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UniqueProductId", SqlDbType.BigInt) { Value = this.Id });
            cmd.Parameters.Add(new SqlParameter("@CompoundId", SqlDbType.BigInt) { Value = this.Compound.Id });

            cmd.ExecuteNonQuery();
        }
    }
}