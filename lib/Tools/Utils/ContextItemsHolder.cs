// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextItemsHolder.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the ContextItemsHolder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;

    /// <summary>
    /// The context items holder.
    /// </summary>
    public class ContextItemsHolder
    {
        /// <summary>
        /// The connection context id.
        /// </summary>
        public const string ConnectionContextId = "SqlConn";

        public const string DropDownLookUp = "DropDownLookUp";

        /// <summary>
        /// The object from context.
        /// </summary>
        /// <param name="objectContextId">
        /// The object context id.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ObjectFromContext(string objectContextId)
        {
            return null == new HttpContextWrapper(HttpContext.Current) ? null : new HttpContextWrapper(HttpContext.Current).Items[objectContextId];
        }

        /// <summary>
        /// The object to context.
        /// </summary>
        /// <param name="objectContextId">
        /// The object context id.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ObjectToContext(string objectContextId, object obj)
        {
            if (null == new HttpContextWrapper(HttpContext.Current))
            {
                return false;
            }

            new HttpContextWrapper(HttpContext.Current).Items[objectContextId] = obj;
            return true;
        }
    }
}