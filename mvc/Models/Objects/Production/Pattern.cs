using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Models.Common;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using User = LIB.BusinessObjects.User;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class Pattern : ModelBase
    {
        #region Constructors
        public Pattern()
            : base(0) { }

        public Pattern(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.String)]
        public string Name { get; set; }

        [Template(Mode = Template.String)]
        public string Code { get; set; }

        [Template(Mode = Template.VisibleString), Db(_Ignore = true)]
        public string FullCode => string.Empty;

        [Template(Mode = Template.ParentDropDown)]
        public Constructor Constructor { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Collection Collection { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public LocationStorage LocationStorage { get; set; }

        [Template(Mode = Template.Image), Image(ThumbnailWidth = 615, ThumbnailHeight = 630)]
        public Graphic Image { get; set; }
        #endregion

        public override string GetName() => this.Name;

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var Breadcrumbs = new List<LinkModel>();

            if (this.Id > 0)
            {
                Breadcrumbs = new List<LinkModel>
                {
                    new LinkModel() { Caption = "Добавить новую лекалу", Href = URLHelper.GetUrl("DocControl/Pattern"), Class = "button" }
                };
            }

            return Breadcrumbs;
        }

        public override ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            this.Id = searchItem.Id;
            return PopulateOne(conn);
        }

        public Pattern PopulateOne(SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Populate_Pattern", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("PatternId", SqlDbType.BigInt) { Value = this.Id });

            var pattern = new Pattern();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (dr.Read())
                    pattern.FromDataRow(dr);

                dr.Close();
            }

            return pattern;
        }
    }
}