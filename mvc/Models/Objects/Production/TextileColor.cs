using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using LIB.Tools.Utils;
using LIB.Models.Common;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
   , ModulesAccess = (long)(Modulesenum.ControlPanel)
   , DisplayName = "Ткани"
   , SingleName = ""
   , DoCancel = false
   , AllowCreate = false
   , AllowEdit = false
   , AllowDelete = false
   , DefaultQuery = "[Textile].Code TextileCode, [ColorProduct].Code ColorProductCode, ")]
    public class TextileColor : ModelBase
    {
        #region Constructors
        public TextileColor()
            : base(0) { }

        public TextileColor(long id)
            : base(id) { }

        public TextileColor(Textile textile, long colorProductId)
        {
            Textile = textile;
            ColorProduct = new ColorProduct(colorProductId);
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Ткань"), Template(Mode = Template.ParentDropDown)]
        public Textile Textile { get; set; }

        [Common(DisplayName = "Цвет"), Template(Mode = Template.ParentDropDown)]
        public ColorProduct ColorProduct { get; set; }

        [Common(DisplayName = "Количество"), Template(Mode = Template.Decimal)]
        public decimal CurrentCount { get; set; }

        [Common(DisplayName = "Код"), Template(Mode = Template.VisibleString), Db(_Ignore = true)]
        public string Code => this.GetCode();

        [Db(_Ignore = true)]
        public decimal LastPrice { get; set; }
        #endregion

        public string GetCode() => this.Textile?.Code + "-" + this.ColorProduct?.Code;

        public override string GetCaption() => nameof(this.Textile) + nameof(this.Textile.Id);

        public override string GetName() => this.Textile?.GetName();

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Вернутся к оcнове ткани", Href = URLHelper.GetUrl("DocControl/Textile/" + this.Textile.Id), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null) 
            => PopulateById(searchItem.Id);

        public static TextileColor PopulateById(long textileColorId)
        {
            var cmd = new SqlCommand("TextileColor_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileColorId", SqlDbType.BigInt) { Value = textileColorId });

            var ds = new DataSet();
            new SqlDataAdapter { SelectCommand = cmd }.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            var Row = ds.Tables[0].Rows[0];

            var textileColor = (TextileColor)new TextileColor().FromDataRow(Row);

            var lastPriceObj = Row[nameof(LastPrice)];
            textileColor.LastPrice = lastPriceObj != DBNull.Value ? Convert.ToDecimal(lastPriceObj) : default;

            var countObj = Row[nameof(CurrentCount)];
            textileColor.CurrentCount = countObj != DBNull.Value ? Convert.ToDecimal(countObj) : default;

            return textileColor;
        }

        public List<TextileColor> PopulateByParent(Textile textile)
        {
            var cmd = new SqlCommand("TextileColor_PopulateByParent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileId", SqlDbType.BigInt) { Value = textile.Id });

            var textileColors = new List<TextileColor>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var textileColor = (TextileColor)new TextileColor().FromDataRow(dr);
                    textileColors.Add(textileColor);
                }
                dr.Close();
            }

            return textileColors;
        }

        public TextileColor GetByTextileAndColor(long textileId, long colorId)
        {
            var cmd = new SqlCommand("TextileColor_GetByTextileAndColor", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileId", SqlDbType.BigInt) { Value = textileId });
            cmd.Parameters.Add(new SqlParameter("ColorProductId", SqlDbType.BigInt) { Value = colorId });

            var textileColor = new TextileColor();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    textileColor.FromDataRow(dr);

                dr.Close();
            }

            return textileColor;
        }

        public override void SetByFullCode(string[] fullCodeData)
        {
            if (fullCodeData != null && fullCodeData.Length == 2)
            {
                string textileCode = fullCodeData[0],
                       colorCode = fullCodeData[1];

                this.SetByFullCode(textileCode, colorCode);
            }
        }

        public void SetByFullCode(string textileCode, string colorCode)
        {
            var cmd = new SqlCommand("TextileColor_GetByFullCode", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileCode", SqlDbType.NVarChar, 10) { Value = textileCode });
            cmd.Parameters.Add(new SqlParameter("ColorCode", SqlDbType.NVarChar, 10) { Value = colorCode });

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    this.FromDataRow(dr);

                dr.Close();
            }
        }

        public static void DeleteByTextile(Textile textile, long colorId, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("TextileColor_DeleteByTextile", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TextileId", SqlDbType.BigInt) { Value = textile.Id });
            cmd.Parameters.Add(new SqlParameter("ColorId", SqlDbType.BigInt) { Value = colorId });

            cmd.ExecuteNonQuery();
        }
    }
}