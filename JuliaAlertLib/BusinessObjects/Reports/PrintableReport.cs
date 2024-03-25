// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Contacts.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The Contact.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using LIB.Tools.AdminArea;
    using System.Collections.Generic;
    using LIB.Tools.Utils;
    using System.Data.SqlClient;
    using System.Data;

    /// <summary>
    /// The Contact.
    /// </summary>
    [Serializable]
    [Bo(ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Rapoarte de tipar"
       , SingleName = "Raport de tipar"
       , DoCancel = true
       , LogRevisions = true)]
    public class PrintableReport : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public PrintableReport()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public PrintableReport(long id)
            : base(id)
        {
        }
        #endregion        

        #region Properties

        [Common(DisplayName="Denumire"),Template(Mode = Template.Name)]
        public string Name { get; set; }
        
        [Common(DisplayName="Activ"),Template(Mode = Template.CheckBox)]
        public bool Enabled { get; set; }

        [Template(Mode = Template.ParentDropDown)]
        public PrintableReportType PrintableReportType { get; set; }

        [Template(Mode = Template.Html)]
        public string ReportTemplate { get; set; }

        [Template(Mode = Template.Html)]
        public string WordTemplate { get; set; }

        #endregion


        public static PrintableReport LoadTemplate(string ptype)
        {
            var conn = DataBase.ConnectionFromContext();

            var cmd = new SqlCommand("PrintableReport_LoadTemplate", conn) { CommandType = CommandType.StoredProcedure };
            
            cmd.Parameters.Add(new SqlParameter("@Tag", SqlDbType.NVarChar, 50) { Value = ptype });

            PrintableReport lPrintableReport = null;
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    lPrintableReport = (PrintableReport)(new PrintableReport()).FromDataRow(rdr);
                    break;
                }
                rdr.Close();
            }

            return lPrintableReport;
        }
    }
}

