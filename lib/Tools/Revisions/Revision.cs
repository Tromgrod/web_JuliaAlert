// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Revisions.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The revisions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Revisions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using LIB.BusinessObjects;
    using LIB.Tools.Utils;
    using LIB.Tools.Security;

    /// <summary>
    /// The translation.
    /// </summary>
    [Serializable]
    public class Revision
    {

        #region Revision Properties

        public long Id { get; set; }
        public string Table { get; set; }
        public long BOId { get; set; }
        public string BOName { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public OperationTypes Type { get; set; }
        public string Comment { get; set; }
        public string Json { get; set; }

        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case OperationTypes.Cancel:
                        return "ban";
                    case OperationTypes.Update:
                        return "edit";
                    case OperationTypes.Delete:
                        return "trash-o";
                    case OperationTypes.Insert:
                        return "plus";
                }

                return "";
            }
        }

        public string Color
        {
            get
            {
                switch (Type)
                {
                    case OperationTypes.Cancel:
                        return "maroon";
                    case OperationTypes.Update:
                        return "aqua";
                    case OperationTypes.Delete:
                        return "red";
                    case OperationTypes.Insert:
                        return "purple";
                }

                return "";
            }
        }
        #endregion

        public static OperationTypes LoadOperationType(int type)
        {
            switch (type)
            {
                case (int)OperationTypes.Cancel:
                    return OperationTypes.Cancel;
                case (int)OperationTypes.Update:
                    return OperationTypes.Update;
                case (int)OperationTypes.Delete:
                    return OperationTypes.Delete;
                case (int)OperationTypes.Insert:
                    return OperationTypes.Insert;
            }
            return OperationTypes.Update;
        }

        public static Dictionary<DateTime,List<Revision>> LoadRevisions(string table, long Id, SqlConnection conn=null)
        {
            if (conn == null)
            {
                conn = DataBase.ConnectionFromContext();
            }

            const string StrSql = "BORevisions_Populate";

            var sqlCommand = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@Table", SqlDbType.NVarChar, 1000) { Value = table };
            sqlCommand.Parameters.Add(param);

            param = new SqlParameter("@Id", SqlDbType.BigInt) { Value = Id };
            sqlCommand.Parameters.Add(param);

            var revs = new Dictionary<DateTime, List<Revision>>();

            using (var rdr = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var Revision = new Revision();
                    Revision.Id = (long)rdr["BORevisionId"];
                    Revision.Table = (string)rdr["Table"];
                    Revision.BOId = (long)rdr["BOId"];
                    Revision.BOName = rdr["BOName"] != DBNull.Value ? (string)rdr["BOName"] : "";
                    Revision.User = (User)(new User()).FromDataRow(rdr); ;
                    Revision.Date = (DateTime)rdr["Date"];
                    Revision.Type = LoadOperationType(Convert.ToInt32(rdr["Type"]));
                    Revision.Comment = rdr["Comment"]!=DBNull.Value?(string)rdr["Comment"]:"";
                    Revision.Json = rdr["Json"] != DBNull.Value ? (string)rdr["Json"] : "";

                    var date = Revision.Date.Date;

                    if(!revs.ContainsKey(date))
                        revs.Add(date,new List<Revision>());

                    revs[date].Add(Revision);      
                  
                }

                rdr.Close();
            }

            return revs;
        }
        public static Dictionary<DateTime, List<Revision>> LoadRevisions(User user, SqlConnection conn = null)
        {
            if (conn == null)
            {
                conn = DataBase.ConnectionFromContext();
            }

            const string StrSql = "BORevisions_Populate_By_User";

            var sqlCommand = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@UserId", SqlDbType.BigInt) { Value = user.Id };
            sqlCommand.Parameters.Add(param);

            var revs = new Dictionary<DateTime, List<Revision>>();

            using (var rdr = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var Revision = new Revision();
                    Revision.Id = (long)rdr["BORevisionId"];
                    Revision.Table = (string)rdr["Table"];
                    Revision.BOId = (long)rdr["BOId"];
                    Revision.BOName = rdr["BOName"] != DBNull.Value ? (string)rdr["BOName"] : "";
                    Revision.User = (User)(new User()).FromDataRow(rdr); ;
                    Revision.Date = (DateTime)rdr["Date"];
                    Revision.Type = LoadOperationType(Convert.ToInt32(rdr["Type"]));
                    Revision.Comment = rdr["Comment"] != DBNull.Value ? (string)rdr["Comment"] : "";
                    Revision.Json = rdr["Json"] != DBNull.Value ? (string)rdr["Json"] : "";

                    var date = Revision.Date.Date;

                    if (!revs.ContainsKey(date))
                        revs.Add(date, new List<Revision>());

                    revs[date].Add(Revision);

                }

                rdr.Close();
            }

            return revs;
        }

        public static void Insert(Revision rev)
        {
            var conn = DataBase.ConnectionFromContext();
            var cmd = new SqlCommand("Revision_Insert", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = Authentication.GetCurrentUser().Id });
            cmd.Parameters.Add(new SqlParameter("@BOId", SqlDbType.BigInt) { Value = rev.BOId });
            cmd.Parameters.Add(new SqlParameter("@BOName", SqlDbType.NVarChar,200) { Value = rev.BOName });
            cmd.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar, 2000) { Value = rev.Comment });
            cmd.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime) { Value = rev.Date });
            if (!string.IsNullOrEmpty(rev.Json))
            {
                cmd.Parameters.Add(new SqlParameter("@Json", SqlDbType.NVarChar, -1) { Value = rev.Json });
            }
            cmd.Parameters.Add(new SqlParameter("@Table", SqlDbType.NVarChar, 2000) { Value = rev.Table });
            cmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.TinyInt) { Value = Convert.ToInt16(rev.Type) });

            cmd.ExecuteNonQuery();
        }
    }
}