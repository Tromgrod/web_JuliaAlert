// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Users.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.Utils;
    using System.Data.SqlClient;
    using System.Data;
    using LIB.Tools.AdminArea;

    [Serializable]
    [Bo(Group = AdminAreaGroupenum.UserManagement
      , ModulesAccess = (long)(Modulesenum.SMI)
      , DisplayName = "Пользователи"
      , SingleName = "Пользователя"
      , DeleteAccess = (long)BasePermissionenum.SuperAdmin
      , LogRevisions = true
      , AllowCopy = false
      , RevisionsAccess = (long)BasePermissionenum.SuperAdmin
      , Icon = "user")]
    public class User : LIB.BusinessObjects.User
    {
        #region Constructors
        public User()
            : base(0) { }

        public User(long id)
            : base(id) { }

        public User(string login, string password)
            : base(login, password) { }
        #endregion

        #region UPDATE
        public static void UpdatePassword(LIB.BusinessObjects.User usr)
        {
            var conn = DataBase.ConnectionFromContext();
            var cmd = new SqlCommand("User_UpdatePassword", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = usr.Id });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = usr.GetpasswordHash() });
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region POPULATE
        public static User Populate(LIB.BusinessObjects.User usr)
        {
            var cmd = new SqlCommand("User_Populate", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = usr.Id });

            var user = new User();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    user.FromDataRow(rdr);

                rdr.Close();
            }
            return user;
        }
        #endregion
    }
}