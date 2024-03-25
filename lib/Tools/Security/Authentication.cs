// ------------------------------------------------------------------------------------------------------public --------------
// <copyright file="Authentication.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The authentication.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Security
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.Utils;

    /// <summary>
    /// The authentication.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// The do authorization.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The administrator area login.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool DoAuthorization(User user, SqlConnection conn = null, HttpContextBase context = null, Modulesenum module = Modulesenum.BaseModule)
        {
            if (conn == null)
            {
                conn = DataBase.ConnectionFromContext();
            }

            if (context == null)
            {
                context = new HttpContextWrapper(HttpContext.Current);
            }

            if (conn != null && conn.State == ConnectionState.Open)
            {
                const string StrSql = "User_DoLogin";

                var cmd = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

                var param = new SqlParameter("@Login", SqlDbType.NVarChar, 50) { Value = GetUserName(user) };
                cmd.Parameters.Add(param);

                return Authorization(conn, cmd, context, module, user);
            }
            else
            {
                throw new Exception("DataBase is not available");
            }
        }
        /// <summary>
        /// The check user.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        public static User CheckUser(string page, HttpContextBase context = null, Modulesenum module = Modulesenum.BaseModule)
        {
            if (context == null)
            {
                context = new HttpContextWrapper(HttpContext.Current);
            }

            if (context != null)
            {
                var user = GetCurrentUser(context, module);
                if (null == user)
                {
                    if (string.IsNullOrEmpty(page))
                    {
                         context.Session["EnterPage"]=context.Request.Url;
                    }
                    else
                    {
                        context.Session["EnterPage"] = page;
                    }
                }

                return user;
            }

            return null;
        }

        /// <summary>
        /// The get current user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        public static User GetCurrentUser(HttpContextBase context = null, Modulesenum module = Modulesenum.BaseModule)
        {
            if (context == null)
            {
                context = new HttpContextWrapper(HttpContext.Current);
            }

            if (context != null && context.Session!=null)
            {
                var ouser = context.Session[SessionItems.User];
                if (ouser != null)
                {
                    var user = ouser as User;
                    if (user != null)
                    {
                        return user;
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// The get current user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        public static void LogOff()
        {
            var context = new HttpContextWrapper(HttpContext.Current);

            if (context != null)
            {
                context.Session.Remove(SessionItems.User);
                context.Session.Remove(SessionItems.UserGuid);
                context.Session.Remove(SessionItems.Person);
            }
        }

        /// <summary>
        /// The get current user id.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static long GetCurrentUserId(HttpContextBase context = null, Modulesenum module = Modulesenum.BaseModule)
        {
            var user = GetCurrentUser(context, module);
            return user != null ? user.Id : 0;
        }

        /// <summary>
        /// The check current user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        public static bool CheckUser(HttpContextBase context = null, Modulesenum module = Modulesenum.BaseModule)
        {
            return GetCurrentUser(context, module) != null;
        }

        /// <summary>
        /// The authorization.
        /// </summary>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="admin">
        /// The administrator area login.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool Authorization(SqlConnection conn, SqlCommand cmd, HttpContextBase context, Modulesenum module, User user)
        {
            user.Password = user.GetpasswordHash();

            using (var rdrUsers = cmd.ExecuteReader())
            {
                while (rdrUsers.Read())
                {
                    var str = rdrUsers["password"].ToString().Trim();
                    if (user.Password == str)
                    {
                        user = (User)(new User()).FromDataRow(rdrUsers);

                        switch (module)
                        {
                            case Modulesenum.ControlPanel:
                                if (!user.HasPermissions((long)BasePermissionenum.CPAccess))
                                {
                                    return false;
                                }
                                break;

                            case Modulesenum.SMI:
                                if (!user.HasPermissions((long)BasePermissionenum.SMIAccess))
                                {
                                    return false;
                                }
                                break;
                        }

                        UpdateLastLogin(conn, user);
                        InsertUserToSession(user, context);

                        return true;
                    }
                }
            }

            return false;
        }

        public static void UpdateLastLogin(SqlConnection conn, User user)
        {
            // set last_login time
            var cmd = new SqlCommand("User_Updatelast_login", conn) { CommandType = CommandType.StoredProcedure };
            var param = new SqlParameter("@UserId", SqlDbType.NVarChar, 50) { Value = user.Id };
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// The insert user to session.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public static void InsertUserToSession(User user, HttpContextBase context = null)
        {
            if (context == null)
            {
                context = new HttpContextWrapper(HttpContext.Current);
            }

            if (context != null)
            {
                var maxTimeout = Convert.ToInt32(Config.GetConfigValue("MaxUserSessionTimeout"));
                context.Session.Timeout = (user.Timeout>0 && user.Timeout < maxTimeout) ? user.Timeout : maxTimeout;
                context.Session[SessionItems.User] = user;
                context.Session[SessionItems.UserGuid] = user.UniqueId;
            }
        }

        /// <summary>
        /// The get user name.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetUserName(User user)
        {
            var result = user.Login;
            var pi = user.GetType().GetProperty("Login");
            if (
                pi.GetCustomAttributes(typeof(Encryption), false)
                  .OfType<Encryption>()
                  .Any(encryption => encryption.Encrypted))
            {
                result = Crypt.Encrypt(user.Login, Config.GetConfigValue("CryptKey"));
            }

            return !string.IsNullOrEmpty(result)?result:"";
        }
    }
}
