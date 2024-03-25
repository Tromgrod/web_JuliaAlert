using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.Tools.Utils;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Model
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Цвета моделей"
       , SingleName = "Цвет модели"
       , DoCancel = true
       , LogRevisions = true)]
    public class ColorProduct : ItemBase
    {
        #region Constructors
        public ColorProduct()
            : base(0) { }

        public ColorProduct(long id)
            : base(id) { }

        public ColorProduct(string code, SqlConnection conn = null)
            : base(code, nameof(Code), true, conn) 
        {
            Code = code;
        }
        #endregion

        #region Properties
        [Common(DisplayName = "Цвет"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Код цвета"), Template(Mode = Template.VisibleString), Db(Sort = DbSortMode.Asc)]
        public string Code { get; set; }
        #endregion

        public static Dictionary<long, ItemBase> PopulateByType<T>(T item) where T: ItemBase
        {
            var cmdStr =
                $"SELECT ColorProductId, Name, Code " +
                $"FROM {item.GetType().Name} obj " +
                $"JOIN ColorProduct cp ON cp.ColorProductId = obj.ColorProductId AND cp.DeletedBy IS NULL" +
                $"WHERE obj.DeletedBy IS NULL AND obj.{item.GetType().Name + nameof(item.Id)} = {item.Id}";

            var cmd = new SqlCommand(cmdStr, DataBase.ConnectionFromContext());

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

        public override string GetName() => this.Name + (string.IsNullOrEmpty(this.Code) ? "" : " (" + this.Code + ")");
    }
}