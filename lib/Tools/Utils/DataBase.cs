// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBase.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the DataBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// The data base.
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// The get reader value.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetReaderValue(IDataReader rdr, DataRow dr, string valueKey, object defaultValue)
        {
            if (rdr != null)
            {
                return GetReaderValue(rdr, valueKey, defaultValue);
            }

            return GetReaderValue(dr, valueKey, defaultValue);
        }

        /// <summary>
        /// The get reader value.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetReaderValue(IDataReader rdr, string valueKey, object defaultValue)
        {
            return rdr[valueKey] != DBNull.Value ? rdr[valueKey] : defaultValue;
        }

        /// <summary>
        /// The get reader value.
        /// </summary>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetReaderValue(DataRow dr, string valueKey, object defaultValue)
        {
            return dr[valueKey] != DBNull.Value ? dr[valueKey] : defaultValue;
        }

        /// <summary>
        /// The get exist reader value.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetExistReaderValue(IDataReader rdr, DataRow dr, string valueKey, object defaultValue)
        {
            return rdr != null ? GetExistReaderValue(rdr, valueKey, defaultValue) : GetExistReaderValue(dr, valueKey, defaultValue);
        }

        /// <summary>
        /// The get exist reader value.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetExistReaderValue(IDataReader rdr, string valueKey, object defaultValue)
        {
            try
            {
                if (rdr[valueKey] != DBNull.Value)
                {
                    return rdr[valueKey];
                }
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// The get exist reader value.
        /// </summary>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetExistReaderValue(DataRow dr, string valueKey, object defaultValue)
        {
            if (dr.Table.Columns.Contains(valueKey) && dr[valueKey] != DBNull.Value)
            {
                return dr[valueKey];
            }

            return defaultValue;
        }

        /// <summary>
        /// The reader value exist.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ReaderValueExist(IDataReader rdr, DataRow dr, string valueKey)
        {
            return rdr != null ? ReaderValueExist(rdr, valueKey) : ReaderValueExist(dr, valueKey);
        }

        /// <summary>
        /// The reader value exist.
        /// </summary>
        /// <param name="rdr">
        /// The data reader.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ReaderValueExist(IDataReader rdr, string valueKey)
        {
            try
            {
                if (rdr[valueKey] != DBNull.Value)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// The reader value exist.
        /// </summary>
        /// <param name="dr">
        /// The data row.
        /// </param>
        /// <param name="valueKey">
        /// The value key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ReaderValueExist(DataRow dr, string valueKey)
        {
            if (dr.Table.Columns.Contains(valueKey) && dr[valueKey] != DBNull.Value)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The create new connection.
        /// </summary>
        /// <param name="connectionKey">
        /// The connection key.
        /// </param>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        public static SqlConnection CreateNewConnection(string connectionKey = "")
        {
            if (string.IsNullOrEmpty(connectionKey))
            {
                connectionKey = Config.PrimaryConnectionId;
            }

            var conn = new SqlConnection(Config.GetConnectionString(connectionKey));

            conn.Open();

            return conn;
        }

        /// <summary>
        /// The connection from context.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        public static SqlConnection ConnectionFromContext()
        {
            if (null == new HttpContextWrapper(HttpContext.Current))
            {
                return null;
            }

            var conn = ContextItemsHolder.ObjectFromContext(ContextItemsHolder.ConnectionContextId) as SqlConnection;

            if (conn == null)
            {
                conn = CreateNewConnection();
                ConnectionToContext(conn);
            }
            else if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            return conn;
        }

        /// <summary>
        /// The connection from context.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        public static void CloseConnection()
        {
            if (null == new HttpContextWrapper(HttpContext.Current))
            {
                return;
            }

            var conn = ContextItemsHolder.ObjectFromContext(ContextItemsHolder.ConnectionContextId) as SqlConnection;

            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// The connection to context.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ConnectionToContext(SqlConnection conn)
        {
            if (null == new HttpContextWrapper(HttpContext.Current))
            {
                return false;
            }

            if (null != ContextItemsHolder.ObjectFromContext(ContextItemsHolder.ConnectionContextId))
            {
                var lconn = ContextItemsHolder.ObjectFromContext(ContextItemsHolder.ConnectionContextId) as SqlConnection;
                if (lconn != null && lconn.State == ConnectionState.Open)
                {
                    lconn.Close();
                }
            }

            ContextItemsHolder.ObjectToContext(ContextItemsHolder.ConnectionContextId, conn);

            return true;
        }

        public static string GenerateParamSize(int ParamSize)
        {
            var result="";
            if (ParamSize == -1)
            {
                result = "(MAX)";

            }
            if(ParamSize > 0){
                result = "(" + ParamSize.ToString(CultureInfo.InvariantCulture) + ")";
            } 

            return result;
        }
    }
}