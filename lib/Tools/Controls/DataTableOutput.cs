// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortParameter.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The translation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using LIB.BusinessObjects;
    using LIB.Tools.Utils;

    /// <summary>
    /// The translation.
    /// </summary>
    [Serializable]
    public class DataTableOutput
    {
        public int sEcho { get; set; }
        public long iTotalRecords { get; set; }
        public long iTotalDisplayRecords { get; set; }
        public List<List<string>> aaData { get; set; }
    }
}