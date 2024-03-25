using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Data;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Models.Common;
using LIB.Models;
using LIB.Helpers;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class Textile : ModelBase
    {
        #region Constructors
        public Textile()
            : base(0) { }

        public Textile(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Template(Mode = Template.VisibleString)]
        public string Code { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Compound Compound { get; set; }

        [Image(ThumbnailWidth = 615, ThumbnailHeight = 630)]
        public Graphic Image { get; set; }

        [Db(_Ignore = true)]
        public List<ColorProduct> Colors { get; set; }

        [Db(_Ignore = true)]
        public decimal LastPrice { get; set; }

        [Db(_Ignore = true)]
        public decimal TotalCount { get; set; }
        #endregion

        public override string GetName() => this.Name;

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Добавить новую ткань", Href = URLHelper.GetUrl("DocControl/Textile"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            this.Id = searchItem.Id;
            return PopulateById(conn);
        }

        public Dictionary<long, ItemBase> GetColors()
        {
            var cmd = new SqlCommand("Textile_GetColors", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@TextileId", SqlDbType.BigInt) { Value = this.Id });

            var colorProducts = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var colorProduct = (ColorProduct)new ColorProduct().FromDataRow(dr);
                    colorProducts.Add(colorProduct.Id, colorProduct);
                }

                dr.Close();
            }
            return colorProducts;
        }

        public Textile PopulateById(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Populate_Textile", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileId", SqlDbType.BigInt) { Value = this.Id });

            var ds = new DataSet();
            new SqlDataAdapter { SelectCommand = cmd }.Fill(ds);

            ds.Tables[0].TableName = "Textile";
            ds.Tables[1].TableName = "Colors";

            if (ds.Tables["Textile"].Rows.Count == 0)
                return this;

            var Textile = (Textile)new Textile().FromDataRow(ds.Tables["Textile"].Rows[0]);

            var lastPriceObj = ds.Tables["Textile"].Rows[0][nameof(this.LastPrice)];
            Textile.LastPrice = lastPriceObj != DBNull.Value ? Convert.ToDecimal(lastPriceObj) : default;

            var totalCountObj = ds.Tables["Textile"].Rows[0][nameof(this.TotalCount)];
            Textile.TotalCount = totalCountObj != DBNull.Value ? Convert.ToDecimal(totalCountObj) : default;

            Textile.Colors = new List<ColorProduct>();

            foreach (DataRow dr in ds.Tables["Colors"].Rows)
            {
                var Color = (ColorProduct)new ColorProduct().FromDataRow(dr);
                Textile.Colors.Add(Color);
            }

            return Textile;
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var colors = HttpContext.Current.Request.Form[nameof(ColorProduct)]
                    .Split(',')
                    .Select(cp => long.Parse(cp.Trim()));

            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection conn = DataBase.CreateNewConnection())
                {
                    try
                    {
                        if (this.Id > 0)
                        {
                            base.Update(this, connection: conn);
                            this.Update(colors, conn);
                        }
                        else
                        {
                            base.Insert(this, connection: conn);
                            this.Insert(colors, conn);
                        }
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

        private void Update(IEnumerable<long> formColor, SqlConnection conn)
        {
            var pattern = this.PopulateById(conn);

            var colorFromDB = pattern.Colors.Select(c => c.Id);

            var deleteColors = colorFromDB.Except(formColor);

            var insertColors = formColor.Except(colorFromDB);

            foreach (var deleteSize in deleteColors)
            {
                TextileColor.DeleteByTextile(this, deleteSize, conn);
            }

            foreach (var insertColor in insertColors)
            {
                var textileColor = new TextileColor(this, insertColor);
                textileColor.Insert(textileColor, connection: conn);
            }
        }

        private void Insert(IEnumerable<long> colors, SqlConnection conn)
        {
            foreach (var color in colors)
            {
                var textileColor = new TextileColor(this, color);
                textileColor.Insert(textileColor, connection: conn);
            }
        }

        public override RequestResult SaveForm() => new RequestResult()
        {
            Result = RequestResultType.Reload,
            RedirectURL = URLHelper.GetUrl("DocControl/" + this.GetType().Name + "/" + Id.ToString()),
            Message = "Данные успешно сохранены"
        };
    }
}