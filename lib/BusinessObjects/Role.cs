// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Role.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The role.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;

    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools;
    using LIB.Tools.AdminArea;
    using System.Collections.Generic;
    using Tools.Utils;
    using System.Data;
    using System.Data.SqlClient;
    using Tools.Security;
    using Helpers;

    /// <summary>
    /// The role.
    /// </summary>
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.UserManagement
      , ModulesAccess = (long)Modulesenum.SMI
      , DisplayName = "Роли"
      , SingleName = "Роль"
      , EditAccess = (long)BasePermissionenum.SuperAdmin
      , CreateAccess = (long)BasePermissionenum.SuperAdmin
      , DeleteAccess = (long)BasePermissionenum.SuperAdmin
      , ReadAccess = (long)BasePermissionenum.SuperAdmin
      , LogRevisions = true
      , RevisionsAccess = (long)BasePermissionenum.SuperAdmin
      , Icon = "users"
      )
    ]
    public class Role : AggregateBase
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role()
            : base(0)
        {
            this.Id = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Role(long id)
            : base(id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public Role(long permission, int id, string name)
        {
            this.Permission = permission;
            this.Id = id;
            this.Name = name;
        }
        #endregion

        #region Role Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Common(Order = 0), Template(Mode = Template.Name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        [Common(Order = 1)]
        [Template(Mode = Template.PermissionsSelector)]
        public long Permission { get; set; }

        /// <summary>
        /// Gets or sets the permissions req for user to create user with this permision.
        /// </summary>
        [Common(Order = 1, DisplayName = "Разрешение на создание пользователя с этими разрешениями")]
        [Template(Mode = Template.PermissionsSelector),Access(DisplayMode=DisplayMode.Advanced)]
        public long RoleAccessPermission { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [Common(Order = 2), Template(Mode = Template.Image), Image(ThumbnailWidth = 160, ThumbnailHeight = 160)]
        public Graphic Avatar { get; set; }
        #endregion

        /// <summary>
        /// The has permissions.
        /// </summary>
        public bool HasPermissions(long binaryFlags) => Permissions.HasPermissions(this.Permission,binaryFlags);

        /// <summary>
        /// The has at least one permission.
        /// </summary>
        public bool HasAtLeastOnePermission(long binaryFlags) => Permissions.HasAtLeastOnePermission(this.Permission, binaryFlags);
        
        public override string GetAdditionalSelectQuery(AdvancedProperty property) => ",[" + property.PropertyName + "].RoleAccessPermission AS " + property.PropertyName + "RoleAccessPermission";

        public List<AggregateBase> LoadUsersPerRoles()
        {
            var cmd = new SqlCommand("Role_Populate_Users", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var Roles = new List<AggregateBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                    Roles.Add((Role)new Role().FromDataRow(rdr));

                rdr.Close();
            }

            return Roles;
        }
    }
}