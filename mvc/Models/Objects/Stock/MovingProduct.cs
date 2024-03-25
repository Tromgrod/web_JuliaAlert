using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.BusinessObjects;
using LIB.Tools.Security;
using JuliaAlertLib.BusinessObjects;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(DoCancel = true, LogRevisions = true)]
    public class MovingProduct : ModelBase
    {
        #region Constructors
        public MovingProduct()
            : base(0) { }

        public MovingProduct(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string DocumentNumber { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.Description)]
        public string Description { get; set; }
        #endregion

        public override string GetName() => this.DocumentNumber;

        public override string GetCaption() => nameof(this.DocumentNumber);

        public override bool HaveAccess(string fullModel, string Id)
        {
            var currentUser = Authentication.GetCurrentUser();

            if (currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.MDAccess)))
                return true;

            return false;
        }

        public static MovingProduct PopulateById(long movingProductId)
        {
            var cmd = new SqlCommand("MovingProduct_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("MovingProductId", SqlDbType.BigInt) { Value = movingProductId });

            var movingProduct = new MovingProduct();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    movingProduct.FromDataRow(rdr);

                rdr.Close();
            }
            return movingProduct;
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "", SqlConnection connection = null, LIB.BusinessObjects.User user = null)
        {
            foreach (MovingProduct movingProduct in dictionary.Values)
            {
                var movingProductUnits = MovingProductUnit.PopulateByParentId(movingProduct.Id);
                new MovingProductUnit().Delete(movingProductUnits);
            }

            return base.Delete(dictionary, Comment, connection, user);
        }

        public override void Update(ItemBase item, LIB.AdvancedProperties.DisplayMode DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var movingProduct = (MovingProduct)item;

            var movingProductFromDB = PopulateById(movingProduct.Id);

            if (movingProduct.Date.Date != movingProductFromDB.Date.Date)
            {
                foreach (MovingProductUnit movingProductUnit in MovingProductUnit.PopulateByParentId(movingProduct.Id).Values)
                {
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, movingProductUnit.Count, movingProductFromDB.Date, actionType);
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, -movingProductUnit.Count, movingProductFromDB.Date, actionType);

                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockFrom, -movingProductUnit.Count, movingProduct.Date, actionType);
                    SpecificProductStock.UpdateCountInStock(movingProductUnit.SpecificProduct, movingProductUnit.StockTo, movingProductUnit.Count, movingProduct.Date, actionType);
                }
            }

            base.Update(item, DisplayMode, Comment, connection);
        }
    }
}