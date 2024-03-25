using System;
using System.Data;
using System.Data.SqlClient;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable, Bo]
    public class FindingLocationStorage : ItemBase
    {
        #region Constructors
        public FindingLocationStorage()
            : base(0) { }

        public FindingLocationStorage(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.ParentDropDown)]
        public LocationStorage LocationStorage { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public FindingColor FindingColor { get; set; }

        [Template(Mode = Template.Decimal)]
        public decimal CurrentCount { get; set; }
        #endregion

        public static FindingLocationStorage PopulateByFindingColorAndLocationStorage(long locationStorageId, FindingColor findingColor)
        {
            var cmd = new SqlCommand("FindingLocationStorage_PopulateByFindingColorAndLocationStorage", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("LocationStorageId", SqlDbType.BigInt) { Value = locationStorageId });
            cmd.Parameters.Add(new SqlParameter("FindingColorId", SqlDbType.BigInt) { Value = findingColor.Id });

            var findingLocationStorage = new FindingLocationStorage();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    findingLocationStorage.FromDataRow(rdr);

                rdr.Close();
            }

            if (findingLocationStorage.Id <= 0)
            {
                findingLocationStorage.LocationStorage = new LocationStorage(locationStorageId);
                findingLocationStorage.FindingColor = findingColor;

                findingLocationStorage.Insert(findingLocationStorage);
            }

            return findingLocationStorage;
        }
    }
}