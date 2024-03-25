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
using LIB.Models;
using LIB.Helpers;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class Finding : ModelBase
    {
        #region Constructors
        public Finding()
            : base(0) { }

        public Finding(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название"), Template(Mode = Template.Name), Db(_Ignore = true)]
        public string Name => this.GetName();

        [Common(DisplayName = "Изображение"), Template(Mode = Template.Image), Image(ThumbnailWidth = 615, ThumbnailHeight = 630)]
        public Graphic Image { get; set; }

        [Common(DisplayName = "Подвид фурнитуры"), Template(Mode = Template.ParentDropDown)]
        public FindingSubspecie FindingSubspecie { get; set; }

        [Common(DisplayName = "Код"), Template(Mode = Template.VisibleString), Db(_Ignore = true)]
        public string Code => this.GetCode();

        [Db(_Ignore = true)]
        public List<ColorProduct> Colors { get; set; }

        [Db(_Ignore = true)]
        public decimal LastPrice { get; set; }

        [Db(_Ignore = true)]
        public decimal TotalCount { get; set; }
        #endregion

        public string GetCode() => this.FindingSubspecie?.FindingSpecie?.Code + "-" + this.FindingSubspecie?.Code;

        public override string GetName() => this.FindingSubspecie?.FindingSpecie?.GetName() + " " + this.FindingSubspecie?.GetName();

        public override string GetCaption() => nameof(this.FindingSubspecie) + nameof(this.FindingSubspecie.Id);

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Добавить новую фурнитуру", Href = URLHelper.GetUrl("DocControl/Finding"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Finding_PopulateAutocomplete", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            var findings = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var finding = (Finding)new Finding().FromDataRow(dr);
                    findings.Add(finding.Id, finding);
                }
                dr.Close();
            }

            return findings;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            this.Id = searchItem.Id;
            return PopulateById(conn);
        }

        public Dictionary<long, ItemBase> GetColors()
        {
            var cmd = new SqlCommand("Finding_GetColors", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@FindingId", SqlDbType.BigInt) { Value = this.Id });

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

        public Finding PopulateById(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Populate_Finding", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingId", SqlDbType.BigInt) { Value = this.Id });

            var ds = new DataSet();
            new SqlDataAdapter { SelectCommand = cmd }.Fill(ds);

            ds.Tables[0].TableName = "Finding";
            ds.Tables[1].TableName = "Colors";

            if (ds.Tables["Finding"].Rows.Count == 0)
                return this;

            var Finding = (Finding)new Finding().FromDataRow(ds.Tables["Finding"].Rows[0]);

            var lastPriceObj = ds.Tables["Finding"].Rows[0][nameof(this.LastPrice)];
            Finding.LastPrice = lastPriceObj != DBNull.Value ? Convert.ToDecimal(lastPriceObj) : default;

            var totalCountObj = ds.Tables["Finding"].Rows[0][nameof(this.TotalCount)];
            Finding.TotalCount = totalCountObj != DBNull.Value ? Convert.ToDecimal(totalCountObj) : default;

            Finding.Colors = new List<ColorProduct>();

            foreach (DataRow dr in ds.Tables["Colors"].Rows)
            {
                var Color = (ColorProduct)new ColorProduct().FromDataRow(dr);
                Finding.Colors.Add(Color);
            }

            return Finding;
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
                FindingColor.DeleteByFinding(this, deleteSize, conn);
            }

            foreach (var insertColor in insertColors)
            {
                var findingColor = new FindingColor(this, insertColor);
                findingColor.Insert(findingColor, connection: conn);
            }
        }

        private void Insert(IEnumerable<long> colors, SqlConnection conn)
        {
            foreach (var color in colors)
            {
                var findingColor = new FindingColor(this, color);
                findingColor.Insert(findingColor, connection: conn);
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