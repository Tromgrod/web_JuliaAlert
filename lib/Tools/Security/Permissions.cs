// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Permission type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Security
{
    using LIB.BusinessObjects;

    /// <summary>
    /// The permissions.
    /// </summary>
    public class Permissions
    {
        /// <summary>
        /// The has permissions.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="binaryFlags">
        /// The binary flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasPermissions(User user, long binaryFlags )
        {
            return HasPermissions(user.Permission, binaryFlags) || HasPermissions(user.Role.Permission, binaryFlags);
        }

        /// <summary>
        /// The has permissions.
        /// </summary>
        /// <param name="permissions">
        /// The permissions.
        /// </param>
        /// <param name="binaryFlags">
        /// The p binary flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasPermissions(long permissions, long binaryFlags)
        {
            if ((permissions & (long)BasePermissionenum.SuperAdmin) != 0)
                return true;
            return (permissions & binaryFlags) == binaryFlags;
        }

        /// <summary>
        /// The has at least one permission.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="binaryFlags">
        /// The binary flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasAtLeastOnePermission(User user, long binaryFlags)
        {
            return HasAtLeastOnePermission(user.Permission, binaryFlags) || HasAtLeastOnePermission(user.Role.Permission, binaryFlags);
        }

        /// <summary>
        /// The has at least one permission.
        /// </summary>
        /// <param name="permissions">
        /// The permissions.
        /// </param>
        /// <param name="binaryFlags">
        /// The Binary Flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasAtLeastOnePermission(long permissions, long binaryFlags)
        {
            if (binaryFlags == 0)
            {
                return true;
            }
            if ((permissions & (long)BasePermissionenum.SuperAdmin) != 0)
                return true;

            return (permissions & binaryFlags) != 0;
        }
    }
}
