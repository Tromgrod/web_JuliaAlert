using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LIB.BusinessObjects;
using LIB.Tools.Utils;
using JuliaAlertLib.BusinessObjects;
using User = JuliaAlertLib.BusinessObjects.User;

namespace JuliaAlert.Helpers
{
    public class BasicAuthHttpModule : Weblib.Helpers.BasicAuthHttpModule
    {
        protected override LIB.BusinessObjects.User GetUser(string username)
        {
            User user = null;
            var conn = DataBase.ConnectionFromContext();
            const string StrSql = "User_DoLogin";

            var cmd = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@Login", SqlDbType.NVarChar, 50) { Value = username };
            cmd.Parameters.Add(param);

            using (var rdrUsers = cmd.ExecuteReader())
            {
                while (rdrUsers.Read())
                {
                    user = (User)(new User()).FromDataRow(rdrUsers);
                    break;
                }
            }

            return user;
        }
    }
}