using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Helpers;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(LogRevisions = true)]
    public class Return : ItemBase
    {
        #region Constructors
        public Return()
            : base(0) { }

        public Return(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public ProductForOrder ProductForOrder { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime ReturnDate { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime ReceivingReturnDate { get; set; }

        [Template(Mode = Template.String)]
        public string CauseReturn { get; set; }

        [Template(Mode = Template.Number)]
        public int ReturnCount { get; set; }

        [Template(Mode = Template.CheckBox)]
        public bool InCountry { get; set; }

        [Template(Mode = Template.String)]
        public string TrackingNumber { get; set; }
        #endregion

        #region Override Methods
        public override RequestResult SaveForm()
        {
            if (this.ReceivingReturnDate != DateTime.MinValue && this.ReturnDate > this.ReceivingReturnDate)
                return new RequestResult() { Result = RequestResultType.Fail, Message = "Ошибка в датах" };

            if (ProductForOrder.PopulateById(this.ProductForOrder.Id).Count < this.ReturnCount)
                return new RequestResult() { Result = RequestResultType.Fail, Message = "Количество возврата больше количества заказа" };

            return base.SaveForm();
        }

        public override string GetCaption() => string.Empty;

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Insert;

            var @return = (Return)item;

            var productForOrder = ProductForOrder.PopulateById(@return.ProductForOrder.Id);

            if (@return.ReceivingReturnDate != default)
            {
                SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, @return.ReturnCount, @return.ReceivingReturnDate, actionType);
            }

            base.Insert(item, Comment, connection, user);
        }

        public override void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Update;

            var @return = (Return)item;
            var returnFromDB = PopulateById(@return.Id);

            var productForOrder = ProductForOrder.PopulateById(@return.ProductForOrder.Id);

            if (@return.ReceivingReturnDate != default)
            {
                if (returnFromDB.ReceivingReturnDate.Date != default && @return.ReceivingReturnDate.Date != returnFromDB.ReceivingReturnDate.Date)
                {
                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, -returnFromDB.ReturnCount, returnFromDB.ReceivingReturnDate, actionType);
                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, @return.ReturnCount, @return.ReceivingReturnDate, actionType);
                }
                else if (returnFromDB.ReturnCount - @return.ReturnCount != default)
                {
                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, @return.ReturnCount - returnFromDB.ReturnCount, @return.ReceivingReturnDate, actionType);
                }
            }
            else if (returnFromDB.ReceivingReturnDate != default)
            {
                SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, -returnFromDB.ReturnCount, returnFromDB.ReceivingReturnDate, actionType);
            }

            base.Update(item, DisplayMode, Comment, connection);
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            var actionType = SpecificProductStockHistory.ActionTypeEnum.Delete;

            foreach (Return @return in dictionary.Values)
            {
                var productForOrder = ProductForOrder.PopulateById(@return.ProductForOrder.Id);

                if (@return.ReceivingReturnDate != default)
                {
                    SpecificProductStock.UpdateCountInStock(productForOrder.SpecificProduct, productForOrder.Order.Stock, -@return.ReturnCount, @return.ReceivingReturnDate, actionType);
                }
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
        #endregion

        #region Populate
        public static Return PopulateById(long returnId)
        {
            var cmd = new SqlCommand("Return_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ReturnId", SqlDbType.BigInt) { Value = returnId });

            var @return = new Return();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    @return.FromDataRow(rdr);

                rdr.Close();
            }
            return @return;
        }

        public static Dictionary<DateTime, int> PopulateCountByYear(int Year, SalesChannel salesChannel = null)
        {
            var cmd = new SqlCommand("Populate_Return_CountByYear", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("Year", SqlDbType.Int) { Value = Year });
            if (salesChannel != null)
                cmd.Parameters.Add(new SqlParameter("SalesChannelId", SqlDbType.BigInt) { Value = salesChannel.Id });

            var ds = new DataSet();
            var da = new SqlDataAdapter { SelectCommand = cmd };
            da.Fill(ds);

            var Counts = new Dictionary<DateTime, int>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var returnDate = (DateTime)dr[nameof(ReturnDate)];
                var returnCount = (int)dr[nameof(ReturnCount)];

                Counts.Add(returnDate, returnCount);
            }
            return Counts;
        }
        #endregion
    }
}