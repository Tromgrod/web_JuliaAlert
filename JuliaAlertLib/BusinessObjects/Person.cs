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
    using System.Linq;
    using System.Data;
    using System.Collections.Generic;
    using LIB.Tools.BO;
    using LIB.Tools.AdminArea;
    using System.Web;
    using LIB.Tools.Security;

    /// <summary>
    /// The user.
    /// </summary>
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.UserManagement
      , ModulesAccess = (long)(Modulesenum.SMI)
      , DisplayName = "Личности"
      , SingleName = "Личность"
      , EditAccess = (long)(BasePermissionenum.CPAccess | BasePermissionenum.SMIAccess)
      , CreateAccess = (long)(BasePermissionenum.CPAccess | BasePermissionenum.SMIAccess)
      , DeleteAccess = (long)(BasePermissionenum.CPAccess | BasePermissionenum.SMIAccess)
      , ReadAccess = (long)(BasePermissionenum.CPAccess | BasePermissionenum.SMIAccess)
      , LogRevisions = true
      , RevisionsAccess = (long)(BasePermissionenum.CPAccess | BasePermissionenum.SMIAccess)
      , Icon = "male"
      )
    ]
    public class Person : LIB.BusinessObjects.Person
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        public Person()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Person(long id)
            : base(id)
        {
        }
        #endregion

        public override string GetName()
        {
            return LastName + " " + FirstName;
        }

        public override string GetCaption()
        {
            return "LastName";
        }

        public override string GetAdditionalSelectQuery(AdvancedProperty property)
        {
            return ",[" + property.PropertyName + "].FirstName" + " AS " + property.PropertyName + "FirstName";
        }

        #region Properties

        [Common(Order = 3, ControlClass = CssClass.Large), Template(Mode = Template.String)]
        public string Phone { get; set; }

        #endregion

        #region UPDATE Methods

        public static void UpdateEmail(LIB.BusinessObjects.User usr, System.String email)
        {
            var conn = DataBase.ConnectionFromContext();
            var cmd = new SqlCommand("Person_UpdateEmail", conn) { CommandType = CommandType.StoredProcedure };
            var param = new SqlParameter("@UserId", SqlDbType.BigInt) { Value = usr.Id };
            cmd.Parameters.Add(param);
            param = new SqlParameter("@Email", SqlDbType.NVarChar, 50) { Value = email };
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region POPULATE
        public static void AddPersonInfo(LIB.BusinessObjects.User user)
        {
            const string StrSql = "Person_LoadInfo";

            var conn = DataBase.ConnectionFromContext();

            var cmd = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@UserId", SqlDbType.BigInt) { Value = user.Id };
            cmd.Parameters.Add(param);

            var ds = new DataSet();

            var da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            ds.Tables[0].TableName = "Person";

            foreach (DataRow dr in ds.Tables["Person"].Rows)
            {
                var person = (Person)(new Person()).FromDataRow(dr);

                var context = new HttpContextWrapper(HttpContext.Current);

                context.Session[SessionItems.Person] = person;
            }
        }

        public static Dictionary<long, ItemBase> PopulateByPermission(long pPermission)
        {
            const string StrSql = "Person_Populate_ByPermission";

            var conn = DataBase.ConnectionFromContext();

            var selectCommand = new SqlCommand(StrSql, conn) { CommandType = CommandType.StoredProcedure };

            var ds = new DataSet();

            var da = new SqlDataAdapter();

            selectCommand.Parameters.Add(new SqlParameter("@Permission", SqlDbType.BigInt) { Value = pPermission });

            da.SelectCommand = selectCommand;
            da.Fill(ds);

            ds.Tables[0].TableName = "Person";

            var lPersons = new Dictionary<long, ItemBase>();

            foreach (DataRow dr in ds.Tables["Person"].Rows)
            {
                var lPerson = (Person)(new Person()).FromDataRow(dr);
                if (!lPersons.ContainsKey(lPerson.Id))
                {
                    lPersons.Add(lPerson.Id, lPerson);
                }
            }

            return lPersons;
        }
        #endregion

    }
}