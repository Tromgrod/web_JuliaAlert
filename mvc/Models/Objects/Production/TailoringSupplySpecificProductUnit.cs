using System;
using LIB.Tools.BO;
using LIB.AdvancedProperties;
using LIB.Tools.Utils;
using System.Data.SqlClient;
using System.Data;
using LIB.Helpers;
using System.Web;
using LIB.BusinessObjects;
using System.Collections.Generic;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class TailoringSupplySpecificProductUnit : ItemBase
    {
        #region Constructors
        public TailoringSupplySpecificProductUnit()
            : base(0) { }

        public TailoringSupplySpecificProductUnit(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public SupplySpecificProductUnit SupplySpecificProductUnit { get; set; }

        [Template(Mode = Template.DateTime)]
        public DateTime Date { get; set; }

        [Template(Mode = Template.Number)]
        public int Count { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public Factory FactoryTailoring { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal TailoringCost { get; set; }

        [Db(_Ignore = true)]
        public string DateString => this.Date != DateTime.MinValue ? this.Date.ToString("dd/MM/yyyy") : string.Empty;
        #endregion

        public override string GetName() => this.SupplySpecificProductUnit.Id.ToString();

        public override string GetCaption() => nameof(this.SupplySpecificProductUnit) + nameof(this.SupplySpecificProductUnit.Id);

        public override object LoadPopupData(long itemId) => SupplySpecificProductUnit.PopulateTailoringSupplySpecificProductUnits(itemId);

        public override object PopulatePrintData() => FindingLocationStorageTailoringSupplySpecificProductUnit.PopulateByParentId(this.Id);

        public static TailoringSupplySpecificProductUnit PopulateById(long implementSupplySpecificProductUnitId)
        {
            var cmd = new SqlCommand("TailoringSupplySpecificProductUnit_PopulateById", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("TailoringSupplySpecificProductUnitId", SqlDbType.BigInt) { Value = implementSupplySpecificProductUnitId });

            var tailoringSupplySpecificProductUnit = new TailoringSupplySpecificProductUnit();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    tailoringSupplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }

            return tailoringSupplySpecificProductUnit;
        }

        public static TailoringSupplySpecificProductUnit GetLastByParrent(SupplySpecificProductUnit supplySpecificProductUnit)
        {
            var cmd = new SqlCommand("TailoringSupplySpecificProductUnit_GetLastByParrent", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SupplySpecificProductUnitId", SqlDbType.BigInt) { Value = supplySpecificProductUnit.Id });

            var tailoringSupplySpecificProductUnit = new TailoringSupplySpecificProductUnit();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    tailoringSupplySpecificProductUnit.FromDataRow(rdr);

                rdr.Close();
            }
            return tailoringSupplySpecificProductUnit;
        }

        public override RequestResult SaveForm()
        {
            int.TryParse(HttpContext.Current.Request.Form["SupplySpecificProductUnitCount"], out var supplySpecificProductUnitCount);
            int.TryParse(HttpContext.Current.Request.Form["TailoringSupplySpecificProductUnitCount"], out var tailoringSupplySpecificProductUnitCount);
            int.TryParse(HttpContext.Current.Request.Form["TotalTailoringCount"], out var totalTailoringCount);

            if (totalTailoringCount - tailoringSupplySpecificProductUnitCount + this.Count <= supplySpecificProductUnitCount)
                return base.SaveForm();
            else
                return new RequestResult { Result = RequestResultType.Fail, Message = "Общее количество пошива больше чем в прайс листе" };
        }

        public override bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            foreach (var deleteItem in dictionary.Values)
            {
                var tailoringSupplySpecificProductUnit = PopulateById(deleteItem.Id);

                var findingLocationStorageTailoringSupplySpecificProductUnits = FindingLocationStorageTailoringSupplySpecificProductUnit.PopulateByParentId(tailoringSupplySpecificProductUnit.Id);

                foreach(FindingLocationStorageTailoringSupplySpecificProductUnit findingLocationStorageTailoringSupplySpecificProductUnit in findingLocationStorageTailoringSupplySpecificProductUnits.Values)
                {
                    findingLocationStorageTailoringSupplySpecificProductUnit.Delete();
                }

                tailoringSupplySpecificProductUnit.Delete();
            }

            return base.Delete(dictionary, Comment, connection, user);
        }
    }
}