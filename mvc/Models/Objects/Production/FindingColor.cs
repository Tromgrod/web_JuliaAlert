using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using JuliaAlertLib.BusinessObjects;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Models;
using LIB.Tools.AdminArea;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
   , ModulesAccess = (long)(Modulesenum.ControlPanel)
   , DisplayName = "Фурнитуры"
   , SingleName = ""
   , DoCancel = false
   , AllowCreate = false
   , AllowEdit = false
   , AllowImport = false
   , AllowDelete = false
   , DefaultQuery = "fsubs.FindingSubspecieId, fs.FindingSpecieId, fsubs.Name FindingSubspecieName, fs.Name FindingSpecieName, fsubs.Code FindingSubspecieCode, fs.Code FindingSpecieCode, [ColorProduct].Code ColorProductCode, "
   , AdditionalJoin = " JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = [Finding].FindingSubspecieId JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId")]
    public class FindingColor : ModelBase
    {
        #region Constructors
        public FindingColor()
            : base(0) { }

        public FindingColor(long id)
            : base(id) { }

        public FindingColor(Finding finding, long colorProductId)
        {
            Finding = finding;
            ColorProduct = new ColorProduct(colorProductId);
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Фурнитура"), Template(Mode = Template.ParentDropDown)]
        public Finding Finding { get; set; }

        [Common(DisplayName = "Цвет"), Template(Mode = Template.ParentDropDown)]
        public ColorProduct ColorProduct { get; set; }

        [Common(DisplayName = "Код"), Template(Mode = Template.VisibleString), Db(_Ignore = true)]
        public string Code => this.GetCode();

        [Db(_Ignore = true)]
        public decimal LastPrice { get; set; }

        [Db(_Ignore = true)]
        public decimal CurrentCount { get; set; }
        #endregion

        public string GetCode() => this.Finding?.FindingSubspecie?.FindingSpecie?.Code + "-" + this.Finding?.FindingSubspecie?.Code + "-" + this.ColorProduct?.Code;

        public override string GetCaption() => nameof(this.Finding) + nameof(this.Finding.Id);

        public override string GetName() => this.Finding?.GetName();

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Вернутся к оcнове фурнитуры", Href = URLHelper.GetUrl("DocControl/Finding/" + this.Finding.Id), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, LIB.BusinessObjects.User sUser = null, SqlConnection conn = null)
        {
            return PopulateById(searchItem.Id);
        }

        public static FindingColor PopulateById(long findingColorId)
        {
            var cmd = new SqlCommand("FindingColor_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingColorId", SqlDbType.BigInt) { Value = findingColorId });

            var ds = new DataSet();
            new SqlDataAdapter { SelectCommand = cmd }.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            var Row = ds.Tables[0].Rows[0];

            var findingColor = (FindingColor)new FindingColor().FromDataRow(Row);

            var lastPriceObj = Row[nameof(LastPrice)];
            findingColor.LastPrice = lastPriceObj != DBNull.Value ? Convert.ToDecimal(lastPriceObj) : default;

            return findingColor;
        }

        public FindingColor GetByFindingAndColor(long findingId, long colorId)
        {
            var cmd = new SqlCommand("FindingColor_GetByFindingAndColor", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingId", SqlDbType.BigInt) { Value = findingId });
            cmd.Parameters.Add(new SqlParameter("ColorProductId", SqlDbType.BigInt) { Value = colorId });

            var findingColor = new FindingColor();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    findingColor.FromDataRow(dr);

                dr.Close();
            }

            return findingColor;
        }

        public List<FindingColor> PopulateByParent(Finding finding)
        {
            var cmd = new SqlCommand("FindingColor_PopulateByParent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingId", SqlDbType.BigInt) { Value = finding.Id });

            var findingColors = new List<FindingColor>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var findingColor = (FindingColor)new FindingColor().FromDataRow(dr);
                    findingColors.Add(findingColor);
                }
                dr.Close();
            }

            return findingColors;
        }

        public override void SetByFullCode(string[] fullCodeData)
        {
            if (fullCodeData != null && fullCodeData.Length == 3)
            {
                string findingSpecieCode = fullCodeData[0],
                       findingSubspecieCode = fullCodeData[1],
                       colorCode = fullCodeData[2];

                this.SetByFullCode(findingSpecieCode, findingSubspecieCode, colorCode);
            }
        }

        public void SetByFullCode(string findingSpecieCode, string findingSubspecieCode, string colorCode)
        {
            var cmd = new SqlCommand("FindingColor_GetByFullCode", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingSpecieCode", SqlDbType.NVarChar, 10) { Value = findingSpecieCode });
            cmd.Parameters.Add(new SqlParameter("FindingSubspecieCode", SqlDbType.NVarChar, 10) { Value = findingSubspecieCode });
            cmd.Parameters.Add(new SqlParameter("ColorCode", SqlDbType.NVarChar, 10) { Value = colorCode });

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                {
                    this.FromDataRow(dr);

                    var lastPriceObj = dr[nameof(this.LastPrice)];
                    this.LastPrice = lastPriceObj != DBNull.Value ? Convert.ToDecimal(lastPriceObj) : default;
                }


                dr.Close();
            }
        }

        public static void DeleteByFinding(Finding finding, long colorId, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("FindingColor_DeleteByFinding", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingId", SqlDbType.BigInt) { Value = finding.Id });
            cmd.Parameters.Add(new SqlParameter("ColorId", SqlDbType.BigInt) { Value = colorId });

            cmd.ExecuteNonQuery();
        }
    }
}