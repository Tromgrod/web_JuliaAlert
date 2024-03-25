using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Collections.Generic;
using System.Data.SqlClient;
using LIB.Tools.Utils;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.ModelType
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Типы моделей"
       , SingleName = "Тип модели"
       , DoCancel = false
       , LogRevisions = true)]
    public class TypeProduct : ItemBase
    {
        #region Constructors
        public TypeProduct()
            : base(0) { }

        public TypeProduct(long id)
            : base(id) { }

        public TypeProduct(string name, SqlConnection conn)
            : base(name, conn: conn) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Группа модели"), Template(Mode = Template.ParentDropDown)]
        public GroupProduct GroupProduct { get; set; }

        [Common(DisplayName = "Тип модели"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion

        public static Dictionary<long, ItemBase> PopulateByGroupProduct(long groupProductId)
        {
            var cmd = new SqlCommand("TypeProduct_PopulateByGroupProduct", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@GroupProductId", SqlDbType.BigInt) { Value = groupProductId });

            var typeProducts = new Dictionary<long, ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var typeProduct = (TypeProduct)(new TypeProduct().FromDataRow(rdr));
                    typeProducts.Add(typeProduct.Id, typeProduct);
                }
                rdr.Close();
            }
            return typeProducts;
        }
    }
}