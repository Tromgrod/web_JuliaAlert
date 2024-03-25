// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortParameter.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The translation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.BO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using LIB.BusinessObjects;
    //using LIB.Tools.Memcached;
    using LIB.Tools.Utils;

    /// <summary>
    /// The translation.
    /// </summary>
    [Serializable]
    public class SortParameter
    {
        public string Direction { get; set; }
        public string Field { get; set; }
    }
}