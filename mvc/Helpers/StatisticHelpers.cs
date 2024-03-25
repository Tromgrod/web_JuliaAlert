using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LIB.Tools.Utils;
using static JuliaAlert.Controllers.StatisticController;

public struct BarData
{
    public BarData(string name, int sales, int @return)
    {
        Name = name;
        Sales = sales;
        Return = @return;
    }

    public string Name;
    public int Sales;
    public int Return;
}

public struct MapData
{
    public MapData(long id, string name, int sales)
    {
        Id = id;
        Name = name;
        Sales = sales;
    }

    public long Id;
    public string Name;
    public int Sales;
}

namespace JuliaAlert.Helpers
{
    public static class StatisticHelpers
    {
        public static async Task<List<dynamic>> GetFrameDataAsync(CancellationToken cancellationToken, StatisticGroup statisticGroup, StatisticSqlGroup statisticSqlGroup, string years, int monthFrom, int monthTo, int countingType, string uniqueProducts, string salesChannels, string typeProducts, string countries, string dynamicFilterValues)
        {
            var cmd = new SqlCommand($"StatisticHelpers_FrameData_{statisticGroup}_{statisticSqlGroup}", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("Years", SqlDbType.NVarChar, 100) { Value = years });
            cmd.Parameters.Add(new SqlParameter("MonthFrom", SqlDbType.Int) { Value = monthFrom });
            cmd.Parameters.Add(new SqlParameter("MonthTo", SqlDbType.Int) { Value = monthTo });
            cmd.Parameters.Add(new SqlParameter("CountingType", SqlDbType.Int) { Value = countingType });

            if (!string.IsNullOrEmpty(uniqueProducts))
                cmd.Parameters.Add(new SqlParameter("UniqueProducts", SqlDbType.NVarChar, 100) { Value = uniqueProducts });
            if (!string.IsNullOrEmpty(salesChannels))
                cmd.Parameters.Add(new SqlParameter("SalesChannels", SqlDbType.NVarChar, 100) { Value = salesChannels });
            if (!string.IsNullOrEmpty(typeProducts))
                cmd.Parameters.Add(new SqlParameter("TypeProducts", SqlDbType.NVarChar, 100) { Value = typeProducts });
            if (!string.IsNullOrEmpty(countries))
                cmd.Parameters.Add(new SqlParameter("Countries", SqlDbType.NVarChar, 100) { Value = countries });

            if (!string.IsNullOrEmpty(dynamicFilterValues))
                cmd.Parameters.Add(new SqlParameter("DynamicFilterValues", SqlDbType.NVarChar, 100) { Value = dynamicFilterValues });

            var frameData = new List<dynamic>();

            var ds = new DataSet();

            using (var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken))
                ds.Load(rdr, LoadOption.OverwriteChanges, "Data");

            var table = ds.Tables["Data"];

            foreach (DataColumn column in table.Columns)
                frameData.Add(new { Name = column.ColumnName, Data = new List<object>() });

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    frameData.FirstOrDefault(data => data.Name == column.ColumnName).Data.Add(row[column.ColumnName]);
                }
            }

            return frameData;
        }

        public static async Task<List<int>> GetSalesTypeChartDataAsync(CancellationToken cancellationToken, StatisticGroup statisticGroup, string years, int monthFrom, int monthTo, int countingType, string uniqueProducts, string salesChannels, string typeProducts, string countries, string dynamicFilterValues)
        {
            var statisticGroupEmptyVal = GroupSearchData(dynamicFilterValues, statisticGroup, ref uniqueProducts, ref salesChannels, ref typeProducts, ref countries);

            if (!statisticGroupEmptyVal)
            {
                var cmd = new SqlCommand("StatisticHelpers_SalesTypeChartData", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("Years", SqlDbType.NVarChar, 100) { Value = years });
                cmd.Parameters.Add(new SqlParameter("MonthFrom", SqlDbType.Int) { Value = monthFrom });
                cmd.Parameters.Add(new SqlParameter("MonthTo", SqlDbType.Int) { Value = monthTo });
                cmd.Parameters.Add(new SqlParameter("CountingType", SqlDbType.Int) { Value = countingType });

                if (!string.IsNullOrEmpty(uniqueProducts))
                    cmd.Parameters.Add(new SqlParameter("UniqueProducts", SqlDbType.NVarChar, 100) { Value = uniqueProducts });
                if (!string.IsNullOrEmpty(salesChannels))
                    cmd.Parameters.Add(new SqlParameter("SalesChannels", SqlDbType.NVarChar, 100) { Value = salesChannels });
                if (!string.IsNullOrEmpty(typeProducts))
                    cmd.Parameters.Add(new SqlParameter("TypeProducts", SqlDbType.NVarChar, 100) { Value = typeProducts });
                if (!string.IsNullOrEmpty(countries))
                    cmd.Parameters.Add(new SqlParameter("Countries", SqlDbType.NVarChar, 100) { Value = countries });

                var salesTypeChartData = new List<int>();

                var ds = new DataSet();

                using (var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken))
                    ds.Load(rdr, LoadOption.OverwriteChanges, "Data");

                var table = ds.Tables["Data"];

                DataRow row = table.Rows[0];

                foreach (DataColumn column in table.Columns)
                    salesTypeChartData.Add((int)row[column.ColumnName]);

                return salesTypeChartData;
            }
            else
            {
                return new List<int> { 0, 0 };
            }
        }

        public static async Task<List<BarData>> GetBarDataAsync(CancellationToken cancellationToken, StatisticGroup statisticGroup, string years, int monthFrom, int monthTo, int countingType, string uniqueProducts, string salesChannels, string typeProducts, string countries, string dynamicFilterValues, BarType barType)
        {
            var statisticGroupEmptyVal = GroupSearchData(dynamicFilterValues, statisticGroup, ref uniqueProducts, ref salesChannels, ref typeProducts, ref countries);

            if (!statisticGroupEmptyVal)
            {
                var cmd = new SqlCommand($"StatisticHelpers_BarData_{barType}", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add(new SqlParameter("Years", SqlDbType.NVarChar, 100) { Value = years });
                cmd.Parameters.Add(new SqlParameter("MonthFrom", SqlDbType.Int) { Value = monthFrom });
                cmd.Parameters.Add(new SqlParameter("MonthTo", SqlDbType.Int) { Value = monthTo });
                cmd.Parameters.Add(new SqlParameter("CountingType", SqlDbType.Int) { Value = countingType });

                if (!string.IsNullOrEmpty(uniqueProducts))
                    cmd.Parameters.Add(new SqlParameter("UniqueProducts", SqlDbType.NVarChar, 100) { Value = uniqueProducts });
                if (!string.IsNullOrEmpty(salesChannels))
                    cmd.Parameters.Add(new SqlParameter("SalesChannels", SqlDbType.NVarChar, 100) { Value = salesChannels });
                if (!string.IsNullOrEmpty(typeProducts))
                    cmd.Parameters.Add(new SqlParameter("TypeProducts", SqlDbType.NVarChar, 100) { Value = typeProducts });
                if (!string.IsNullOrEmpty(countries))
                    cmd.Parameters.Add(new SqlParameter("Countries", SqlDbType.NVarChar, 100) { Value = countries });

                var barData = new List<BarData>();

                var ds = new DataSet();

                using (var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken))
                    ds.Load(rdr, LoadOption.OverwriteChanges, "Data");

                var table = ds.Tables["Data"];

                foreach (DataRow row in table.Rows)
                {
                    var name = (string)row["Name"];
                    var sales = (int)row["Sales"];
                    var @return = (int)row["Return"];

                    var barElem = new BarData(name, sales, @return);
                    barData.Add(barElem);
                }

                return barData;
            }
            else
            {
                return new List<BarData>();
            }
        }

        public static async Task<List<MapData>> GetMapDataAsync(CancellationToken cancellationToken)
        {
            var cmd = new SqlCommand("StatisticHelpers_MapData", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var mapData = new List<MapData>();

            var ds = new DataSet();

            using (var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken))
                ds.Load(rdr, LoadOption.OverwriteChanges, "Data");

            var table = ds.Tables["Data"];

            foreach (DataRow row in table.Rows)
            {
                var id = (long)row["Id"];
                var name = (string)row["Name"];
                var sales = (int)row["Sales"];

                var mapElem = new MapData(id, name, sales);
                mapData.Add(mapElem);
            }

            return mapData;
        }

        private static bool GroupSearchData(string dynamicFilterValues, StatisticGroup statisticGroup, ref string uniqueProducts, ref string salesChannels, ref string typeProducts, ref string countries)
        {
            bool statisticGroupEmptyVal = false;

            switch (statisticGroup)
            {
                case StatisticGroup.Product:
                    uniqueProducts = dynamicFilterValues;
                    statisticGroupEmptyVal = string.IsNullOrEmpty(uniqueProducts);
                    break;
                case StatisticGroup.SalesChannel:
                    salesChannels = dynamicFilterValues;
                    statisticGroupEmptyVal = string.IsNullOrEmpty(salesChannels);
                    break;
                case StatisticGroup.TypeProduct:
                    typeProducts = dynamicFilterValues;
                    statisticGroupEmptyVal = string.IsNullOrEmpty(typeProducts);
                    break;
                case StatisticGroup.Country:
                    countries = dynamicFilterValues;
                    statisticGroupEmptyVal = string.IsNullOrEmpty(countries);
                    break;
            }

            return statisticGroupEmptyVal;
        }
    }
}